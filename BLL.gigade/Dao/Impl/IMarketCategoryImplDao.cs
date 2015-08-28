using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;


namespace BLL.gigade.Dao.Impl
{
    public interface IMarketCategoryImplDao
    {
        string InsertMarketCategory(MarketCategory model);
        List<MarketCategoryQuery> GetMarketCategoryList(MarketCategory model, out int totalCount);
        string UpdateMarketCategory(MarketCategory model);
        List<MarketCategoryQuery> GetMarketCategoryList(MarketCategory model);
        string DeleteMarketCategory(int cid);
    }
}
