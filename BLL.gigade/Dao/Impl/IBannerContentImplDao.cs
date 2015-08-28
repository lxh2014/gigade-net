using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface IBannerContentImplDao
    {
        List<BannerContent> GetList(BannerContent bc, out int totalCount);
        int Add(BannerContent bc);
        int Update(BannerContent bc);
    }
}
