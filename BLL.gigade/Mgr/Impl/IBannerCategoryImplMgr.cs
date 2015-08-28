using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IBannerCategoryImplMgr
    {
        List<BannerCategory> GetBannerCategoryList(BannerCategory bc, out int totalCount);
    }
}
