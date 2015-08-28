using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface INewPromoRecordImplMgr
    {
        List<NewPromoRecordQuery> NewPromoRecordList(NewPromoRecordQuery query, out int totalCount);
    }
}
