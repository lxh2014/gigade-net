using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface ITrialProdCateImplMgr
    { 
        List<TrialProdCateQuery> Query(TrialProdCateQuery query, out int totalCount);
        List<TrialProdCateQuery> UadateTrialProd();
        bool InsertTrialProd(List<TrialProdCateQuery> tpcQueryLi);
        string InsertTrialProd(TrialProdCateQuery query);
        string DeleteTrialProd();
    }
}
