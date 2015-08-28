using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    public interface IOrderMoneyReturnImplDao
    {
        string InsertSql(OrderReturnMasterQuery query, OrderMoneyReturn om);
        List<OrderMoneyReturnQuery> OrderMoneyReturnList(OrderMoneyReturnQuery query, out int totalCount);
        DataTable ExportATM(OrderMoneyReturnQuery query);
        DataTable ExportCARD(OrderMoneyReturnQuery query);
         int SaveOMReturn(OrderMoneyReturnQuery query);
         int SaveCSNote(OrderMoneyReturnQuery query);
    }
}
