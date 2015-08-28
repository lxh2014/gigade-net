using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using System.Data;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    /// <summary>
    /// 
    /// </summary>
   public interface IPromoPairImplDao
    {
       //List<PromoPair> mylist();
       //List<PromoPairQuery> Query(PromoPairQuery query);
       List<PromoPairQuery> QueryAll(Model.Query.PromoPairQuery query, out int totalCount);
       //DataTable Save(PromoPair query);
       int SaveOne(PromoPair query);
       int SaveTwo(PromoPair query,PromoPairQuery PPquery);
       int Update(PromoPair query, PromoPairQuery PPquery);
       int Delete(PromoPair query);
       PromoPair GetPPModel(int id);
       DataTable Select(PromoPair pronopair);
       DataTable SelCategoryID(PromoPair pronopair);
       PromoPair GetModelById(int id);
       string CategoryID(PromoPair pronopair);
       //int ReSave(PromotionsAmountDiscountCustom model);
       //int ReUpdate(PromotionsAmountDiscountCustom model);
       //PromoPair GetModelById(int id);
       int UpdateActive(PromoPairQuery store);
       PromoPairQuery Select(int id);
    }
}
