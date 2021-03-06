﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IDeliverChangeLogImplDao
    {
        int insertDeliverChangeLog(DeliverChangeLog dCL);
        List<DeliverChangeLogQuery> GetDeliverChangeLogList(DeliverChangeLogQuery Query, out int totalCount);
        DataTable GetDeliverChangeLogDataTable(DeliverChangeLogQuery Query);
        DataTable GetExpectArriveDateByCreatetime(DeliverChangeLogQuery Query);
        DataTable GetDataTable(DeliverChangeLogQuery Query);
    }
}
