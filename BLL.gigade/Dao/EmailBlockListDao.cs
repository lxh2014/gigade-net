using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Dao
{
    public class EmailBlockListDao
    {
        private IDBAccess _access;
        public EmailBlockListDao(string sqlConnectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, sqlConnectionString);
        }
        public DataTable GetEmailBlockList(EmailBlockListQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder count = new StringBuilder();
            int totalCount = 0;
           DataTable store = new DataTable();
            try
            {
                sql.AppendFormat(@"SELECT email_address,block_reason,block_createdate,(SELECT user_username FROM manage_user WHERE manage_user.user_id=email_block_list.block_create_userid ) as user_name from email_block_list where 1=1 ");
                count.AppendFormat(@"SELECT count(email_address) as totalCount from email_block_list where 1=1 ");
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable(count.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sql.AppendFormat("  limit {0},{1} ;", query.Start, query.Limit);
                }

                store = _access.getDataTable(sql.ToString());
                return store;
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Number.ToString() + ":EmailBlockListDao-->GetEmailBlockList" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("EmailBlockListDao-->GetEmailBlockList-->" + ex.Message, ex);
            }
        }

        public  DataTable GetModel(string email)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT email_address,block_reason,block_create_userid,block_update_userid,block_createdate ,block_updatedate from email_block_list WHERE email_address ='{0}'", email);
                DataTable dt = _access.getDataTable(sql.ToString());
                return dt;
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Number.ToString() + ":EmailBlockListDao-->GetModel" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("EmailBlockListDao-->GetModel-->" + ex.Message, ex);
            }
        }

        public int Add(EmailBlockListQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"INSERT INTO email_block_list(email_address,block_reason,block_create_userid,block_update_userid) VALUES('{0}','{1}','{2}','{3}');", query.email_address, query.block_reason, query.block_create_userid, query.block_update_userid);
                return _access.execCommand(sql.ToString());
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Number.ToString() + ":EmailBlockListDao-->Add" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("EmailBlockListDao-->Add-->" + ex.Message, ex);
            }
        }

        public int Update(EmailBlockListQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"UPDATE email_block_list SET block_reason='{0}' ,block_update_userid={1} WHERE email_address='{2}'", query.block_reason, query.block_update_userid, query.email_address);
                return _access.execCommand(sql.ToString());
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Number.ToString() + ":EmailBlockListDao-->Update" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("EmailBlockListDao-->Update-->" + ex.Message, ex);
            }
        }
        public string Delete(string email)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"DELETE from email_block_list WHERE email_address='{0}'", email);
                return sql.ToString();
            }          
            catch (Exception ex)
            {
                throw new Exception("EmailBlockListDao-->Delete-->" + ex.Message, ex);
            }
        }
    }
}
