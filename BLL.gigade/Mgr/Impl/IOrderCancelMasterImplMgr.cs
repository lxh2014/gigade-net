using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
namespace BLL.gigade.Mgr.Impl
{
   public interface IOrderCancelMasterImplMgr
    {
        List<OrderCancelMaster> GetOrderCancelMasterList(OrderCancelMaster ocm, out int totalCount);
        int Update(OrderCancelMaster ocm);
    }
}
