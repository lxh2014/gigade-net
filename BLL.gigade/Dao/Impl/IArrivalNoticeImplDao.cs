using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
   public interface IArrivalNoticeImplDao
    {
       List<ArrivalNoticeQuery> ArrivalNoticeList(ArrivalNoticeQuery query,out int totalCount);
       string IgnoreNotice(ArrivalNoticeQuery query);
    }
}
