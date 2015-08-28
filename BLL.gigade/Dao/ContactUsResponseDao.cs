using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using MySql.Data.MySqlClient;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
    public class ContactUsResponseDao : IContactUsResponseImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public ContactUsResponseDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }

        public System.Data.DataTable GetRecordList(Model.ContactUsResponse query, out int totalcount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            sqlcount.AppendFormat(@" SELECT count(*) as counts FROM contact_us_response cur 
                                     LEFT JOIN manage_user mu on cur.user_id=mu.user_id 
                                     LEFT JOIN contact_us_question cuq on cuq.question_id =cur.question_id
                                    where cur.question_id='{0}'
                                    order by cur.response_id ", query.question_id);
            sql.AppendFormat(@" SELECT cur.response_id,cur.response_content,mu.user_username,concat(DATE(FROM_UNIXTIME(response_createdate)),' ',TIME(FROM_UNIXTIME(response_createdate))) as response_createdate,cuq.question_content FROM   contact_us_response cur 
                                        LEFT JOIN manage_user mu on cur.user_id=mu.user_id 
                                        LEFT JOIN contact_us_question cuq on cuq.question_id =cur.question_id
                                        where cur.question_id='{0}'
                                        order by cur.response_id ", query.question_id);
            totalcount = 0;
            try
            {
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalcount = Convert.ToInt32(_dt.Rows[0]["counts"]);
                    }
                    sql.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsResponseDao.GetRecordList-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public System.Data.DataTable GetRecordList(Model.ContactUsResponse query, string startDate, string endDate, string reply_user, out int totalcount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder condition = new StringBuilder();
            sqlcount.AppendFormat(@" SELECT count(*) as counts FROM contact_us_response cur 
                                     LEFT JOIN manage_user mu on cur.user_id=mu.user_id 
                                     LEFT JOIN contact_us_question cuq on cuq.question_id =cur.question_id
                                    where cur.question_id='{0}'", query.question_id);
            sql.AppendFormat(@" SELECT cur.response_id,cur.response_content,cur.response_type,mu.user_username,FROM_UNIXTIME(cur.response_createdate) as response_createdate,cuq.question_content 
FROM   contact_us_response cur 
                                        LEFT JOIN manage_user mu on cur.user_id=mu.user_id 
                                        LEFT JOIN contact_us_question cuq on cuq.question_id =cur.question_id
                                        where cur.question_id='{0}'
                                       ", query.question_id);
            totalcount = 0;
            try
            {
                if (!string.IsNullOrEmpty(startDate))
                {
                    condition.AppendFormat(" and FROM_UNIXTIME(cur.response_createdate) >='{0}' ", startDate);
                }
                if (!string.IsNullOrEmpty(endDate))
                {
                    condition.AppendFormat(" and FROM_UNIXTIME(cur.response_createdate) <='{0}' ", endDate);
                }
                if (!string.IsNullOrEmpty(reply_user))
                {
                    condition.AppendFormat(" and mu.user_username like '%{0}%'", reply_user);
                }
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString() + condition.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalcount = Convert.ToInt32(_dt.Rows[0]["counts"]);
                    }
                    sql.Append(condition);
                    sql.AppendFormat("  order by cur.response_id  limit {0},{1}", query.Start, query.Limit);
                }
                return _access.getDataTable(sql.ToString() );
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsResponseDao.GetRecordList-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public int Insert(string sql, Model.ContactUsResponse query)
        {
            int result = 0;
            StringBuilder str = new StringBuilder();
            str.AppendFormat(sql.ToString());
            if (query.response_id != 0)
            {
                str.AppendFormat(@"insert into contact_us_response(response_id,response_type,question_id,user_id,response_content,response_createdate,response_ipfrom)
            values('{0}','{1}','{2}','{3}','{4}','{5}','{6}');"
                    , query.response_id,query.response_type, query.question_id, query.user_id, query.response_content, query.response_createdate, query.response_ipfrom);
            }
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                mySqlCmd.CommandText = str.ToString();
                result = mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
                return result;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("ContactUsReponseDao.Insert-->" + ex.Message + str.ToString(), ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
        }


        public int GetMaxResponseId()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" SELECT max(response_id) FROM contact_us_response ");
                DataTable _dt = _access.getDataTable(sql.ToString());
                return Convert.ToInt32(_dt.Rows[0][0]) + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("ContactUsQuestionDao.GetMaxQuestionId-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
