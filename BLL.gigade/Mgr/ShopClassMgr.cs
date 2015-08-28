using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class ShopClassMgr : IShopClassImplMgr
    {
        private IShopClassImplDao _shopClassDao;
        public ShopClassMgr(string connectionString)
        {
            _shopClassDao = new ShopClassDao(connectionString);
        }
        public List<ShopClass> QueryAll(ShopClass query)
        {
            return _shopClassDao.QueryAll(query);
        }

        public List<ShopClass> QueryStore()
        {
            return _shopClassDao.QueryStore();
        }
    }
}
