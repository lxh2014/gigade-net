using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IPriceUpdateApplyHistoryImplMgr
    {
        bool Save(List<PriceUpdateApplyHistory> pHList);
        string SaveSql(PriceUpdateApplyHistory priceUpdateApplyHistory);
    }
}
