using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class BannerNewsSiteMgr
    {
        private BannerNewsSiteDao _BannerDao;
        public BannerNewsSiteMgr(string connectionStr)
        {
            _BannerDao = new BannerNewsSiteDao(connectionStr);
        }
        public List<BannerNewsSiteQuery> GetList(BannerNewsSiteQuery q, out int totalCount)
        {
            try
            {
                return _BannerDao.GetList(q, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("BannerNewsSiteMgr-->GetList-->" + ex.Message, ex);
            }
        }
        public List<BannerNewsSite> GetBannerNewsSiteName(BannerNewsSite bs)
        {
            try
            {
                return _BannerDao.GetBannerNewsSiteName(bs);
            }
            catch (Exception ex)
            {
                throw new Exception("BannerNewsSiteMgr-->GetBannerNewsSiteName-->" + ex.Message, ex);
            }
        }
    }
}
