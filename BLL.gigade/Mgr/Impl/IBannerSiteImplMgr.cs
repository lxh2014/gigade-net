using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IBannerSiteImplMgr
    {
        List<BannerSite> GetList(BannerSite bs, out int totalCount);
        List<BannerSite> GetBannerSiteName(BannerSite bs);
    }
}
