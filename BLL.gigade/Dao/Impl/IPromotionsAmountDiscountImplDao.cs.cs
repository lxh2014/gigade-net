using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using System.Data;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao.Impl
{
    public interface IPromotionsAmountDiscountImplDao
    {
        List<PromotionAmountDiscountQuery> Query(PromotionAmountDiscountQuery query, out int totalCount, string eventtype);
        DataTable Save(PromotionsAmountDiscount query);
        //int UpdateActive(PromotionsAmountDiscountCustom query);
        int Delete(PromotionsAmountDiscount query, string eventid);
        //int ReSave(PromotionsAmountDiscountCustom model);
        //int ReUpdate(PromotionsAmountDiscountCustom model, string oldEventId);
        PromotionsAmountDiscountCustom GetModelById(int id);

        string UpdatePromoAmountDis(PromotionsAmountDiscountCustom model);

        List<PromotionsAmountDiscountCustom> GetList(PromotionsAmountDiscountCustom query, out int totalCount);
        int Save(PromotionsAmountDiscountCustom query);
        bool Delete(List<PromotionsAmountDiscountCustom> list);
        string UpdatePromoAmountDisActive(PromotionsAmountDiscountCustom model);
    }
}
 