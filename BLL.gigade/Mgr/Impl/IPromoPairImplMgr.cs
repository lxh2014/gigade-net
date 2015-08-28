using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Mgr.Impl
{
   public interface IPromoPairImplMgr
    {
       int Save(PromoPair promopair,PromoPairQuery PPquery);
       int Update(PromoPair promopair,PromoPairQuery PPQuery);
       int Delete(PromoPair promopair);
        List<PromoPairQuery> Query(PromoPairQuery store, out int totalCount);
        PromoPair GetMOdel(int id);
        int Save(PromoPair promopair);
        int SaveTwo(PromoPair promopair, PromoPairQuery PPQuery);
        DataTable Select(PromoPair query);
        DataTable SelCategoryID(PromoPair query);
        PromoPair GetModelById(int id);
        string CategoryID(PromoPair Model);
        int UpdateActive(PromoPairQuery store);
        PromoPairQuery Select(int id);
    }
}
