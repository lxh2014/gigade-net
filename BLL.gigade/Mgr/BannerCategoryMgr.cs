using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;

namespace BLL.gigade.Mgr
{
    public class BannerCategoryMgr : IBannerCategoryImplMgr
    {
        private IBannerCategoryImplDao _bcDao;
        public BannerCategoryMgr(string connectionString)
        {
            _bcDao = new BannerCategoryDao(connectionString);
        }
        public List<BannerCategory> GetBannerCategoryList(BannerCategory bc, out int totalCount)
        {
            try
            {
                return _bcDao.GetBannerCategoryList(bc, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("BannerCategoryMgr-->GetBannerCategoryList" + ex.Message, ex);
            }
        }
    }
}
