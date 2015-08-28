using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
namespace BLL.gigade.Dao.Impl
{
    public interface IEventPromoAmountDiscountImplDao
    {
        List<EventPromoAmountDiscountQuery> GetList(EventPromoAmountDiscountQuery epQuery, out int totalCount);
        string AddOrUpdate(Model.EventPromoAmountDiscount model);
        string UpdateEventId(int row_id, string event_id);
        string UpdateActive(Model.EventPromoAmountDiscount model);
    }
}
