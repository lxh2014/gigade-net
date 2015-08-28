using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using System.Data;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Mgr.Impl
{
    public interface IPromotionsAmountDiscountImplMgr
    {
        List<PromotionAmountDiscountQuery> Query(PromotionAmountDiscountQuery query, out int totalCount, string eventtype);
        DataTable Save(PromotionsAmountDiscount query);
        bool UpdateActive(PromotionsAmountDiscountCustom query);
        int Delete(PromotionsAmountDiscount query, string eventid);
        //int ReSave(PromotionsAmountDiscountCustom model);
        //int ReUpdate(PromotionsAmountDiscountCustom model, string oldEventId);
        PromotionsAmountDiscountCustom GetModelById(int id);
        bool ReUpdateDiscount(PromotionsAmountDiscountCustom model, string oldEventId);
        bool ReSaveDiscount(PromotionsAmountDiscountCustom model);
         
        List<PromotionsAmountDiscountCustom> GetList(PromotionsAmountDiscountCustom query, out int totalCount);
        int Save(PromotionsAmountDiscountCustom query);
        bool Delete(List<PromotionsAmountDiscountCustom> list);
        bool UpdatePromoAmountDisActive(PromotionsAmountDiscountCustom model);
    }
}
