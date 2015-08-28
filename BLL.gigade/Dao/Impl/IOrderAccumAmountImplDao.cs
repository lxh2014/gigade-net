using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IOrderAccumAmountImplDao
    {
       DataTable GetOrderAccumAmountTable(OrderAccumAmountQuery query, out int totalCount);
       string AddOrderAccumAmount(OrderAccumAmountQuery query);
       string UPOrderAccumAmount(OrderAccumAmountQuery query);
       string UpdateActive(OrderAccumAmountQuery query);
       string DelOrderAccumAmount(OrderAccumAmountQuery query);
    }
}
