using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface ISalesUserImplDao
    {
        List<SalesUserQuery> Query(SalesUserQuery store, out int totalCount);
        int SaveUserList(SalesUserQuery usr);
        BLL.gigade.Model.Custom.Users getModel(int id);
        int Selectbigsid();
        int updatesaleuser(SalesUserQuery usr);
        SalesUser MySalesUser(int id);
    }
}
