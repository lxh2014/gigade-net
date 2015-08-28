using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;
using System.Collections;

namespace BLL.gigade.Dao.Impl
{
    interface ITicketMasterImplDao
    {
        DataTable GetTicketMasterList(TicketMasterQuery tm, out int totalCount);
        DataTable GetCourseCountList(CourseQuery query, out int totalCount);
        int Update(TicketMaster tm);
        string CancelOrderTM(TicketMasterQuery query);
        string CancelOrderTD(TicketMasterQuery query);
        bool ExecSql(ArrayList arrList);
    }
} 
