/*
 黃文博
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using BLL.gigade.Model.Query;
using DBAccess;
using Sphinx.Client.Connections;
using Sphinx.Client.Commands.Search;
using Sphinx.Client.Common;
using Sphinx.Client.Commands;
using BLL.gigade.Model;
namespace BLL.gigade.Dao
{
    public class ProductSearchDao
    {
        IDBAccess _accessMySql;
        string warnings = string.Empty;
        string _sphinxHost = String.Empty;
        int _sphinxPort = 0;

        public ProductSearchDao(string connectionstring)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionstring);
        }
        public ProductSearchDao(string connectionstring, string sphinxHost, string sphinxPort)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            _sphinxHost = GetParameterCode(sphinxHost);
            _sphinxPort = 0;
             if (!string.IsNullOrEmpty(GetParameterCode(sphinxPort)))
             {
                 _sphinxPort = Convert.ToInt32(GetParameterCode(sphinxPort));
             }
            
        }
        
        #region 建立連接并獲取符合條件的idList
        /// <summary>
        /// 建立連接并獲取符合條件的idList
        /// GetIdList獲取查詢框對應的idList
        /// GetFlagList獲取食安關鍵字對應的idList
        /// GetSphinxExcludeIdList獲取要排除的idList
        /// </summary>
        /// <param name="query"></param>
        public List<int> GetIdList(string key,int MaxMatches)
        {
            List<int> idList = new List<int>();
            using (TcpConnection connection = new PersistentTcpConnection(_sphinxHost, _sphinxPort))
            {
               
                try
                {
                    ///獲得全部要搜索的關鍵字
                    SearchCommand search = new SearchCommand(connection);
                    ///綁定搜索關鍵字
                    SearchQuery searchQuery = new SearchQuery(key);
                    searchQuery.Limit = searchQuery.MaxMatches = MaxMatches;
                    search.QueryList.Add(searchQuery);
                    search.Execute();
                    if (search.Result.Status == CommandStatus.Warning)
                    {
                        foreach (var s in search.Result.Warnings)
                        {
                            warnings += s;
                        }
                    }
                    foreach (SearchQueryResult res in search.Result.QueryResults)
                    {
                        if (res.HasWarning)
                            warnings += res.Warning;
                        foreach (Match match in res.Matches)
                        {
                            ///取出全部符合條件的Id
                            int strId = Convert.ToInt32(match.DocumentId);
                            if (!idList.Contains(strId))
                            {
                                idList.Add(strId);
                            }
                        }
                    }
                        
                }
                catch (SphinxException ex)
                {
                    throw new Exception("ProductSearchDao-->GetIdList:warnings: " + warnings + "連接失敗信息:" + ex.Message);
                }
            }
            return idList;
        }
        public List<int> GetFlagList(List<string> keyList, int MaxMatches)
        {
            List<int> idList = new List<int>();
            using (TcpConnection connection = new PersistentTcpConnection(_sphinxHost, _sphinxPort))
            {
                try
                {
                    if (keyList.Count > 0)
                    {
                        foreach (var key in keyList)
                        {
                            ///獲得全部要搜索的關鍵字
                            SearchCommand search = new SearchCommand(connection);
                            ///綁定搜索關鍵字
                            SearchQuery searchQuery = new SearchQuery(key);
                            searchQuery.Limit = searchQuery.MaxMatches = MaxMatches;
                            search.QueryList.Add(searchQuery);
                            search.Execute();
                            if (search.Result.Status == CommandStatus.Warning)
                            {
                                foreach (var s in search.Result.Warnings)
                                {
                                    warnings += s;
                                }
                            }
                            foreach (SearchQueryResult res in search.Result.QueryResults)
                            {
                                if (res.HasWarning)
                                    warnings += res.Warning;
                                foreach (Match match in res.Matches)
                                {
                                    ///取出全部符合條件的Id
                                    int strId = Convert.ToInt32(match.DocumentId);
                                    if (!idList.Contains(strId))
                                    {
                                        idList.Add(strId);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (SphinxException ex)
                {
                    throw new Exception("ProductSearchDao-->GetFlagList:warnings: " + warnings + "連接失敗信息:" + ex.Message);
                }
            }
            return idList;
        }
        public List<int> GetSphinxExcludeIdList()
        {
            StringBuilder sql = new StringBuilder();
            List<int> sphinxIdList = new List<int> { };
            //sphinxIdList.Clear();
            try
            {
                sql.Append("SELECT se.product_id as product_id from sphinx_exclude se");
                DataTable _dt = _accessMySql.getDataTable(sql.ToString());
                for(int i=0;i<_dt.Rows.Count;i++)
                {
                    if(!string.IsNullOrEmpty(_dt.Rows[i]["product_id"].ToString()))
                    {
                        sphinxIdList.Add((Convert.ToInt32(_dt.Rows[i]["product_id"].ToString())));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSearchDao-->GetSphinxExclude:" + sql.ToString() + ex.Message);
            }
            return sphinxIdList;
        }
        #endregion
        #region 獲取對應的全部初始數據
        /// <summary>
        /// 獲取包含id對應的數據
        /// </summary>
        /// <param name="query"></param>
        /// <param name="idList"></param>
        /// <returns></returns>
        public List<ProductSearchQuery> GetListDetail(ProductSearchQuery query, List<int> idList, out int totalCount)
        {
            ///獲取對應的數據
            StringBuilder sql = new StringBuilder();
            ///搜索條件
            StringBuilder sqlWhere = new StringBuilder();
            ///計算滿足數據的總數
            StringBuilder sqlCount = new StringBuilder();
            List<ProductSearchQuery> listStore = new List<ProductSearchQuery>();
            ///排除SphinxExclude的product_id
            List<SphinxExclude> ListExclude = new List<SphinxExclude>();
            totalCount = 0;
            try
            {
                ///獲得數據sql 
                sql.AppendFormat("select product_id,product_name,page_content_1,page_content_2,page_content_3,product_keywords,product_detail_text ");
                ///查詢總數sql
                sqlCount.Append(" select count(product_id) as totalCount ");
                ///條件限制aql
                sqlWhere.Append(" from  product  where 1=1 ");
                if (idList.Count != 0)
                {
                    sqlWhere.AppendFormat(" AND product_id in ( ");
                    for (int index = 0; index < idList.Count; index++)
                    {
                        sqlWhere.AppendFormat(" {0},", idList[index]);
                    }
                    sqlWhere.Remove(sqlWhere.Length - 1, 1);
                    sqlWhere.Append(")");
                    
                    ///是否分頁
                    if (query.IsPage)
                    {
                        sqlCount.Append(sqlWhere.ToString());
                        DataTable _dt = _accessMySql.getDataTable(sqlCount.ToString());
                        if (_dt != null && _dt.Rows.Count > 0)
                        {
                            //得到滿足條件的總行數
                            totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                        }
                    }
                    ///得到應分頁的數據
                    sqlWhere.AppendFormat("  LIMIT {0},{1} ;", query.Start, query.Limit);
                    sql.Append(sqlWhere);
                    listStore = _accessMySql.getDataTableForObj<ProductSearchQuery>(sql.ToString());
                }

                return listStore;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSearchDao.GetListQuery-->" + ex.Message + sql.ToString());
            }
        }
        #endregion
        #region 處理舊數據并返回新數據
        public List<ProductSearchQuery> GetListResult(List<ProductSearchQuery> oldStore, List<string> keyList)
        {
            List<ProductSearchQuery> listStore = new List<ProductSearchQuery>();
            try
            {
                for (int index = 0; index < oldStore.Count; index++)
                {
                    if (keyList.Count > 0)
                    {
                        ProductSearchQuery query = new ProductSearchQuery();
                        query = oldStore[index];

                        foreach (var key in keyList)
                        {
                            query.Product_Id = oldStore[index].Product_Id;
                            query.Product_Name = query.Product_Name.Replace(key, "<b> <span style='color:#ff0000'>" + key + "</span></b>");
                            query.Page_Content_1 = query.Page_Content_1.Replace(key, "<b> <span style='color:#ff0000'>" + key + "</span></b>");
                            query.Page_Content_2 = query.Page_Content_2.Replace(key, "<b> <span style='color:#ff0000'>" + key + "</span></b>");
                            query.Page_Content_3 = query.Page_Content_3.Replace(key, "<b> <span style='color:#ff0000'>" + key + "</span></b>");
                            query.Product_Keywords = query.Product_Keywords.Replace(key, "<b> <span style='color:#ff0000'>" + key + "</span></b>");
                            query.product_detail_text = oldStore[index].product_detail_text.Replace(key, "<b> <span style='color:#ff0000'>" + key + "</span></b>");
                        }
                        listStore.Add(query);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSearchDao-->GetListResult:" + ex.Message);

            }
            return listStore;
        }
        #endregion
        
        #region 獲得全部要搜索的關鍵字
        public List<string> GetKeyList(ProductSearchQuery query)
        {
            StringBuilder sql = new StringBuilder();
            List<string> keyList = new List<string>();
            try
            {
               ///是食安關鍵字
               if (query.flag == "1" )
               {
                   sql.AppendFormat("SELECT key_word from sphinx_keyword where 1=1 AND flag='1' ");
               }
               else if (query.flag == "0")
               {
                   sql.AppendFormat("SELECT key_word from sphinx_keyword where 1=1 AND flag='3'");
               }

                DataTable dt = _accessMySql.getDataTable(sql.ToString());
                if (dt.Rows.Count > 0)
                {
                    for (int index = 0; index < dt.Rows.Count; index++)
                    {
                        keyList.Add(dt.Rows[index][0].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSearchDao-->GetAllKey: " + sql.ToString() + ex.Message);
            }
            return keyList;
        }
        #endregion

        #region 根據parameterName獲取ParameterCode
        public string GetParameterCode(string parameterName)
        {
            string result = string.Empty;
            ///獲取對應的數據
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("select rowid,parametertype,parameterproperty,parametercode,parametername from t_parametersrc where 1=1 and parametername='{0}'", parameterName);
                DataTable _dt = _accessMySql.getDataTable(strSql.ToString());
                if (_dt.Rows.Count > 0)
                {
                    result = _dt.Rows[0]["parametercode"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSearchDao.GetParameterCode-->" + ex.Message + strSql.ToString());
            }
            
            return result;
        } 
        #endregion
    }

}

