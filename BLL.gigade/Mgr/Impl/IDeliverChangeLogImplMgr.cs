using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IDeliverChangeLogImplMgr
    {
        int insertDeliverChangeLog(DeliverChangeLog dCL);
        List<DeliverChangeLogQuery> GetDeliverChangeLogList(DeliverChangeLogQuery Query, out int totalCount);
    }
}
