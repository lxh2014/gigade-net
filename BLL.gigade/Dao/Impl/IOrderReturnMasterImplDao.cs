using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IOrderReturnMasterImplDao
    {
        List<OrderReturnMasterQuery> GetReturnMaster(OrderReturnMasterQuery ormQuery, out int totalCount);
        OrderReturnUserQuery GetReturnDetailById(uint order_id);
        int Save(OrderReturnMasterQuery query);
    }
}
