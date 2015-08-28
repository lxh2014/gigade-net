using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IEventPromoShoppingcartImplDao
    {
        List<Model.EventPromoShoppingcart> GetList(string event_id);
        string AddOrUpdate(Model.EventPromoShoppingcart epQuery);
        string Delete(string event_id);
    }
}
