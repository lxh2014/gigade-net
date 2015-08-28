using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface IBannerSiteImplDao
    {
        List<BannerSite> GetList(BannerSite bs, out int totalCount);
        List<BannerSite> GetBannerSiteName(BannerSite bs);
    }
}
