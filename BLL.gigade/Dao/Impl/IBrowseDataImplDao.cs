using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IBrowseDataImplDao
    {
        DataTable GetBrowseDataList(BrowseDataQuery query, out int totalCount);
    }
}
