using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IAppNotifyPoolImplMgr
    {
        //查詢事件
        string GetAppnotifypool(AppNotifyPool ap);
        //編輯事件
        string EditAppNotifyPoolInfo(AppNotifyPoolQuery anpq);
    }
}
