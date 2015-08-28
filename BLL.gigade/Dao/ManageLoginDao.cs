using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
 
namespace BLL.gigade.Dao
{
    public class ManageLoginDao
    {
        private IDBAccess _access;
        private string connString;
        public ManageLoginDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connString = connectionString;
        }
        public List<ManageLoginQuery> GetManageLoginList(ManageLoginQuery query, out int totalCount)
        {
            List<ManageLoginQuery> list = new List<ManageLoginQuery>();
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbSqlWhere = new StringBuilder();
            StringBuilder sbSqlFrom = new StringBuilder();
            try
            {

                sbSql.Append("select ml.login_id as loginID,ml.user_id,ml.login_ipfrom,FROM_UNIXTIME(ml.login_createdate) as login_createtime,mu.user_username as user_name ");
                sbSqlFrom.Append("  from manage_login ml ");
                sbSqlFrom.Append("  left join manage_user mu on ml.user_id = mu.user_id ");

                if (query.login_id != 0)
                {
                    sbSqlWhere.AppendFormat(" and ml.login_id = '{0}' ", query.login_id);
                }
                if (query.login_start != 0)
                {
                    sbSqlWhere.AppendFormat(" and ml.login_createdate >= '{0}' ", query.login_start);
                }
                if (query.login_end != 0)
                {
                    sbSqlWhere.AppendFormat(" and ml.login_createdate <= '{0}' ", query.login_end);
                }
                if (!string.IsNullOrEmpty(query.user_name))
                {
                    sbSqlWhere.AppendFormat(" and  mu.user_username like '%{0}%' ", query.user_name);
                }
                if (!string.IsNullOrEmpty(query.login_ipfrom))
                {
                    sbSqlWhere.AppendFormat(" and ml.login_ipfrom like '%{0}%'", query.login_ipfrom);
                }
                if (sbSqlWhere.Length > 0)
                {
                    sbSqlFrom.Append(" where " + sbSqlWhere.ToString().TrimStart().Remove(0, 3));
                }
                totalCount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable("select count(ml.login_id) as totalCount " + sbSqlFrom.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }
                    sbSqlFrom.AppendFormat(" order by ml.login_id desc limit {0},{1} ", query.Start, query.Limit);
                }
                sbSql.Append(sbSqlFrom.ToString());
                return list = _access.getDataTableForObj<ManageLoginQuery>(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ManageLoginDao-->GetManageLoginList-->" + ex.Message + sbSql.ToString(), ex);
            }

        }
    }
}
