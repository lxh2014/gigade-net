using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
namespace BLL.gigade.Dao.Impl
{
   public interface IOrderCancelMasterImplDao
    {
        List<OrderCancelMaster> GetOrderCancelMasterList(OrderCancelMaster ocm, out int totalCount);
        int Update(OrderCancelMaster ocm);
    }
}
