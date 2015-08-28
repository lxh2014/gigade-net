
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IMarketCategoryImplMgr
    {
        int MarketCategoryImport(DataRow[] dr);
        List<MarketCategoryQuery> GetMarketCategoryList(MarketCategory model, out int totalCount);
        List<MarketCategoryQuery> GetMarketCategoryList(MarketCategory model);
        int InsertMarketCategory(MarketCategory model);
        int UpdateMarketCategory(MarketCategory model);
        int DeleteMarketCategory(string cids);
    }
}
