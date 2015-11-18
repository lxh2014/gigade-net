using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class EmailGroupDao
    {
        private IDBAccess _access;

        public EmailGroupDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public List<EmailGroup> EmailGroupList(EmailGroup query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            totalCount = 0;
            query.Replace4MySQL();
            try
            {
                sqlCount.Append("select count(eg.group_id) 'totalCount' from email_group eg");
                sql.Append("select eg.group_id,eg.group_name,count(el.group_id) 'count',eg.group_updatedate,eg.group_update_userid,mu.user_username ");
                sqlFrom.Append("  from email_group eg LEFT JOIN email_list el on eg.group_id=el.group_id  ");
                sqlFrom.Append(" left join manage_user mu on eg.group_update_userid=mu.user_id  ");
                sqlWhere.Append(" where 1=1 ");
                if (!string.IsNullOrEmpty(query.group_name))
                {
                    sqlWhere.AppendFormat(" and  eg.group_name like '%{0}%' ", query.group_name);
                }
                DataTable _dt = _access.getDataTable(sqlCount.ToString() + sqlWhere.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0][0]);
                }
                sqlWhere.Append(" group by eg.group_id desc ");
                sqlWhere.AppendFormat("limit {0},{1};", query.Start, query.Limit);
                return _access.getDataTableForObj<EmailGroup>(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("EmailGroupDao-->EmailGroupList-->" + sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString() + ex.Message, ex);
            }
        }
        /// <summary>
        /// email_list匯入
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool ImportEmailList(EmailGroup query)
        {
            StringBuilder sql = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                sql.AppendFormat("insert into email_list (group_id,email_address,name) values('{0}','{1}','{2}');", query.group_id, query.email_address, query.name);
                if (_access.execCommand(sql.ToString()) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("EmailGroupDao-->ImportEmailList-->" + sql.ToString() + ex.Message, ex);
            }

        }
        /// <summary>
        /// email_list匯出
        /// </summary>
        /// <param name="group_id"></param>
        /// <returns></returns>
        public DataTable Export(int group_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select group_id,email_address,name from email_list where group_id='{0}';", group_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EmailGroupDao-->Export-->" + sql.ToString() + ex.Message, ex);
            }
        }
        /// <summary>
        /// email_group新增
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string InsertEmailGroup(EmailGroup query)
        {
            StringBuilder sql = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                sql.AppendFormat(" insert into email_group (group_name,group_createdate,group_updatedate,group_create_userid,group_update_userid) values('{0}',now(),now(),'{1}','{2}');", query.group_name, query.group_create_userid, query.group_update_userid);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EmailGroupDao-->InsertEmailGroup-->" + sql.ToString() + ex.Message, ex);
            }
        }

        /// <summary>
        /// email_group編輯
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string UpdateEmailGroup(EmailGroup query)
        {
            StringBuilder sql = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                sql.AppendFormat("update email_group set group_name='{0}',group_updatedate=now(),group_update_userid='{1}' where group_id='{2}';", query.group_name, query.group_update_userid, query.group_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EmailGroupDao-->UpdateEmailGroup-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public List<EmailGroup> EmailGroupStore()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select * from( select eg.group_id,eg.group_name,count(el.group_id) 'count' from email_group eg LEFT  JOIN email_list el on eg.group_id=el.group_id where 1=1 group by eg.group_id ) email where count!=0;");
                return _access.getDataTableForObj<EmailGroup>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EmailGroupDao-->UpdateEmailGroup-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable GetEmailList(int group_id)
        {
            StringBuilder sql = new StringBuilder();
            DataTable _dt = new DataTable();
            try
            {
                if (group_id != 0)
                {
                    sql.AppendFormat("select email_address,name from email_list where group_id='{0}';", group_id);
                    _dt = _access.getDataTable(sql.ToString());
                }
                return _dt;

            }
            catch (Exception ex)
            {

                throw new Exception("EmailGroupDao-->GetEmailList-->" + sql.ToString() + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查看email是否已經存在
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns>為true存在，false則不存在</returns>
        public bool IsExistEmail(string emailAddress,int group_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select email_address from email_list where email_address='{0}' and group_id='{1}';", emailAddress, group_id);
                DataTable _dt = _access.getDataTable(sql.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw new Exception("EmailGroupDao-->IsExistEmail-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string DeleteEmailList(int group_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" set sql_safe_updates = 0;delete from email_list where group_id='{0}';set sql_safe_updates = 1;", group_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EmailGroupDao-->DeleteEmailList-->" + ex.Message+sql.ToString(), ex);
            }
        }

        public string DeleteEmailGroup(int group_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" set sql_safe_updates = 0;delete from email_group where group_id='{0}';set sql_safe_updates = 1;", group_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EmailGroupDao-->EmailGroup-->" + ex.Message+sql.ToString(), ex);
            }
        }
    }
}
