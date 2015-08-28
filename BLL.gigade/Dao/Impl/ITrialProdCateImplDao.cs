using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface ITrialProdCateImplDao
    {
        List<TrialProdCateQuery> Query(TrialProdCateQuery query, out int totalCount);
        List<TrialProdCateQuery> UadateTrialProd();
        string InsertTrialProd(TrialProdCateQuery query);
        string DeleteTrialProd();
    }
}
 