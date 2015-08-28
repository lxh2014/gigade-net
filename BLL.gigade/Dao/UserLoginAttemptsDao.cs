using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using BLL.gigade.Common;
namespace BLL.gigade.Dao
{
    public class UserLoginAttemptsDao
    {
        private IDBAccess _access;
        public UserLoginAttemptsDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
        }
        public DataTable GetUserLoginAttemptsList(UserLoginAttempts ula, out int totalCount)
        {
            totalCount = 0;
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfield = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            try
            {
                sqlfield.AppendLine(@"SELECT ula.login_id,u.user_id,u.user_name,ula.login_mail,ula.login_ipfrom,FROM_UNIXTIME(ula.login_createdate) AS login_createdate,");
                sqlfield.AppendLine(@"login_type,pa.parameterName as slogin_type,COUNT(ula.login_mail) AS sumtotal ");
                sqlfrom.AppendLine(@" FROM user_login_attempts  ula");
                sqlfrom.AppendLine(@" LEFT JOIN users u ON ula.login_mail=u.user_email");
                sqlfrom.AppendLine(@" LEFT JOIN (SELECT parameterCode,parameterName  FROM t_parametersrc WHERE parameterType='user_login_type') pa ON ula.login_type=pa.parameterCode ");
                sqlwhere.AppendLine(@" WHERE 1=1 ");               
                if (!string.IsNullOrEmpty(ula.login_ipfrom))
                {
                    sqlwhere.AppendFormat(@" AND ula.login_ipfrom like '%{0}%' ", ula.login_ipfrom);
                }
                if (!string.IsNullOrEmpty(ula.login_mail))
                {
                    sqlwhere.AppendFormat(@" AND ula.login_mail like '%{0}%' ", ula.login_mail);
                }
                if (ula.slogin_createdate != 0)
                {
                    sqlwhere.AppendFormat(@" AND ula.login_createdate >='{0}' ", ula.slogin_createdate);
                }
                if (ula.elogin_createdate != 0)
                {
                    sqlwhere.AppendFormat(@" AND ula.login_createdate <='{0}' ", ula.elogin_createdate);
                }
                if (ula.login_type != 0)
                {
                    sqlwhere.AppendFormat(@" AND ula.login_type ='{0}' ", ula.login_type);
                }
                if (ula.ismail == 0)
                {
                    sqlwhere.AppendFormat(@" GROUP BY ula.login_mail,ula.login_ipfrom,ula.login_type  ");
                }
                else if (ula.ismail == 1)
                {
                    sqlwhere.AppendFormat(@" GROUP BY ula.login_mail,ula.login_type  ");
                }
                else if (ula.ismail == 2)
                {
                    sqlwhere.AppendFormat(@" GROUP BY ula.login_ipfrom,ula.login_type  ");
                }

                if (ula.sumtotal != 0)
                {
                    if (ula.ismail == 1 || ula.ismail == 0)
                    {
                        sqlwhere.AppendFormat(@" HAVING COUNT(ula.login_mail)>= {0} ", ula.sumtotal);
                    }
                    else if (ula.ismail == 2)
                    {
                        sqlwhere.AppendFormat(@" HAVING COUNT(ula.login_ipfrom)>= {0} ", ula.sumtotal);
                    }
                }
                sql.Append(sqlfield.ToString() + sqlfrom.ToString() + sqlwhere.ToString());
                sql.AppendFormat(@"  ORDER BY login_id DESC ");
                if (ula.IsPage)
                {
                    DataTable dt = _access.getDataTable("SELECT ula.login_id " + sqlfrom.ToString() + sqlwhere.ToString());
                    totalCount = dt.Rows.Count;
                    sql.AppendFormat(@" LIMIT {0},{1} ", ula.Start, ula.Limit);
                }

                return _access.getDataTable(sql.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("UserLoginAttemptsDao-->GetUserLoginAttemptsList" + ex.Message + sql.ToString(), ex);
            }
        }
        public int Insert(UserLoginAttempts ula)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"INSERT INTO user_login_attempts (login_mail,login_ipfrom,login_type,login_createdate) ");
                sql.AppendFormat(@"VALUES('{0}','{1}','{2}','{3}');", ula.login_mail, ula.login_ipfrom, ula.login_type, CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UserLoginAttemptsDao-->Insert" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
