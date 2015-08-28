using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Mgr.Impl
{
    public interface IOrderSlaveMasterImplMgr
    {
        List<OrderSlaveMasterQuery> GetBatchList(OrderSlaveMasterQuery store, out int totalCount);
        List<OrderSlaveMasterQuery> GetSlaveByMasterId(OrderSlaveMasterQuery store);
        List<OrderSlaveMasterQuery> GetDetailBySlave(string slaves);
        // bool BatchSendProd(string slaveMasters, string userName, string userIP);//到貨確認功能棄用
    }
}
