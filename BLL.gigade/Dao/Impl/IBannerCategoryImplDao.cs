using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    interface IBannerCategoryImplDao
    {
        List<BannerCategory> GetBannerCategoryList(BannerCategory bc,out int totalCount);
    }
}
