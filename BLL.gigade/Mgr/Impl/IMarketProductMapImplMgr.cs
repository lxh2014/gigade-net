using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl 
{
   public  interface IMarketProductMapImplMgr
    {
        DataTable GetMarketProductMapList(MarketProductMapQuery query, out int totalCount);
        int SavetMarketProductMap(MarketProductMapQuery query);
        int DeleteMarketProductMap(string row_id);
    }
}
