using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IEventPromoAmountGiftImplMgr
    {
        List<EventPromoAmountGiftQuery> GetList(EventPromoAmountGiftQuery epQuery, out int totalCount);
        string GetCondiType(int condiType, string event_id);
        bool SavePromoAmountGift(EventPromoAmountGift epaGift, List<EventPromoGiftQuery> epGiftDetail, string condiType);
        bool UpdateActive(EventPromoAmountGift model);
    }
}
