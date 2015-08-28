using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IEventPromoAmountFareImplDao
    {

        List<Model.EventPromoAmountFare> GetList(Model.EventPromoAmountFare model, out int totalCount);
        string AddOrUpdate(Model.EventPromoAmountFare model);
        string UpdateEventId(int row_id, string event_id);
        string UpdateActive(Model.EventPromoAmountFare model);
    }
}
