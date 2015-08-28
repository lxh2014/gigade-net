using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model.Query;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
    public class UserHistoryDao : IUserHistoryImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        public UserHistoryDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
     
        #region 查詢出登陸記錄數據
        public List<Model.Query.UsersLoginQuery> Query(Model.Query.UsersLoginQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            sql.Append("SELECT ul.login_createdate,ul.login_id,ul.login_ipfrom,ul.user_id,u.user_name as username ");
            sqlcount.Append("select count(ul.user_id) as totalcounts ");
            sqlfrom.Append("FROM users_login ul left join users u on ul.user_id=u.user_id ");
            sqlfrom.AppendFormat(" WHERE ul.login_createdate >= '{0}' AND ul.login_createdate <= '{1}'", CommonFunction.GetPHPTime(store.serchstart.ToString()), CommonFunction.GetPHPTime(store.serchend.ToString()));
            try
            {
                if (!string.IsNullOrEmpty(store.login_id.ToString()))
                {
                    if (store.login_id.ToString() == "0")
                    {
                        sqlfrom.Append(" ");
                    }
                    else
                    {
                        sqlfrom.AppendFormat(" and ul.login_id like '%{0}%'", store.login_id);
                    }
                }
                else
                {
                    sqlfrom.AppendFormat(" and ul.login_id  like '%{0}%'", store.login_id);
                }
            }
            catch
            {
                sqlfrom.Append(" ");
            }
            sqlfrom.AppendFormat(" ORDER BY login_createdate DESC, ul.user_id ASC");
            totalCount = 0;
            if (store.IsPage)
            {
                System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString() + sqlfrom.ToString());

                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["totalcounts"]);
                }

                sqlfrom.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
            }
            return _access.getDataTableForObj<UsersLoginQuery>(sql.ToString() + sqlfrom.ToString());
        }
        #endregion

        #region 事物 添加電話會員信息 記錄到user_history 返回sql語句
        /// <summary>
        /// 用於返回事物所用到的sql語句
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string Save(UserQuery model)
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" insert into user_history (user_id,user_name,file_name,content,creat_time,ip ) ");
            sb.AppendFormat(" values({0},'{1}','{2}','{3}',{4},'{5}');", model.kuser_id, model.kuser_name, model.file_name, model.content, CommonFunction.GetPHPTime(model.created.ToString()), model.ip);
            return sb.ToString();
        }
        #endregion

        #region 事物 添加供應商 記錄到user_history 返回sql語句
        /// <summary>
        /// 用於返回事物所用到的sql語句
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string Save(VendorQuery model)
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" insert into user_history (user_id,user_name,file_name,content,creat_time,ip ) ");
            sb.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}','{5}');", model.kuser_id, model.kuser_name, model.file_name, model.content, CommonFunction.GetPHPTime(model.created.ToString()), model.ip);
            return sb.ToString();
        }
        #endregion
    }
}
