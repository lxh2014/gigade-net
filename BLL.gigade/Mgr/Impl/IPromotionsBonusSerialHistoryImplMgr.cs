using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IPromotionsBonusSerialHistoryImplMgr
    {
        List<PromotionsBonusSerialHistoryQuery> QueryById(int id);
        List<Model.Query.PromotionsBonusSerialHistoryQuery> QueryById(PromotionsBonusSerialHistoryQuery query, out int TotalCount);
    }
}
