using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    interface IShopClassImplDao
    {
        List<ShopClass> QueryAll(ShopClass query);
        List<ShopClass> QueryStore(int classId = 0);
    }
}
 