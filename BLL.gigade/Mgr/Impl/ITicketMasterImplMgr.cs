using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Mgr.Impl
{
    public interface ITicketMasterImplMgr
    {
        DataTable GetTicketMasterList(TicketMasterQuery tm, out int totalCount);
        DataTable GetCourseCountList(CourseQuery query, out int totalCount);
        string Update(TicketMaster tm);
        bool CancelOrder(List<TicketMasterQuery> list);
        string ExecCancelOrder(List<TicketMasterQuery> list);
       
    }
}
 