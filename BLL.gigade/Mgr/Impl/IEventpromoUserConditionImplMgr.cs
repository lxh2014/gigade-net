using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IEventpromoUserConditionImplMgr
    {
        DataTable GetList(EventPromoUserConditionQuery epQuery, out int totalCount);
        string GetEventCondi(Model.Query.EventPromoUserConditionQuery epQuery);
        int AddOrUpdate(EventPromoUserConditionQuery epQuery);
        int Delete(EventPromoUserConditionQuery epQuery);
    }
}
