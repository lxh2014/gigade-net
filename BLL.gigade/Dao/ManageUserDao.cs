using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class ManageUserDao : IManageUserImplDao
    {
        private IDBAccess _access;
        public ManageUserDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
        } 
        public List<ManageUserQuery> GetNameMail(ManageUserQuery query, out int totalcount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append(@" select  user_id, user_username as user_name,user_email ");
                sqlCondi.Append(" from manage_user ");
                sqlCondi.Append(" where 1=1 ");
                if (query.user_status != 0)
                {
                    sqlCondi.AppendFormat(" and  user_status='{0}' ", query.user_status);
                }
                if (!string.IsNullOrEmpty(query.user_username))
                {
                    sqlCondi.AppendFormat(" and  user_username LIKE '%{0}%' ", query.user_username);
                }

                sqlCondi.Append(" order by user_id asc ");
                totalcount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable("select count(user_id) as totalCount " + sqlCondi.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalcount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }

                    sqlCondi.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }

                sql.Append(sqlCondi.ToString());
                return _access.getDataTableForObj<ManageUserQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ManageUserDao.GetNameMail-->" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 根據userid獲取用戶信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public List<ManageUser> GetManageUser(ManageUser m)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("SELECT * from manage_user WHERE user_id='{0}'; ", m.user_id);
                return _access.getDataTableForObj<ManageUser>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao-->GetManageUser" + ex.Message + sb.ToString(), ex);
            }
        }
        public int UnlockManagerUser(ManageUser m)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("update manage_user set user_login_attempts='{0}' ", m.user_login_attempts);
                sb.AppendFormat("  , user_status='{0}' ", m.user_status);
                sb.AppendFormat("  where user_id ='{0}' ;", m.user_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ManageUserDao-->UnlockManagerUser" + ex.Message + sb.ToString(), ex);
            }
        } 
        public List<ManageUserQuery> GetManageUserList(ManageUserQuery query, out int totalcount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            try
            {
                sql.Append(@" SELECT user_id,user_username,user_email,user_status,user_login_attempts,user_last_login,manage,user_createdate,user_updatedate,erp_id ");
                sqlFrom.Append(" FROM manage_user   ");
                if (!string.IsNullOrEmpty(query.userid))
                {
                    sqlCondi.AppendFormat(" and  user_id = '{0}' ", query.userid);
                }
                if (!string.IsNullOrEmpty(query.user_username))
                {
                    sqlCondi.AppendFormat(" and  user_username like N'%{0}%' ", query.user_username);
                }
                if (!string.IsNullOrEmpty(query.user_email))
                {
                    sqlCondi.AppendFormat(" and  user_email like N'%{0}%' ", query.user_email);
                }

                if (query.search_status != "-1")
                {
                    sqlCondi.AppendFormat(" and  user_status = '{0}' ", query.search_status);
                }
                if (!string.IsNullOrEmpty(query.login_sum))
                {
                    sqlCondi.AppendFormat(" and  user_login_attempts >= '{0}' ", query.login_sum);
                }
                if (sqlCondi.Length > 0)
                {
                    sqlFrom.Append(" where " + sqlCondi.ToString().TrimStart().Remove(0, 3));
                }
                totalcount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable("select count(user_id) as totalCount " + sqlFrom.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalcount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }
                    sqlFrom.Append(" order by user_id desc ");
                    sqlFrom.AppendFormat(" limit {0},{1};", query.Start, query.Limit);
                }
                sql.Append(sqlFrom.ToString());
                return _access.getDataTableForObj<ManageUserQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ManageUserDao.GetNameMail-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int ManageUserAdd(ManageUserQuery q)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sel_id = new StringBuilder();
            DataTable dt = new DataTable();
            try
            {
                sb.Append("SELECT * FROM serial where serial_id='1';");
                dt = _access.getDataTable(sb.ToString());
                if (dt.Rows.Count > 0)
                {
                    q.user_id = uint.Parse(dt.Rows[0]["serial_value"].ToString()) + 1;
                    sb.Clear();
                    sb.AppendFormat("insert into manage_user (user_id,user_username,user_email,user_status,user_lastvisit,user_last_login,manage,user_createdate,user_updatedate,erp_id,user_password) VALUE ");
                    sb.AppendFormat("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}') ;", q.user_id, q.user_username, q.user_email, q.user_status, q.user_lastvisit, q.user_last_login, q.manage, q.user_createdate, q.user_updatedate, q.erp_id, q.user_password);
                    sb.AppendFormat("update serial set serial_value='{0}' where serial_id='1'; ", q.user_id);
                    return _access.execCommand(sb.ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ManageUserDao-->ManageUserAdd" + ex.Message + sb.ToString(), ex);
            }
        }
        public int ManageUserUpd(ManageUserQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("update manage_user set user_username='{0}' ", query.user_username);
                if (!string.IsNullOrEmpty(query.user_email))
                {
                    sb.AppendFormat(",user_email='{0}' ", query.user_email);
                }
                sb.AppendFormat(",erp_id='{0}',user_status='{1}',manage='{2}',user_updatedate='{3}' ", query.erp_id, query.user_status, query.manage, query.user_updatedate);
                if (!string.IsNullOrEmpty(query.user_delete_email))
                {
                    sb.AppendFormat(",user_delete_email='{0}' ", query.user_delete_email);
                }
                sb.AppendFormat("  where user_id ='{0}' ;", query.user_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ManageUserDao-->ManageUserUpd" + ex.Message + sb.ToString(), ex);
            }
        }
        public int UpdPassword(ManageUserQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("update manage_user set user_password='{0}',user_updatedate='{1}' ", query.user_password, query.user_updatedate);
                sb.AppendFormat(" where user_id ='{0}' ;", query.user_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ManageUserDao-->UpdPassword" + ex.Message + sb.ToString(), ex);
            }
        }

        #region 新增檢查email是否存在
        public int CheckEmail(ManageUserQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                if (!string.IsNullOrEmpty(query.user_email))
                {
                    sb.AppendFormat("SELECT user_username FROM manage_user WHERE user_email='{0}'; ", query.user_email);
                    return _access.getDataTable(sb.ToString()).Rows.Count;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao-->CheckEmail" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        public int UpdStatus(ManageUserQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("update manage_user set user_status='{0}',user_login_attempts=0,user_updatedate='{1}' ", query.user_status, query.user_updatedate);
                sb.AppendFormat(" where user_id ='{0}' ;", query.user_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ManageUserDao-->UpdStatus" + ex.Message + sb.ToString(), ex);
            }
        }

        public List<ManageUser> GetUserIdByEmail(string email)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("SELECT user_id FROM manage_user WHERE user_email = '{0}'",email);
                return _access.getDataTableForObj<ManageUser>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ManageUserDao-->GetUserIdByEmail" +ex.Message,ex);
            }
        }
        public List<ManageUser> GetkutiaoUser() //by zhaozhi0623j
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"select mu.user_id,mu.user_username from t_fgroup tfg 
                                   LEFT JOIN t_groupcaller tg on  tfg.rowid=tg.groupid
                                   left join manage_user mu on mu.user_email=tg.callid
                                   where groupCode='picking'");
                return _access.getDataTableForObj<ManageUser>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ManageUserDao-->GetkutiaoUser" + ex.Message, ex);
            }
        }
    }
}
