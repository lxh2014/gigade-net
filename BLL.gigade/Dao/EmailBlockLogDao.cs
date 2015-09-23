using BLL.gigade.Model.Query;
using DBAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class EmailBlockLogDao
    {
        private IDBAccess _access;
        public EmailBlockLogDao(string sqlConnectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, sqlConnectionString);
        }
        public string AddUnBlockLog(EmailBlockLogQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"INSERT INTO email_block_log (email_address,block_start,block_end,block_reason,unblock_reason,block_create_userid,unblock_create_userid)
VALUES('{0}','{1}','{2}','{3}','{4}',{5},{6})", query.email_address, query.block_start, query.block_end, query.block_reason, query.unblock_reason, query.block_create_userid, query.unblock_create_userid);
                return sql.ToString();
            }         
            catch (Exception ex)
            {
                throw new Exception("EmailBlockLogDao-->AddUnBlockLog-->" + ex.Message, ex);
            }
        
        }
    }
}
