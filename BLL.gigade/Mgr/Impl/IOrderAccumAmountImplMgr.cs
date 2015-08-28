using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IOrderAccumAmountImplMgr
    {
        DataTable GetOrderAccumAmountTable(OrderAccumAmountQuery query, out int totalCount);
        int AddOrderAccumAmount(OrderAccumAmountQuery query);
        int UPOrderAccumAmount(OrderAccumAmountQuery query);
        int UpdateActive(OrderAccumAmountQuery query);
        int DelOrderAccumAmount(OrderAccumAmountQuery query);
    }
}
