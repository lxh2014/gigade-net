using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface ISalesUserImplMgr
    {
        List<SalesUserQuery> Query(SalesUserQuery store, out int totalCount);
        int SaveUserList(SalesUserQuery usr);
        BLL.gigade.Model.Custom.Users getModel(int id);
        int Selectbigsid();
        int updatesaleuser(SalesUserQuery usr);
        SalesUser MySalesUser(int id);
    }
}
