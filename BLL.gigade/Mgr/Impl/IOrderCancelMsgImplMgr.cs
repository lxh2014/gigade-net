using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Mgr.Impl
{
    public interface IOrderCancelMsgImplMgr
    {
        List<OrderCancelMsgQuery> Query(OrderCancelMsgQuery ocm, out int totalCount);
        int Reply(OrderCancelResponse ocr);
    }
}
