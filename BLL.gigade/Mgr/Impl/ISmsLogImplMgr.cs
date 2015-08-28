using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface ISmsLogImplMgr
    {
        List<SmsLogQuery> GetSmsLog(SmsLogQuery slog, out int totalCount);
    }
}
