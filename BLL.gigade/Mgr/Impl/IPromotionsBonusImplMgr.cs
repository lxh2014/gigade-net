using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IPromotionsBonusImplMgr
    {
        int Save(Model.PromotionsBonus promoBonus);
        int Update(Model.PromotionsBonus promoBonus);
        int Delete(Model.PromotionsBonus pId);
        List<PromotionsBonusQuery> Query(PromotionsBonusQuery store, out int totalCount);
        PromotionsBonus GetModel(int id);
        int UpdateActive(PromotionsBonus store);
    }
}
