using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    interface IPromotionsAccumulateRateImplDao
    {
        int Save(PromotionsAccumulateRate store);
        int Update(PromotionsAccumulateRate store);
        int Delete(PromotionsAccumulateRate rodId);

        /// <summary>
        /// 店家查詢
        /// </summary>
        /// <param name="store">Store Model</param>
        /// <param name="status">店家狀態</param>
        /// <returns>Store Model List</returns>
        //List<PromotionsAccumulateRateQuery> Query();
        List<PromotionsAccumulateRateQuery> Query(PromotionsAccumulateRateQuery query, ref int totalCount);
        PromotionsAccumulateRate GetModel(int id);
        int UpdateActive(PromotionsAccumulateRate store);
    }
}
