using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class MailUserDao : IMailUserImplDao
    {
        private IDBAccess _access;
        public MailUserDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public List<MailUserQuery> GetMailUserStore(MailUserQuery query, out int totalcount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append(@" select  row_id,user_mail,user_name,`status`,user_pwd,create_time,create_user,update_time,update_user ");
                sqlCondi.Append(" from mail_user ");
                sqlCondi.Append(" where 1=1 ");
                if (!string.IsNullOrEmpty(query.user_name))
                {
                    sqlCondi.AppendFormat(" and user_name like '%{0}%' ", query.user_name);
                }
                if (!string.IsNullOrEmpty(query.user_mail))
                {
                    sqlCondi.AppendFormat(" and user_mail like '%{0}%' ", query.user_mail);
                }
                if (query.row_id != 0)
                {
                    sqlCondi.AppendFormat(" and row_id = '{0}' ", query.row_id);
                }
                totalcount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable("select count(row_id) as totalCount " + sqlCondi.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalcount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }
                    sqlCondi.Append(" order by row_id desc ");
                    sqlCondi.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                else
                {
                    sqlCondi.Append(" and  status=1; ");
                }

                sql.Append(sqlCondi.ToString());
                return _access.getDataTableForObj<MailUserQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("MailUserDao.GetMailUserStore-->" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 有關mail的查詢（此方法可擴充，擴充后請詳細注釋）
        /// 內容：根據用戶編號查詢啟用狀態用戶email地址 add by shuangshuang0420j 2015.02.04
        /// 修改：
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<MailUserQuery> MailUserQuery(MailUserQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append(@" select  row_id,user_mail,user_name,`status`,user_pwd,create_time,create_user,update_time,update_user ");
                sqlCondi.Append(" from mail_user ");
                sqlCondi.Append(" where 1=1 ");//啟用狀態
                if (query.row_id != 0)
                {
                    sqlCondi.AppendFormat(" and row_id={0}", query.row_id);
                }
                if (query.status != -1)
                {
                    sqlCondi.AppendFormat(" and `status`={0}", query.status);
                }
                sql.Append(sqlCondi.ToString());
                return _access.getDataTableForObj<MailUserQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("MailUserDao.MailUserQuery-->" + ex.Message + sql.ToString(), ex);
            }

        }
        /// <summary>
        /// 新增或修改用戶信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int SaveMailUser(MailUserQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                if (VerifyUserMail(query))
                {
                    if (query.row_id == 0)
                    {
                        sql.Append(@"insert into mail_user(user_mail,user_name,status,user_pwd,create_time,create_user,update_time,update_user)values(");
                        sql.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',", query.user_mail, query.user_name, query.status, query.user_pwd, CommonFunction.DateTimeToString(query.create_time));
                        sql.AppendFormat(" '{0}','{1}','{2}')", query.create_user, CommonFunction.DateTimeToString(query.update_time), query.update_user);
                    }
                    else
                    {

                        sql.AppendFormat(@"update mail_user set user_mail='{0}',user_name='{1}',user_pwd='{2}',", query.user_mail, query.user_name, query.user_pwd);
                        sql.AppendFormat(" update_time='{0}',update_user='{1}' where row_id='{2}' ", CommonFunction.DateTimeToString(query.update_time), query.update_user, query.row_id);
                    }
                    return _access.execCommand(sql.ToString());
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("MailUserDao-->SaveMailUser-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public bool VerifyUserMail(MailUserQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {


                if (query.row_id == 0)//新增 count=0；
                {
                    sql.AppendFormat("select count(row_id) as num from mail_user where user_mail='{0}' ; ", query.user_mail);
                    DataTable _dt = _access.getDataTable(sql.ToString());
                    if (Convert.ToInt32(_dt.Rows[0]["num"].ToString()) == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else//編輯
                {
                    sql.AppendFormat("select count(row_id) as num from mail_user where user_mail='{0}' and row_id !={1}; ", query.user_mail, query.row_id);
                    DataTable _dt = _access.getDataTable(sql.ToString());
                    if (Convert.ToInt32(_dt.Rows[0]["num"].ToString()) == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("MapGroupDao-->VerifyGroup-->" + sql.ToString() + ex.Message, ex);
            }
        }
        /// <summary>
        /// 刪除用戶信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int DeleteMailUser(MailUserQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                if (!string.IsNullOrEmpty(query.row_id_in))
                {
                    sql.AppendFormat(@"delete from mail_user where row_id in({0})", query.row_id_in);
                }
                else
                {
                    sql.AppendFormat(@"delete from mail_user where row_id ='{0}'", query.row_id);
                }
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("MailUserDao-->DeleteMailUser-->" + sql.ToString() + ex.Message, ex);
            }
        }
        /// <summary>
        /// 更改啟用禁用狀態
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int UpdateMailUserStatus(MailUserQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"update mail_group_map set status='{0}' where user_id='{1}';", query.status, query.row_id);
                sql.AppendFormat(@"update mail_user set `status`='{0}', update_time='{1}' ", query.status, CommonFunction.DateTimeToString(query.update_time));
                sql.AppendFormat(" ,update_user='{0}' where row_id='{1}'", query.update_user, query.row_id);

                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("MailUserDao-->UpdateMailUserStatus-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable GetUserInfo(int r_id)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@" select  row_id,mu.user_mail as user_email,mu.user_name,u.user_id,u.user_phone,u.user_address");
            sql.Append(" from mail_user mu");
            sql.Append(" left join users u on u.user_email=mu.user_mail");

            sql.AppendFormat(" where mu.row_id='{0}' ", r_id);
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("MailUserDao-->GetUserInfo-->" + sql.ToString() + ex.Message, ex);
            }
        }
    }
}
