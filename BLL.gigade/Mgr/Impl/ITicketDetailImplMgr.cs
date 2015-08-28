using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace BLL.gigade.Mgr.Impl
{
    public interface ITicketDetailImplMgr
    {
        DataTable GetTicketDetailTable(TicketDetailQuery query, out int totalCount);
        int UpdateTicketStatus(string RowId);
        bool UpLoadFile(HttpPostedFileBase httpPostedFile, string serverPath, string fileName, string extensions, int maxSize, int minSize, ref string ErrorMsg, string ftpUser, string ftpPasswd);
    
        DataTable GetTicketDetailAllCodeTable(TicketDetailQuery query, out int totalCount);
    }
}
 