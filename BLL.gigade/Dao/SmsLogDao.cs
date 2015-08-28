using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class SmsLogDao : ISmsLogImplDao
    {
        private IDBAccess _accessMySql;
        private string connStr;
        public SmsLogDao(string connectionstring)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }

        public List<SmsLogQuery> GetSmsLog(SmsLogQuery slog, out int totalCount)
        {
            slog.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqladd = new StringBuilder();
            totalCount = 0;
            try
            {
                sqladd.Append("SELECT id,sms_id,provider,success as sucess_status,code,created   ");
                sql.Append(" FROM sms_log sl WHERE 1=1 ");

                if (slog.sms_id != 0)
                {
                    sql.AppendFormat(" AND sl.sms_id='{0}'", slog.sms_id);
                }
                if (slog.provider != 0)
                {
                    sql.AppendFormat(" AND sl.provider='{0}'", slog.provider);
                }
                if (slog.success != -1)
                {
                    sql.AppendFormat(" AND sl.success='{0}'", slog.success);
                }
                if (slog.created != DateTime.MinValue)
                {
                    sql.AppendFormat(" AND sl.created>='{0}'", slog.created.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (slog.modified != DateTime.MinValue)
                {
                    sql.AppendFormat(" AND sl.created<='{0}'", slog.modified.ToString("yyyy-MM-dd 23:59:59"));
                }
                if (slog.IsPage)
                {
                    DataTable _dt = _accessMySql.getDataTable("SELECT count(id) as totalCount " + sql.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                }
                sql.AppendFormat(" limit {0},{1};", slog.Start, slog.Limit);
                return _accessMySql.getDataTableForObj<SmsLogQuery>(sqladd.Append(sql).ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SmsLogDao-->GetSmsLog-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
