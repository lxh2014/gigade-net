using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IEventPromoUserConditionImplDao
    {
        DataTable GetList(EventPromoUserConditionQuery epQuery, out int totalCount);
        DataTable GetEventCondi(Model.Query.EventPromoUserConditionQuery epQuery);
        int AddOrUpdate(EventPromoUserConditionQuery epQuery);
        int Delete(EventPromoUserConditionQuery epQuery);
    }
}
