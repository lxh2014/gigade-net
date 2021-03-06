﻿using BLL.gigade.Dao.Impl;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using System.Data;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao
{
    public class AppNotifyPoolDao : IAppNotifyPoolImplDao
    {
        private IDBAccess _access;
        private string connStr;
        string strSql = string.Empty;
        public AppNotifyPoolDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        #region 推播設定方法(肖國棟 2015.8.21)
        /// <summary>
        /// 通過開始時間和結束時間獲取推播設定
        /// </summary>
        /// <param name="valid_start">開始時間</param>
        /// <param name="valid_end">結束時間</param>
        /// <param name="totalCount">總頁數</param>
        /// <returns></returns>
        public List<AppNotifyPoolQuery> GetAppnotifypool(AppNotifyPoolQuery ap, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select apl.id,apl.title,apl.alert, apl.url,apl.to,FROM_UNIXTIME(apl.valid_start) as starttime 
                    ,FROM_UNIXTIME(apl.valid_end) as endtime,apl.notified,FROM_UNIXTIME(apl.notify_time) as  notifytime
                    from app_notify_pool as apl ");
                sqlcount.AppendFormat(" select count(apl.id) as totalCount   from app_notify_pool as apl ");
                sqlwhere.Append(" where 1=1");
                if (ap.valid_start != 0)
                {
                    sqlwhere.AppendFormat(@" AND   apl.valid_start>={0} ", ap.valid_start);
                }
                if (ap.startendtime != 0) 
                {
                    sqlwhere.AppendFormat(@" AND   apl.valid_start<={0} ", ap.startendtime);
                }
                if (ap.valid_end != 0)
                {
                    sqlwhere.AppendFormat(@" AND apl.valid_end>={0}", ap.valid_end);
                }
                if (ap.endendtime != 0)
                {
                    sqlwhere.AppendFormat(@" AND apl.valid_end<={0}", ap.endendtime);
                }
                sql.Append(sqlwhere);
                sql.AppendFormat(" order by  apl.valid_end DESC ");
                totalCount = 0;
                if (ap.IsPage)
                {
                    sqlcount.Append(sqlwhere.ToString());
                    sql.AppendFormat(@" limit {0},{1} ", ap.Start, ap.Limit);
                    DataTable dt = _access.getDataTable(sqlcount.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                    }
                }
                return _access.getDataTableForObj<AppNotifyPoolQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("AppNotifyPoolDao-->GetAppnotifypool" + ex.Message + sql.ToString() + sqlcount.ToString(), ex);
            }
        }
        /// <summary>
        /// Appnotifypool表增加方法
        /// </summary>
        /// <param name="anpq"></param>
        /// <returns></returns>
        public int AddAppnotifypool(AppNotifyPoolQuery anpq)
        {
            try
            {
                anpq.Replace4MySQL();
                strSql = string.Format(@"insert into app_notify_pool(title,alert,url,`to`,valid_start,valid_end,notified)
                                                values('{0}','{1}','{2}','{3}',{4},{5},{6});select @@identity",
                        anpq.title, anpq.alert, anpq.url, anpq.to, anpq.valid_start, anpq.valid_end, anpq.notified
                );
                return _access.execCommand(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception("AppNotifyPoolDao-->AddAppnotifypool" + ex.Message + strSql.ToString(), ex);
            }

        }
        #endregion
    }
}
