using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IArrivalNoticeImplMgr
    {
        List<ArrivalNoticeQuery> ArrivalNoticeList(ArrivalNoticeQuery query, out int totalCount);
        bool IgnoreNotice(List<ArrivalNoticeQuery> list);
        List<ArrivalNoticeQuery> GetArrNoticeList(ArrivalNoticeQuery query, out int totalCount);
        List<ArrivalNoticeQuery> ShowArrByUserList(ArrivalNoticeQuery query, out int totalCount);
        int SaveArrivaleNotice(ArrivalNotice query);
        int UpArrivaleNoticeStatus(ArrivalNotice query);
    }
}
