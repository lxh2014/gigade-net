using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using System.Data;

namespace BLL.gigade.Dao
{
    public class SecretAccountSetDao
    {
        private IDBAccess _access;
        public SecretAccountSetDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
        }
        public DataTable GetSecretSetList(SecretAccountSetQuery sasq, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();

            StringBuilder sqlfrom = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            totalCount = 0;
            try
            {
                sasq.Replace4MySQL();

                sql.Append("SELECT sas.id,sas.user_id,sas.secret_pwd,sas.createdate,sas.updatedate, sas.`status`,sas.pwd_status, ");
                sql.Append("mu.user_email,mu.user_username,sas.user_login_attempts,sas.ipfrom ,sas.secret_limit,sas.secret_count ");

                sqlfrom.Append(" FROM secret_account_set sas ");
                sqlfrom.Append(" LEFT JOIN manage_user mu ON sas.user_id=mu.user_id  ");
                if (!string.IsNullOrEmpty(sasq.user_username))
                {
                    sqlwhere.AppendFormat(@" AND (mu.user_username like N'%{0}%' OR  mu.user_email like N'%{0}%' OR  sas.ipfrom like N'%{0}%' )", sasq.user_username);
                }
                if (sasq.user_id != 0)
                {
                    sqlwhere.AppendFormat(@" AND sas.user_id = '{0}'", sasq.user_id);
                }
                //判斷相同的用戶和IP不能重複添加
                if (!string.IsNullOrEmpty(sasq.ipfrom))
                {
                    sqlwhere.AppendFormat(@" AND sas.ipfrom = '{0}'", sasq.ipfrom);
                    if (sasq.id != 0)
                    {
                        sqlwhere.AppendFormat(@" AND sas.id != '{0}'", sasq.id);
                    }
                }
                else
                {
                    if (sasq.id != 0)
                    {
                        sqlwhere.AppendFormat(@" AND sas.id = '{0}'", sasq.id);
                    }
                }
                //  sqlwhere.Append(" ORDER BY sas.id DESC");
                if (sqlwhere.Length != 0)
                {
                    sqlfrom.Append(" WHERE " + sqlwhere.ToString().TrimStart().Remove(0, 3));
                }
                sqlfrom.Append(" ORDER BY sas.id DESC");
                if (sasq.IsPage)
                { 

                    DataTable dt = _access.getDataTable("SELECT  count(sas.id) as totalCount " + sqlfrom.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                    }
                    sqlfrom.AppendFormat(" LIMIT {0},{1} ;", sasq.Start, sasq.Limit);
                }
                sql.Append(sqlfrom.ToString());
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SecretAccountSetDao-->GetSecretSetList" + ex.Message + sql.ToString(), ex);
            }

        }
        public List<SecretAccountSet> GetSecretSetList(SecretAccountSet query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            try
            {
                sql.Append(@"SELECT sas.id,sas.user_id,sas.secret_pwd,sas.createdate,sas.updatedate, sas.`status`,sas.pwd_status,sas.user_login_attempts,sas.ipfrom,secret_limit,secret_count  ");

                sqlwhere.AppendLine(@" FROM secret_account_set sas ");

                sqlwhere.AppendLine(@" WHERE  user_login_attempts<5 ");

                if (query.user_id != 0)
                {
                    sqlwhere.AppendFormat(@" AND sas.user_id ='{0}'", query.user_id);
                }
                if (!string.IsNullOrEmpty(query.ipfrom))
                {
                    sqlwhere.AppendFormat(@" AND sas.ipfrom ='{0}'", query.ipfrom);
                }
                if (query.status != -1)
                {
                    sqlwhere.AppendFormat(@" AND sas.`status` ='{0}'", query.status);

                }
                sql.Append(sqlwhere.ToString());
                return _access.getDataTableForObj<SecretAccountSet>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SecretAccountSetDao-->GetSecretSetList" + ex.Message + sql.ToString(), ex);
            }
        }

        public int Insert(SecretAccountSet sas)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"INSERT INTO secret_account_set(user_id,secret_pwd,createdate,updatedate,`status`,pwd_status,ipfrom,user_login_attempts,secret_limit,secret_count )");
            sql.AppendFormat(@" VALUES('{0}','{1}','{2}',", sas.user_id, sas.secret_pwd, sas.createdate.ToString("yyyy-MM-dd HH:mm:ss"));
            sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}',", sas.updatedate.ToString("yyyy-MM-dd HH:mm:ss"), sas.status, sas.pwd_status, sas.ipfrom, sas.user_login_attempts);
            sql.AppendFormat(@"'{0}','{1}')", sas.secret_limit, sas.secret_count);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SecretAccountSetDao-->Insert" + ex.Message + sql.ToString(), ex);
            }

        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="sas"></param>
        /// <returns></returns>
        public int Update(SecretAccountSet sas)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@" UPDATE secret_account_set SET updatedate='{0}'  ", sas.updatedate.ToString("yyyy-MM-dd HH:mm:ss"));

            if (sas.user_id != 0)
            {
                sql.AppendFormat(@" ,user_id='{0}'", sas.user_id);
            }
            if (!string.IsNullOrEmpty(sas.ipfrom))
            {
                sql.AppendFormat(@", ipfrom='{0}' ", sas.ipfrom);
            }
            if (!string.IsNullOrEmpty(sas.secret_pwd))
            {
                sql.AppendFormat(@" , secret_pwd='{0}'", sas.secret_pwd);
            }
            if (sas.secret_limit != -1)
            {
                sql.AppendFormat(@" ,secret_limit='{0}'", sas.secret_limit);
            }
            if (sas.secret_count != -1)
            {
                sql.AppendFormat(@" ,secret_count='{0}'", sas.secret_count);
            }
            if (sas.user_login_attempts != -1)
            {
                sql.AppendFormat(@", user_login_attempts='{0}' ", sas.user_login_attempts);
            } if (sas.status != -1)
            {
                sql.AppendFormat(@" ,`status`='{0}' ", sas.status);
            }

            sql.AppendFormat(@" ,pwd_status='{0}' ", sas.pwd_status);


            sql.AppendFormat(@" WHERE id ='{0}'; ", sas.id);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SecretAccountSetDao-->UpdateCode" + ex.Message + sql.ToString(), ex);
            }

        }

        public SecretAccountSet Select(SecretAccountSet model)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendLine(@"SELECT id,user_id,secret_pwd,createdate,updatedate, `status`,pwd_status, ");
                sql.AppendLine(@"user_login_attempts,ipfrom ,secret_limit,secret_count ");
                sql.AppendLine(@" FROM secret_account_set  ");
                sql.AppendFormat("  WHERE id='{0}'", model.id);
                return _access.getSinggleObj<SecretAccountSet>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SecretAccountSetDao-->Select" + ex.Message + sql.ToString(), ex);
            }
        }
        public SecretAccountSet SelectByUserIP(SecretAccountSet model)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendLine(@"SELECT id,user_id,secret_pwd,createdate,updatedate, `status`,pwd_status, ");
                sql.AppendLine(@"user_login_attempts,ipfrom ,secret_limit,secret_count ");
                sql.AppendLine(@" FROM secret_account_set  ");
                sql.AppendFormat("  WHERE user_id='{0}'", model.user_id);
                sql.AppendFormat("  AND  ipfrom='{0}'", model.ipfrom);
                return _access.getSinggleObj<SecretAccountSet>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SecretAccountSetDao-->Select" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
