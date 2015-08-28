using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IOrderSlaveMasterImplDao
    {
        List<OrderSlaveMasterQuery> GetBatchList(OrderSlaveMasterQuery store, out int totalCount);
        List<OrderSlaveMasterQuery> GetSlaveByMasterId(OrderSlaveMasterQuery store);
        List<OrderSlaveMasterQuery> GetDetailBySlave(string slaves);
        List<OrderSlaveMasterQuery> GetOrderByMasterIDs(string slaves);
        //List<OrderSlaveMasterQuery> GetDeliverByOrderIDs(string orderIDs);//到貨確認功能棄用
    }
}
