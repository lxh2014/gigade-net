using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using System.Data;
namespace BLL.gigade.Mgr.Impl
{
    public interface ITicketImplMgr
    {
        List<TicketQuery> GetTicketList(TicketQuery tqQuery, out int totalCount, string condition);
        List<TicketQuery> GetPickingDetail(TicketQuery query);
        List<TicketQuery> GetTicketDetail(TicketQuery query);
        DataTable GetOrderDelivers(TicketQuery query);
        int UpdateTicketStatus(TicketQuery query);
    }
}
