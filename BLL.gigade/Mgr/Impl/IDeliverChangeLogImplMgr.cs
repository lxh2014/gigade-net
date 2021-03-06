﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.APIModels;

namespace BLL.gigade.Mgr.Impl
{
    public interface IDeliverChangeLogImplMgr
    {
        int insertDeliverChangeLog(DeliverChangeLog dCL);
        List<DeliverChangeLogQuery> GetDeliverChangeLogList(DeliverChangeLogQuery Query, out int totalCount);
        bool Start(string schedule_code);
        string GetHtmlByDataTable(DataTable _dtmyMonth,string DeleteName=null,string value=null);
        bool isCanModifyExpertArriveDate(string apiServer, long deliver_id);
        bool ModifyExpertArriveDate(string apiServer, ModifyExpertArriveDateViewModel expertArriveDateViewModel);
    }
}
