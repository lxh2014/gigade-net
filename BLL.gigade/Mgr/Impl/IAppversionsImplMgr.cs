using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IAppversionsImplMgr
    {
        //查詢分頁列表
        string GetAppversionsList(AppversionsQuery appsions);
        //通過ID刪除
        string DeleteAppversionsById(string id);
        //編輯事件
        string EditAppversionsInfo(AppversionsQuery anpq);

        int UpdateAppversionsActive(int id, int status);
    }
}
