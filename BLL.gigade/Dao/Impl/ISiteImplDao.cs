using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao.Impl
{
    interface ISiteImplDao
    {
        List<Site> Query(Site query);
        List<SiteQuery> QuerryAll(SiteQuery site, out int totalCount);
        int InsertSite(SiteModel site);
        int UpSite(SiteModel site);
        int UpSiteStatus(SiteModel site);

        List<Site> GetSite(Site query);
    }
}
