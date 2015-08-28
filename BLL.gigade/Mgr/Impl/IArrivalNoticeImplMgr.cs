using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
   public interface IArrivalNoticeImplMgr
    {
       List<ArrivalNoticeQuery> ArrivalNoticeList(ArrivalNoticeQuery query, out int totalCount);
       bool IgnoreNotice(List<ArrivalNoticeQuery> list);
    }
}
