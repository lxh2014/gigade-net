using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IEventPromoAmountFareImplMgr
    {
        List<Model.EventPromoAmountFare> GetList(Model.EventPromoAmountFare model, out int totalCount);
        string GetCondiType(int condiType, string event_id);
        bool SavePromoAmountFare(Model.EventPromoAmountFare epaFare, string condiType);
        bool UpdateActive(Model.EventPromoAmountFare model);
    }
}
