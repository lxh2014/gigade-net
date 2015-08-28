using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Mgr.Impl;

namespace BLL.gigade.Mgr
{
    public class BannerSiteMgr : IBannerSiteImplMgr
    {
        private IBannerSiteImplDao _bsDao;
        public BannerSiteMgr(string connectionStr)
        {
            _bsDao = new BannerSiteDao(connectionStr);
        }
        public List<BannerSite> GetList(BannerSite bs, out int totalCount)
        {
            try
            {
                return _bsDao.GetList(bs, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("BannerSiteMgr-->GetList" + ex.Message, ex);
            }
        }
        public List<BannerSite> GetBannerSiteName(BannerSite bs)
        {
            try
            {
                return _bsDao.GetBannerSiteName(bs);
            }
            catch (Exception ex)
            {
                throw new Exception("BannerSiteMgr-->GetBannerSiteName" + ex.Message, ex);
            }
        }
    }
}
