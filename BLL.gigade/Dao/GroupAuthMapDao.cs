using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class GroupAuthMapDao : IGroupAuthMapImplDao
    {
        private IDBAccess _access;

        public GroupAuthMapDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        /// <summary>
        ///查詢GroupAuthMap數據
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        ///  
        public DataTable Query(GroupAuthMapQuery m)
        {
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbKeySql = new StringBuilder();
            try
            {
                sbSql.AppendFormat(@"select mu.user_id,gam.table_name,gam.column_name,gam.value,gam.table_alias_name from group_auth_map gam ");
                sbSql.AppendFormat(" LEFT JOIN t_groupcaller t_g on t_g.groupId= gam.group_id LEFT JOIN manage_user mu on mu.user_email=t_g.callid ");
                sbSql.AppendFormat(" where  gam.table_name='{0}' and mu.user_id={1} and status=1 ", m.table_name, m.user_id);
                if (!String.IsNullOrEmpty(m.column_name))
                {
                    sbSql.AppendFormat("and gam.column_name={0}", m.column_name);
                }

                sbSql.Append(" order by gam.column_name");
                return _access.getDataTable(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("GroupAuthMapDao-->Query-->" + ex.Message + sbSql.ToString(), ex);
            }
        }

        /// <summary>
        /// 查询GroupAuthMap的所有数据，或根据条件查询
        /// </summary>
        /// <param name="m">查询的条件</param>
        /// <returns>List</returns>
        public List<GroupAuthMapQuery> QueryAll(GroupAuthMapQuery m, out int totalCount)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"select content_id,table_name,table_alias_name,column_name,`value`,`status`,gm.group_id,groupName from group_auth_map gm ");
            sbSql.AppendFormat(@"left JOIN t_fgroup on gm.group_id=t_fgroup.rowid  where 1=1  ");
            //sbSql.Append(@" where 1=1 ");
            if (m.group_id != 0)
            {
                sbSql.AppendFormat(" and gm.group_id='{0}' ", m.group_id);
            }
            if (!string.IsNullOrEmpty(m.table_name))
            {
                sbSql.AppendFormat(" and gm.table_name like '%{0}%' ", m.table_name);
            }
            sbSql.Append(@" order by content_id desc");
            totalCount = 0;
            string sql = "select COUNT(gm.content_id) as search_total from group_auth_map gm ";
            if (m.IsPage)
            {
                System.Data.DataTable _dt = _access.getDataTable(sql);
                if (_dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                }
                sbSql.AppendFormat(" limit {0},{1}", m.Start, m.Limit);
            }
            try
            {
                return _access.getDataTableForObj<GroupAuthMapQuery>(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("GroupAuthMapDao-->QueryAll-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
        /// <summary>
        /// 新增GroupAuthMap
        /// </summary>
        /// <param name="query">实体类</param>
        /// <returns>返回受影响行数</returns>
        public int AddGroupAuthMapQuery(GroupAuthMapQuery query)
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.AppendFormat(@"insert into group_auth_map(group_id,table_name,column_name,`value`,");
                sbSql.AppendFormat("`status`,create_user_id,create_date,table_alias_name)VALUES(");
                sbSql.AppendFormat(" '{0}','{1}','{2}','{3}',", query.group_id, query.table_name, query.column_name, query.value);
                sbSql.AppendFormat("'{0}','{1}','{2}','{3}')", query.status, query.create_user_id, query.create_date.ToString("yyyy-MM-dd HH:mm:ss"), query.table_alias_name);
                return _access.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("GroupAuthMapDao-->AddGroupAuthMapQuery-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
        /// <summary>
        /// 修改GroupAuthMap
        /// </summary>
        /// <param name="query">实体</param>
        /// <returns></returns>
        public int UpGroupAuthMapQuery(GroupAuthMapQuery query)
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.AppendFormat(@"update group_auth_map set group_id='{0}',table_name='{1}',column_name='{2}', ", query.group_id, query.table_name, query.column_name);
                sbSql.AppendFormat("value='{0}',table_alias_name='{1}' where content_id='{2}'", query.value, query.table_alias_name, query.content_id);

                return _access.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("GroupAuthMapDao-->UpGroupAuthMapQuery-->" + ex.Message + sbSql.ToString(), ex);
            }

        }
        /// <summary>
        /// 更改权限状态
        /// </summary>
        /// <param name="content_id">编号</param>
        /// <param name="status">状态</param>
        /// <returns>受影响行数</returns>
        public int UpStatus(int content_id, int status)
        {
            StringBuilder sbSql = new StringBuilder();
            try
            {
                sbSql.AppendFormat(@"update group_auth_map set `status`='{0}'   where content_id='{1}'", status, content_id);
                return _access.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("GroupAuthMapDao-->UpStatus-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
    }
}
