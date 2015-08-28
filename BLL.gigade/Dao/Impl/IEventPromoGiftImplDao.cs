using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IEventPromoGiftImplDao
    {
        List<Model.Query.EventPromoGiftQuery> GetList(string event_id);
        string AddOrUpdate(Model.Query.EventPromoGiftQuery epQuery);
        string Delete(string event_id, string gift_ids);
    }
}
