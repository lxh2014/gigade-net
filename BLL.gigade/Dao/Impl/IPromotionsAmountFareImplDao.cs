using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IPromotionsAmountFareImplDao
    {
        List<PromotionsAmountFareQuery> Query(PromotionsAmountFareQuery query, out int totalCount);
        int Save(PromotionsAmountFareQuery model);
        System.Data.DataTable Select(PromotionsAmountFare model);
        PromotionsAmountFareQuery Select(int id);
        int ReSave(PromotionsAmountFareQuery model);
        int Delete(int id);
        int Update(PromotionsAmountFareQuery model,string oldEventId);
        int UpdateActive(PromotionsAmountFareQuery model);

    }
}
