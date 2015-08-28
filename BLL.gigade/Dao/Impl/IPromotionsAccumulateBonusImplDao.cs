using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface IPromotionsAccumulateBonusImplDao
    {
        int Save(Model.PromotionsAccumulateBonus promoAccumulateBonus);

        int Update(Model.PromotionsAccumulateBonus promoAccumulateBonus);

        int Delete(int Id);

        List<Model.Query.PromotionsAccumulateBonusQuery> Query(Model.Query.PromotionsAccumulateBonusQuery store, out int totalCount);

        PromotionsAccumulateBonus GetModel(int id);
    }
}
