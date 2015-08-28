using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
namespace BLL.gigade.Mgr.Impl
{
    public interface IEventPromoAmountDiscountImplMgr
    {
        List<EventPromoAmountDiscountQuery> GetList(EventPromoAmountDiscountQuery epQuery, out int totalCount);
        string GetCondiType(int condiType, string event_id);
        bool SavePromoAmountDiscount(EventPromoAmountDiscount epaGift, List<EventPromoDiscount> epGiftDetail, string condiType);
        bool UpdateActive(EventPromoAmountDiscount model);
    }
}
