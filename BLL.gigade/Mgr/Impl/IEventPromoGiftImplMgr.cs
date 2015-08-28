using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IEventPromoGiftImplMgr
    {
        List<Model.Query.EventPromoGiftQuery> GetList(string event_id);
    }
}
