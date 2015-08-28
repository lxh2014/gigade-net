using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IEventPromoDiscountImplDao
    {
        List<Model.EventPromoDiscount> GetList(string event_id);
        string AddOrUpdate(Model.EventPromoDiscount model);
        string Delete(string event_id, string gift_ids);
    }
}
