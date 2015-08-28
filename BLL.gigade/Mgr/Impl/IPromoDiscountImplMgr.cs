using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Mgr.Impl
{
   public interface IPromoDiscountImplMgr
    {
       List<PromoDiscount> GetPromoDiscount(PromoDiscount model);
       int DeleteByRid(int rid);
       int DeleteByEventid(PromoDiscount model);
       int Save(PromoDiscount model);
       int Update(PromoDiscount model);
       DataTable GetLimitByEventId(string event_id,int rid);
    }
}
