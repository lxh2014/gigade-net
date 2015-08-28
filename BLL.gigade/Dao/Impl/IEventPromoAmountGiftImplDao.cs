using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IEventPromoAmountGiftImplDao
    {
        List<EventPromoAmountGiftQuery> GetList(EventPromoAmountGiftQuery epQuery, out int totalCount);
        string AddOrUpdate(Model.EventPromoAmountGift model);
        string UpdateEventId(int row_id, string event_id);
        string UpdateActive(Model.EventPromoAmountGift model);
    }
}
