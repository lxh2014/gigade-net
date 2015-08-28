using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IPromoAdditionalPriceMgr
    {
        List<PromoAdditionalPriceQuery> Query(PromoAdditionalPriceQuery store, out int totalCount);
        int InsertFirst(PromoAdditionalPrice m);
        int InsertSecond(PromoAdditionalPrice m, PromoAdditionalPriceQuery mq);
        int Update(PromoAdditionalPrice m, PromoAdditionalPriceQuery mq);
        int Delete(int i, string str);
        int ChangeActive(PromoAdditionalPriceQuery m);
        PromoAdditionalPrice GetModel(int id);
        string CategoryID(PromoAdditionalPrice m);
        PromoAdditionalPriceQuery Select(int id);
        int DeletLessThen(PromoAdditionalPriceQuery m, int types);
    } 
}
