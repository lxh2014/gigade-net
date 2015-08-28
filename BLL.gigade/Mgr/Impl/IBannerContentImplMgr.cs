using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IBannerContentImplMgr
    {
        List<BannerContent> GetList(BannerContent bc, out int totalCount);
        int Add(BannerContent bc);
        int Update(BannerContent bc);
    }
}
