using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class PageErrorLogDao
    {
        IDBAccess _accessMySql;
        string connStr = string.Empty;
        public PageErrorLogDao(string connectionstring)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }

        #region 獲取參數
        /// <summary>
        /// 獲取參數
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public List<Parametersrc> QueryPara(Parametersrc para)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(" SELECT  parameterName,parameterCode from t_parametersrc WHERE parameterType='{0}';", para.ParameterType);
                return _accessMySql.getDataTableForObj<Parametersrc>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PageErrorLogDao.QueryPara-->" + ex.Message + ex);
            }
        } 
        #endregion

        #region 獲取數據
        /// <summary>
        /// 獲取數據
        /// </summary>
        /// <param name="sqltText">執行語句</param>
        /// <returns>返回數據表</returns>
        public List<PageErrorLogQuery> GetData(PageErrorLogQuery query, out int totalCount)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                StringBuilder sqlCondi = new StringBuilder();
                StringBuilder sqlCount = new StringBuilder();
                totalCount = 0;
                ///獲得分頁對應數據

                ///獲得數據sql 
                sql.AppendFormat("SELECT rowID,error_page_url,parameterName as errorName,create_date, create_ip ");
                ///條件限制aql
                sqlCondi.Append(" from page_error_log  as pel");
                sqlCondi.Append(" LEFT JOIN (SELECT parameterCode,parameterName from  t_parametersrc where parameterType='page_error_type') as par ");
                sqlCondi.Append(" on pel.error_type=par.parameterCode  where 1=1");
                ///查詢總數sql
                sqlCount.Append(" select count(pel.rowID) as totalCount ");
                #region 查詢條件限制
                if (query.searchType != 0)
                {
                    ///增加查詢類型限制
                    sqlCondi.AppendFormat(" AND error_type={0} ", query.searchType);
                }
                ///關鍵字查詢限制
                if (!string.IsNullOrEmpty(query.searchKey))
                {
                    sqlCondi.AppendFormat(" AND (rowID like N'%{0}%' ", query.searchKey);
                    sqlCondi.AppendFormat(" OR error_page_url like N'%{0}%' ", query.searchKey);
                    sqlCondi.AppendFormat(" OR create_ip like N'%{0}%' )", query.searchKey);
                }
                ///增加查詢時間限制
                if (!string.IsNullOrEmpty(query.startT) && !string.IsNullOrEmpty(query.endT))
                {
                    sqlCondi.AppendFormat(" AND create_date BETWEEN '{0}' AND '{1}' ", query.startT, query.endT);
                }
                #endregion
                ///是否分頁
                if (query.IsPage)
                {
                    sqlCount.Append(sqlCondi.ToString());
                    DataTable _dt = _accessMySql.getDataTable(sqlCount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        //得到滿足條件的總行數
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                }
                ///得到應分頁的數據
                sqlCondi.AppendFormat(" ORDER BY rowID ASC LIMIT {0},{1} ;", query.Start, query.Limit);
               
                return _accessMySql.getDataTableForObj<PageErrorLogQuery>( sql.Append(sqlCondi).ToString());
               
            }
            catch (Exception ex)
            {
                throw new Exception("PageErrorLogDao.GetData-->" + ex.Message + ex);
            }
        }
        #endregion

    }
}
