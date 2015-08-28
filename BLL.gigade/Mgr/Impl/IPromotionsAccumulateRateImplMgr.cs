using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IPromotionsAccumulateRateImplMgr
    {
        int Save(PromotionsAccumulateRate store);
        int Update(PromotionsAccumulateRate store);
        int Delete(PromotionsAccumulateRate Id);
        List<PromotionsAccumulateRateQuery> AllMessage(PromotionsAccumulateRateQuery query, ref int tatal);
        //string Query();
        PromotionsAccumulateRate GetModel(int id);
        int UpdateActive(PromotionsAccumulateRate store);
    }
}
