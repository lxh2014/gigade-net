using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IPromotionsAccumulateBonusImplMgr
    {
        int Save(Model.PromotionsAccumulateBonus promotionsAccumulateBonus);
        int Update(Model.PromotionsAccumulateBonus promotionsAccumulateBonus);
        int Delete(int Id);
        List<PromotionsAccumulateBonusQuery> Query(PromotionsAccumulateBonusQuery store, out int totalCount);
        PromotionsAccumulateBonus GetModel(int id);
    }
}
