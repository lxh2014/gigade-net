using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface IPromotionsBonusImplDao
    {
        int Save(Model.PromotionsBonus promoBonus);

        int Update(Model.PromotionsBonus promoBonus);

        int Delete(Model.PromotionsBonus pId);

        List<Model.Query.PromotionsBonusQuery> Query(Model.Query.PromotionsBonusQuery store, out int totalCount);

        PromotionsBonus GetModel(int id);

        int UpdateActive(PromotionsBonus store);
    }
}
