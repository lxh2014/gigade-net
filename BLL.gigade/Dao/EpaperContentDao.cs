using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using MySql.Data.MySqlClient;
using System.Web.Configuration.Common;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
    public class EpaperContentDao : IEpaperContentImplDao
    {
        private IDBAccess _accessMySql;
        private string connStr;
        public EpaperContentDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        #region 新增電子報時查詢最新四條活動頁面+List<EpaperContent> GetEpaperContentLimit()
        /// <summary>
        /// 新增電子報時查詢最新四條活動頁面
        /// </summary>
        /// <returns></returns>
        public List<EpaperContent> GetEpaperContentLimit()
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("select epaper_id,epaper_title,epaper_content from epaper_content order by   epaper_createdate DESC LIMIT 0,4;");
                return _accessMySql.getDataTableForObj<EpaperContent>(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("EpaperContentDao-->GetEpaperContentLimit-->" + ex.Message, ex);
            }
        }
        #endregion
        public List<EpaperContentQuery> GetEpaperContentList(EpaperContentQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlLimit = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            totalCount = 0;
            try
            {
                //sqlCount.AppendFormat("select count(1) as totalCount  from  epaper_content ec left join manage_user mu on ec.user_id=mu.user_id ");
                //sql.AppendFormat("SELECT	ec.epaper_id,mu.user_username as user_username,ec.user_id,ec.epaper_title,ec.epaper_short_title,ec.epaper_content,ec.epaper_sort,ec.epaper_status,ec.epaper_size,ec.epaper_show_start,ec.epaper_show_end,ec.fb_description,ec.epaper_createdate,ec.epaper_updatedate,ec.epaper_ipfrom,ec.type from  epaper_content ec left join manage_user mu on ec.user_id=mu.user_id ");
                sqlCount.AppendFormat(" select count(ec.epaper_id) as totalCount ");
                sql.AppendFormat("SELECT	ec.epaper_id,mu.user_username as user_username,ec.user_id,ec.epaper_title,ec.epaper_short_title,ec.epaper_content,ec.epaper_sort,ec.epaper_status,ec.epaper_size,ec.epaper_show_start,ec.epaper_show_end,ec.fb_description,ec.epaper_createdate,ec.epaper_updatedate,ec.epaper_ipfrom,ec.type ");
                sqlWhere.Append(" from  epaper_content ec ");
                sqlWhere.Append(" left join manage_user mu on ec.user_id=mu.user_id ");
                sqlWhere.AppendFormat(" where 1=1 ");
                //查詢條件
                if (query.searchCon == "1")
                {
                    sqlWhere.AppendFormat(" and ec.epaper_title like '%{0}%' ", query.search_text);
                }
                else if (query.searchCon == "2")
                {
                    sqlWhere.AppendFormat(" and ec.epaper_short_title like '%{0}%' ", query.search_text);
                }
                else if (query.searchCon == "3")
                {
                    sqlWhere.AppendFormat(" and mu.user_username like '%{0}%' ", query.search_text);
                }
                else
                {
                    sqlWhere.AppendFormat(" and ( ec.epaper_title like '%{0}%' or ec.epaper_short_title like '%{0}%' or mu.user_username like '%{0}%' ) ", query.search_text);
                }
                //日期條件

                if (query.dateCon == "1")
                {
                    if (!query.epaperShowStart.Equals(DateTime.MinValue))
                    {
                        sqlWhere.AppendFormat(" and ec.epaper_show_start > '{0}' ", CommonFunction.GetPHPTime(query.epaperShowStart.ToString()));
                    }
                    if (!query.epaperShowEnd.Equals(DateTime.MinValue))
                    {
                        sqlWhere.AppendFormat(" and ec.epaper_show_start<'{0}' ",CommonFunction.GetPHPTime(query.epaperShowEnd.ToString()));
                    }
                }
                else if (query.dateCon == "2")
                {
                    if (!query.epaperShowStart.Equals(DateTime.MinValue))
                    {
                        sqlWhere.AppendFormat(" and ec.epaper_show_end > '{0}' ", CommonFunction.GetPHPTime(query.epaperShowStart.ToString()));
                    }
                    if (!query.epaperShowEnd.Equals(DateTime.MinValue))
                    {
                        sqlWhere.AppendFormat(" and ec.epaper_show_end<'{0}' ", CommonFunction.GetPHPTime(query.epaperShowEnd.ToString()));
                    }
                }
                else
                {
                    //所有日期時不進行日期判斷；
                    //sqlWhere.AppendFormat(" and( ( ec.epaper_show_start > '{0}' and ec.epaper_show_start<'{1}') or ", CommonFunction.GetPHPTime(query.epaperShowStart.ToString()), CommonFunction.GetPHPTime(query.epaperShowEnd.ToString()));
                    //sqlWhere.AppendFormat("  ec.epaper_show_end > '{0}' and ec.epaper_show_end<'{1}' ) ", CommonFunction.GetPHPTime(query.epaperShowStart.ToString()), CommonFunction.GetPHPTime(query.epaperShowEnd.ToString()));
                }
                if (query.epaperStatus != "" && query.epaperStatus != "-1")
                {
                    sqlWhere.AppendFormat(" and ec.epaper_status = '{0}' ", query.epaperStatus);
                }
                if (query.epaper_size != "" && query.epaper_size != "0")
                {
                    if (query.epaper_size == "所有尺寸")
                    {
                        sqlWhere.AppendFormat(" and ec.epaper_size = '725px' or ec.epaper_size = '900px '");
                    }
                    else
                    {
                        sqlWhere.AppendFormat(" and ec.epaper_size = '{0}' ", query.epaper_size);
                    }
                }
                if (query.IsPage)
                {
                    sqlCount.Append(sqlWhere.ToString());
                    DataTable _dt = _accessMySql.getDataTable(sqlCount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                }
                sqlWhere.AppendFormat(" order by ec.epaper_show_start desc, ec.epaper_sort desc, ec.epaper_id desc ");
                sql.AppendFormat(sqlWhere.ToString());
                sqlLimit.AppendFormat("  limit {0},{1};", query.Start, query.Limit);
                sql.AppendFormat(sqlLimit.ToString());
                return _accessMySql.getDataTableForObj<EpaperContentQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EpaperContentDao-->GetEpaperContentList-->" + ex.Message + sql.ToString() + sqlWhere.ToString(), ex);
            }
        }
        public List<EpaperContentQuery> GetEpaperContent()
        {
            StringBuilder sql = new StringBuilder();

            try
            {
                sql.AppendFormat("SELECT *  FROM epaper_content ORDER BY epaper_id DESC ");
                return _accessMySql.getDataTableForObj<EpaperContentQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EpaperContentDao-->GetEpaperContent-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public EpaperContentQuery GetEpaperContentById(EpaperContentQuery query)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("select * from epaper_content where epaper_id='{0}'", query.epaper_id);
            try
            {
                return _accessMySql.getSinggleObj<EpaperContentQuery>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EpaperContentDao-->GetEpaperContentById-->" + ex.Message, ex);
            }
        }
        public List<EpaperLogQuery> GetEpaperLogList(EpaperLogQuery query, out int totalCount)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder strWhere = new StringBuilder();
            try
            {
                strSql.AppendFormat("select el.*,mu.user_username as user_name from epaper_log el left join manage_user mu on el.user_id=mu.user_id  ");
                sqlCount.AppendFormat("select count( *) as  totalCount from epaper_log el left join manage_user mu on el.user_id=mu.user_id ");
                strWhere.AppendFormat(" where 1=1 and  el.epaper_id={0}", query.epaper_id);
                totalCount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _accessMySql.getDataTable(sqlCount.ToString() + strWhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                }
                strSql.AppendFormat(strWhere.ToString());
                strSql.AppendFormat(" limit {0},{1};", query.Start, query.Limit);
                return _accessMySql.getDataTableForObj<EpaperLogQuery>(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("EpaperContentDao-->GetEpaperLogList-->" + ex.Message, ex);
            }
        }

        //('SERIAL_ID_EPAPER_CONTENT',		24);	// 活動頁面流水號
        //('SERIAL_ID_EPAPER_LOG',			25);	// 活動頁面狀態流水號
        #region 得到加1后的serial_value
        public int GetSerialValue(int serialId)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select serial_value from serial where serial_id={0};", serialId);
                DataTable _dt = _accessMySql.getDataTable(sql.ToString());
                return Convert.ToInt32(_dt.Rows[0]["serial_value"]) + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentDao-->GetSerialValue-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion
        #region 更新serial_value
        public string UpdateSerialVal(int serialValue, int serialId)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("update serial set serial_value={0}  where serial_id={1};", serialValue, serialId);
            return sql.ToString();
        }
        #endregion
        public int SaveEpaperContent(EpaperContentQuery query)
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            int re = 0;
            try
            {
                #region 新增
                if (query.epaper_id == 0)
                {
                    if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                    {
                        query.epaper_id = Convert.ToUInt32(GetSerialValue(24));//epapercontent
                        query.log_id = GetSerialValue(25);//log
                        mySqlConn.Open();
                        mySqlCmd.Connection = mySqlConn;
                        mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                        mySqlCmd.CommandType = System.Data.CommandType.Text;
                        mySqlCmd.CommandText = UpdateSerialVal((Convert.ToInt32(query.epaper_id)), 24);
                        mySqlCmd.CommandText += InsertEpaperContent(query);
                        mySqlCmd.CommandText += UpdateSerialVal(query.log_id, 25);
                        mySqlCmd.CommandText += AddEpaperLog(query);
                        re = mySqlCmd.ExecuteNonQuery();
                        mySqlCmd.Transaction.Commit();
                    }
                }

                #endregion

                #region 編輯
                else
                {
                    if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                    {
                        //query.epaper_id = Convert.ToUInt32(GetSerialValue(24));//epapercontent
                        query.log_id = GetSerialValue(25);//log
                        mySqlConn.Open();
                        mySqlCmd.Connection = mySqlConn;
                        mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                        mySqlCmd.CommandType = System.Data.CommandType.Text;
                        mySqlCmd.CommandText = UpdateEpaperContent(query);
                        mySqlCmd.CommandText += UpdateSerialVal(query.log_id, 25);
                        mySqlCmd.CommandText += AddEpaperLog(query);
                        re = mySqlCmd.ExecuteNonQuery();
                        mySqlCmd.Transaction.Commit();
                    }
                }
                #endregion
            }

            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("NewsContentDao-->SaveEpaperContent-->" + mySqlCmd.ToString() + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            return re;
        }
        public string InsertEpaperContent(EpaperContentQuery store)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into epaper_content (epaper_id,user_id,epaper_title,epaper_short_title,epaper_size,");
            sql.Append("epaper_content,epaper_sort,epaper_status,");
            sql.Append("epaper_show_start,epaper_show_end,");
            sql.Append("fb_description,epaper_ipfrom,type,");
            sql.Append("epaper_createdate,epaper_updatedate) values(");
            sql.AppendFormat("{0},{1},'{2}','{3}','{4}', ", store.epaper_id, store.user_id, store.epaper_title, store.epaper_short_title, store.epaper_size);
            sql.AppendFormat("'{0}',{1},{2},", store.epaper_content, store.epaper_sort, store.epaper_status);
            sql.AppendFormat("{0},{1},", CommonFunction.GetPHPTime(store.epaperShowStart.ToString()), CommonFunction.GetPHPTime(store.epaperShowEnd.ToString()));
            sql.AppendFormat("'{0}','{1}',{2},", store.fb_description, store.epaper_ipfrom, store.type);
            sql.AppendFormat("{0},{1});", CommonFunction.GetPHPTime(store.epaperCreateDate.ToString()), CommonFunction.GetPHPTime(store.epaperUpdateDate.ToString()));
            return sql.ToString();
        }

        public string UpdateEpaperContent(EpaperContentQuery store)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("set sql_safe_updates=0;update epaper_content set epaper_title='{0}',epaper_short_title='{1}',epaper_content='{2}',", store.epaper_title, store.epaper_short_title, store.epaper_content);
            sql.AppendFormat("epaper_sort={0},epaper_status={1},epaper_size='{2}',", store.epaper_sort, store.epaper_status, store.epaper_size);
            sql.AppendFormat("epaper_show_start={0},epaper_show_end='{1}',fb_description='{2}',", CommonFunction.GetPHPTime(store.epaperShowStart.ToString()), CommonFunction.GetPHPTime(store.epaperShowEnd.ToString()), store.fb_description);
            sql.AppendFormat("epaper_updatedate={0},epaper_ipfrom='{1}',type={2} ", CommonFunction.GetPHPTime(store.epaperUpdateDate.ToString()), store.epaper_ipfrom, store.type);
            sql.AppendFormat(" where epaper_id={0};set sql_safe_updates=1;", store.epaper_id);
            return sql.ToString();
        }

        public string AddEpaperLog(EpaperContentQuery store)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("insert into epaper_log (log_id,epaper_id,user_id,log_description,log_ipfrom,log_createdate)");
            sql.AppendFormat("values({0},{1},{2},'{3}','{4}',{5});", store.log_id, store.epaper_id, store.user_id, store.log_description, store.epaper_ipfrom, CommonFunction.GetPHPTime(DateTime.Now.ToString()));
            return sql.ToString();
        }
    }
}
