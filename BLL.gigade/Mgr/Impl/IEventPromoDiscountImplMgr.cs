using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IEventPromoDiscountImplMgr
    {
        List<Model.EventPromoDiscount> GetList(string event_id);
    }
}
