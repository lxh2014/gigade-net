using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Dao
{
    public class EdmContentDao : IEdmContentImplDao
    {
        private IDBAccess _accessMySql;
        private string connStr;
        public EdmContentDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }

        public List<EdmContentQuery> GetEdmContentList(EdmContentQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlLimit = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            totalCount = 0;
            try
            {
                store.Replace4MySQL();
                //sqlCount.AppendFormat("select count(*) as totalCount  FROM	edm_content edm left join epaper_content ec on edm.info_epaper_id=ec.epaper_id ");
                //sql.AppendFormat("SELECT	edm.content_id,edm.group_id,edm.content_status,edm.content_start,	edm.content_click,edm.content_person,edm.content_send_success,edm.content_send_failed,edm.content_from_name,edm.content_from_email,edm.content_reply_email,edm.content_body,edm.content_priority,edm.content_title,edm.content_createdate,edm.content_updatedate,ec.epaper_title,edm.info_epaper_id  FROM	edm_content edm left join epaper_content ec on edm.info_epaper_id=ec.epaper_id   ");
                //sqlWhere.AppendFormat(" where 1=1 ");
                sqlCount.AppendFormat("select count(edm.content_id) as totalCount ");
                sql.AppendFormat("SELECT	edm.content_id,edm.group_id,edm.content_status,edm.content_start,	edm.content_click,edm.content_person,edm.content_send_success,edm.content_send_failed,edm.content_from_name,edm.content_from_email,edm.content_reply_email,edm.content_body,edm.content_priority,edm.content_title,edm.content_createdate,edm.content_updatedate,ec.epaper_title,edm.info_epaper_id      ");
                sqlWhere.Append(" FROM	edm_content edm ");
                sqlWhere.Append(" left join epaper_content ec on edm.info_epaper_id=ec.epaper_id ");
                sqlWhere.AppendFormat(" where 1=1 ");
                if (store.search_text != "")
                {
                    sqlWhere.AppendFormat(" and edm.content_title like N'%{0}%'", store.search_text);
                }
                if (store.searchStatus != "0")
                {
                    sqlWhere.AppendFormat(" and edm.content_status= '{0}'", store.searchStatus);
                }
                if (store.s_content_start != DateTime.MinValue && store.s_content_end != DateTime.MinValue)
                {
                    sqlWhere.AppendFormat(" and edm.content_start between '{0}' and '{1}'", CommonFunction.GetPHPTime(store.s_content_start.ToString()), CommonFunction.GetPHPTime(store.s_content_end.ToString()));
                }
                if (store.IsPage)
                {
                    sqlCount.Append(sqlWhere.ToString());
                    DataTable _dt = _accessMySql.getDataTable(sqlCount.ToString() );
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                }
                sqlWhere.AppendFormat(" ORDER BY content_id DESC ");
                sqlLimit.AppendFormat("  limit {0},{1};", store.Start, store.Limit);
                sql.AppendFormat(sqlWhere.ToString());
                sql.AppendFormat(sqlLimit.ToString());
                return _accessMySql.getDataTableForObj<EdmContentQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentDao-->GetEdmContentList-->" + ex.Message + sql.ToString()+sqlCount.ToString(), ex);
            }
        }
        public List<EdmContentQuery> GetEdmContent()
        {
            StringBuilder sql = new StringBuilder("");
            try
            {
                sql.Append("select content_id,group_id,content_status,content_email_id,content_start,content_end,content_range,content_single_count,content_click,content_person,content_send_success,content_send_failed,content_from_name,content_from_email,content_reply_email,content_priority,content_title,info_epaper_id,content_body,content_createdate,content_updatedate from edm_content  ");
                return _accessMySql.getDataTableForObj<EdmContentQuery>(sql.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentDao-->GetEdmContent-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public int DeleteEdm(int contentId)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates=0; delete from edm_content where content_id={0}  and content_status <=3;set sql_safe_updates=1;", contentId);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentDao-->DeleteEdm-->" + ex.Message + sql.ToString(), ex);
            }
        }

        #region 得到加1后的serial_value
        /// <summary>
        /// 得到加1后的serial_value
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public int GetSerialValue()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select serial_value from serial where serial_id=52;");
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
        public string UpdateSerialVal(int serialValue)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("update serial set serial_value={0}  where serial_id=52;", serialValue);
            return sql.ToString();
        }
        #endregion

        public int EdmContentSave(EdmContentQuery store)
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            StringBuilder sql = new StringBuilder();
            int re = 0;
            if (store.content_id == 0)//新增
            {
                try
                {
                    if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                    {
                        store.serialvalue = GetSerialValue();
                        mySqlConn.Open();
                        mySqlCmd.Connection = mySqlConn;
                        mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                        mySqlCmd.CommandType = System.Data.CommandType.Text;
                        mySqlCmd.CommandText = UpdateSerialVal(store.serialvalue);
                        mySqlCmd.CommandText += InsertEdmContent(store);
                        re = mySqlCmd.ExecuteNonQuery();
                        mySqlCmd.Transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    mySqlCmd.Transaction.Rollback();
                    throw new Exception("EdmContentDao-->EdmContentSave-->" + mySqlCmd.ToString() + ex.Message, ex);
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
            else    //編輯
            {
                try
                {
                    store.Replace4MySQL();
                    sql.AppendFormat("update edm_content set group_id={0},content_status={1},content_start={2},", store.group_id, store.content_status, store.content_start);
                    sql.AppendFormat("content_from_name='{0}',content_from_email='{1}',content_reply_email='{2}',content_priority={3},info_epaper_id='{4}',", store.content_from_name, store.content_from_email, store.content_reply_email, store.content_priority, store.info_epaper_id);
                    sql.AppendFormat("content_title='{0}',content_body='{1}',content_updatedate={2}  where  content_id={3};", store.content_title, store.content_body, CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), store.content_id);
                    return _accessMySql.execCommand(sql.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception("EdmContentDao-->EdmContentSave-->" + sql.ToString() + ex.Message, ex);
                }
            }
        }

        public string InsertEdmContent(EdmContentQuery store)
        {
            store.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("insert into edm_content (content_id,group_id,content_status,content_email_id,content_start, ");
            sql.AppendFormat("content_end,content_range,content_single_count,content_click,content_person,");
            sql.AppendFormat("content_from_name,content_from_email,content_reply_email,content_priority,content_title,");
            sql.AppendFormat("content_body,content_createdate,content_updatedate,info_epaper_id)");
            sql.AppendFormat(" values({0},{1},{2},{3},{4},", store.serialvalue, store.group_id, store.content_status, store.content_email_id, store.content_start);
            sql.AppendFormat("{0},{1},{2},{3},{4},", store.content_end, store.content_range, store.content_single_count, store.content_click, store.content_person);
            sql.AppendFormat("'{0}','{1}','{2}',{3},'{4}',", store.content_from_name, store.content_from_email, store.content_reply_email, store.content_priority, store.content_title);
            sql.AppendFormat("'{0}',{1},{2},'{3}');", store.content_body, CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), store.info_epaper_id);
            return sql.ToString();
        }
        /// <summary>
        /// 獲取收件者名單
        /// </summary>
        /// <returns></returns>
        public List<EdmContentQuery> GetEdmGroup()
        {
            string strSql = string.Empty;
            try
            {
                strSql = string.Format("SELECT group_id,CONCAT(group_name,'(',group_total_email,')') as group_name from edm_group WHERE group_name Not                 LIKE '%錯誤%' AND group_name Not LIKE '%test%' AND group_name Not LIKE '%測試%' AND group_name Not LIKE '%純淨契作%' AND group_name Not                 LIKE '%舊%' AND group_name Not LIKE '%STOP%' ;");
                return _accessMySql.getDataTableForObj<EdmContentQuery>(strSql);
            }
            catch (Exception ex)
            {

                throw new Exception("EdmContentDao-->GetEdmGroup-->" + strSql.ToString() + ex.Message, ex);
            }
        }
        public EdmContentQuery GetEdmContentById(EdmContentQuery query)
        {
            string strSql = string.Empty;
            try
            {
                strSql = string.Format("select content_id,group_id,content_status,content_email_id,content_start ,content_end,content_range,content_single_count,content_click,content_person,content_send_success,content_send_failed,content_from_name,content_from_email,content_reply_email,content_priority,content_title,content_body,content_createdate,content_updatedate,info_epaper_id from edm_content where content_id={0}", query.content_id);
                return _accessMySql.getSinggleObj<EdmContentQuery>(strSql);
            }
            catch (Exception ex)
            {

                throw new Exception("EdmContentDao-->GetEdmContentById-->" + strSql.ToString() + ex.Message, ex);
            }
        }

        public int CancelEdm(string mail)
        {

            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append(" set sql_safe_updates = 0;update edm_group_email ege,edm_email ee set ege.email_status=2 ");
                strSql.AppendFormat(" where ee.email_address='{0}' and ege.email_id=ee.email_id;set sql_safe_updates = 1;", mail);
                return _accessMySql.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("EdmContentDao-->CancelEdm-->" + strSql.ToString() + ex.Message, ex);
            }
        }

        public string EditStatus(EdmContentQuery query)
        {            
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(@"update edm_content set content_status={0} WHERE content_id={1}", query.content_status, query.content_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                
                throw new Exception("EdmContentDao-->EditStatus-->" + strSql.ToString() + ex.Message, ex);;
            }
        }
        public DataTable GetAllTestEmail()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT et.email_id,et.test_username,ee.email_address FROM edm_email ee,edm_test et
			WHERE et.test_status = 1 AND et.email_id = ee.email_id");
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("EdmContentDao-->GetAllTestEmail-->" + sql.ToString() + ex.Message, ex); ;
            }
        }
        public DataTable GetTestEmailById(uint content_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT	group_id,
					content_status,
					content_email_id,
					content_start,
					content_end,
					content_range,
					content_single_count,
					content_click,
					content_person,
					content_send_success,
					content_send_failed,
					content_from_name,
					content_from_email,
					content_reply_email,
					content_priority,
					content_title,
					content_body,
					content_createdate,
					content_updatedate
				FROM edm_content 
				WHERE content_id = {0}", content_id);
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentDao-->GetTestEmailById-->" + sql.ToString() + ex.Message, ex); ;
            }
        }

    }
}
