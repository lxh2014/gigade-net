using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using System.Data;
namespace BLL.gigade.Dao
{
    public class UserForbidDao : IUserForbidImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        public UserForbidDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        #region 黑名單列表頁+List<UserForbidQuery> GetUserForbidList(UserForbidQuery store, out int totalCount)
        /// <summary>
        /// 黑名單列表頁
        /// </summary>
        /// <param name="store"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<UserForbidQuery> GetUserForbidList(UserForbidQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            try
            {
                sql.Append(@"SELECT  uf.forbid_id, uf.forbid_ip,uf.forbid_createdate,FROM_UNIXTIME(uf.forbid_createdate) create_time,uf.forbid_createuser,mu.user_username ");
                sqlfrom.AppendFormat(" from user_forbid uf left join manage_user mu on uf.forbid_createuser=mu.user_id  where 1=1 ");
                if (store.forbid_ip != "")
                {
                    sqlfrom.AppendFormat(" and uf.forbid_ip='{0}' ", store.forbid_ip);
                }
                if (!string.IsNullOrEmpty(store.timestart))
                {
                    sqlfrom.AppendFormat(" and uf.forbid_createdate>='{0}' ",CommonFunction.GetPHPTime(store.timestart));
                }
                if (!string.IsNullOrEmpty(store.timeend))
                {
                    sqlfrom.AppendFormat(" and uf.forbid_createdate<='{0}' ",CommonFunction.GetPHPTime(store.timeend));
                }
                string str = string.Format(" SELECT count(*) AS search_total  ");
                totalCount = 0;
                if (store.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(str + sqlfrom.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {

                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }
                    sqlfrom.AppendFormat(" order by uf.forbid_id desc  ");
                    sqlfrom.AppendFormat(" limit {0},{1};", store.Start, store.Limit);

                }
                return _access.getDataTableForObj<UserForbidQuery>(sql.ToString() + sqlfrom.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("UserForbidDao-->GetUserForbidList-->" + sqlfrom.ToString() + ex.Message, ex);
            }


        }
        #endregion

        #region  新增黑名單+int UserForbidInsert(UserForbidQuery query)
        /// <summary>
        /// 新增黑名單
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int UserForbidInsert(UserForbidQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"insert into user_forbid(forbid_ip,forbid_createdate,forbid_createuser) values('{0}','{1}','{2}')", query.forbid_ip, query.forbid_createdate, query.forbid_createuser);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UserForbidDao-->UserForbidInsert-->" + ex.Message + sb.ToString(), ex);
            }

        }
        #endregion

        #region  刪除黑名單+int UserForbidDelete(UserForbidQuery query)
        /// <summary>
        /// 刪除黑名單
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int UserForbidDelete(UserForbidQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"delete from user_forbid where forbid_id in ({0})", query.rowIds);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UserForbidDao-->UserForbidDelete-->" + ex.Message + sb.ToString(), ex);
            }

        }
        #endregion

        #region  判斷黑名單中是否已經存在IP+int GetUserForbidIp(UserForbidQuery query)
        /// <summary>
        /// 判斷黑名單中是否已經存在IP
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int GetUserForbidIp(UserForbidQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"select forbid_id from user_forbid where forbid_ip = '{0}'", query.forbid_ip);
                UserForbidQuery temp = _access.getSinggleObj<UserForbidQuery>(sb.ToString());
                if (temp != null)
                {
                    return temp.forbid_id;
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("UserForbidDao-->GetUserForbidIp-->" + ex.Message + sb.ToString(), ex);
            }

        }
        #endregion
    }
}
