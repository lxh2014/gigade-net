#region 文件信息
/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：UserLoginLogDao.cs 
 * 摘   要： 
 *      會員登入記錄與資料庫交互方法
 * 当前版本：v1.2 
 * 作   者： dongya0410j
 * 完成日期：2014/6/20
 * 修改歷史：
 *      v1.1修改日期：2014/9/23
 *      v1.1修改人員：changjian0408j
 *      v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋
 */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Common;
namespace BLL.gigade.Dao
{
    public class UserLoginLogDao : IUserLoginLogImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        public UserLoginLogDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);

        }
        #region 查詢出登陸記錄數據+List<Model.Query.UsersLoginQuery> Query(Model.Query.UsersLoginQuery store, out int totalCount)
        public List<Model.Query.UsersLoginQuery> Query(Model.Query.UsersLoginQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            try
            {
                sql.Append("SELECT ul.login_createdate,ul.login_id,ul.login_ipfrom,ul.user_id,u.user_name as username ");
                sqlcount.Append("select count(ul.user_id) as totalcounts ");
                sqlfrom.Append("FROM users_login ul left join users u on ul.user_id=u.user_id  WHERE 1=1 ");
                if (store.serchstart != DateTime.MinValue && store.serchend != DateTime.MinValue)
                {
                    sqlfrom.AppendFormat(" AND ul.login_createdate >= '{0}' AND ul.login_createdate <= '{1}'", CommonFunction.GetPHPTime(store.serchstart.ToString()), CommonFunction.GetPHPTime(store.serchend.ToString()));
                }
                if (!string.IsNullOrEmpty(store.user_id.ToString()))
                {
                    if (store.user_id.ToString() == "0")
                    {
                        sqlfrom.Append(" ");
                    }
                    else
                    {
                        sqlfrom.AppendFormat(" and ul.user_id = '{0}'", store.user_id);
                    }
                }
                else
                {
                    sqlfrom.AppendFormat(" and ul.user_id='{0}'", store.user_id);
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
            try
            {
                return _access.getDataTableForObj<UsersLoginQuery>(sql.ToString() + sqlfrom.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("UserLoginLog.Query-->" + sql.ToString() + sql.ToString() + ex.Message, ex);
            }

        }
        #endregion
        #region 獲取機敏信息
        public System.Data.DataTable GetUserInfo(int rid)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            sql.Append("SELECT ul.login_id,ul.user_id,u.user_name,u.user_email,u.user_phone,u.user_address ");
            sqlfrom.Append("FROM users_login ul left join users u on ul.user_id=u.user_id ");
            sqlfrom.Append(" WHERE 1=1");
            try
            {
                if (rid != 0)
                {
                    sqlfrom.AppendFormat(" and ul.login_id='{0}'", rid);
                }
                sql.Append(sqlfrom.ToString());
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("UserLoginLog.Query-->" + sql.ToString() + ex.Message, ex);
            }

        }
        #endregion
    }
}
