using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;


namespace BLL.gigade.Dao.Impl
{
   public  interface IMarketProductMapImplDao
    {
       DataTable GetMarketProductMapList(MarketProductMapQuery query, out int totalCount);
       string SavetMarketProductMap(MarketProductMapQuery query);
       string SelectProductMapCount(MarketProductMapQuery query);
       string DeleteMarketProductMap(string row_id);
    }
}
