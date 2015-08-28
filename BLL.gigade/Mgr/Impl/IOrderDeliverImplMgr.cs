using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Mgr.Impl
{
    public interface IOrderDeliverImplMgr
    {
        List<OrderDeliverQuery> GetOrderDeliverList(OrderDeliverQuery query, out int totalCount);
        int DismantleSlave(int slave_id, string select_did, DataTable dt);
    }
}
