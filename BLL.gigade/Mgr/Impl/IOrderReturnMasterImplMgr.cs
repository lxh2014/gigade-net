using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IOrderReturnMasterImplMgr
    {
        List<OrderReturnMasterQuery> GetReturnMaster(OrderReturnMasterQuery ormQuery, out int totalCount);
        OrderReturnUserQuery GetReturnDetailById(uint order_id);
        int Save(OrderReturnMasterQuery query);
    }
}
