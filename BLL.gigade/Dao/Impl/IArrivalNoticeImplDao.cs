using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
   public interface IArrivalNoticeImplDao
    {
       List<ArrivalNoticeQuery> ArrivalNoticeList(ArrivalNoticeQuery query,out int totalCount);
       string IgnoreNotice(ArrivalNoticeQuery query);
       List<ArrivalNoticeQuery> GetArrNoticeList(ArrivalNoticeQuery query, out int totalCount);
       List<ArrivalNoticeQuery> ShowArrByUserList(ArrivalNoticeQuery query, out int totalCount);
       int SaveArrivaleNotice(ArrivalNotice query);
       int UpArrivaleNoticeStatus(ArrivalNotice query);
       Dictionary<int, int> GetNoticeGoods(ArrivalNotice query);
    }
}
