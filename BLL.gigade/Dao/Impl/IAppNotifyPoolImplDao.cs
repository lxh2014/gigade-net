using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IAppNotifyPoolImplDao
    {
        /// <summary>
        /// 通過開始時間和結束時間獲取推播設定
        /// </summary>
        /// <param name="totalCount">總頁數</param>
        /// <returns></returns>
        List<AppNotifyPoolQuery> GetAppnotifypool(AppNotifyPool ap, out int totalCount);
            /// <summary>
        /// Appnotifypool表增加方法
        /// </summary>
        /// <param name="anpq"></param>
        /// <returns></returns>
         int AddAppnotifypool(AppNotifyPoolQuery anpq);
    }
}
