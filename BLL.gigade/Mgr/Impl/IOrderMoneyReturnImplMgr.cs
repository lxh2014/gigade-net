using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
  public  interface IOrderMoneyReturnImplMgr
    {
      List<OrderMoneyReturnQuery> OrderMoneyReturnList(OrderMoneyReturnQuery query, out int totalCount);
      DataTable ExportATM(OrderMoneyReturnQuery query);
      DataTable ExportCARD(OrderMoneyReturnQuery query);
      int SaveOMReturn(OrderMoneyReturnQuery query);
      string SaveCSNote(OrderMoneyReturnQuery query);
    }
}
