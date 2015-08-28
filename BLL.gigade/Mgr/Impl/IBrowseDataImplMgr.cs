using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IBrowseDataImplMgr
    {
        DataTable GetBrowseDataList(BrowseDataQuery query, out int totalCount);
    }
}
