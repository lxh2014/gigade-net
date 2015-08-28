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
    public class BannerContentMgr:IBannerContentImplMgr
    {
        private IBannerContentImplDao _bcDao;
        public BannerContentMgr(string connectionStr)
        {
            _bcDao = new BannerContentDao(connectionStr);
        }
        public List<BannerContent> GetList(BannerContent bc, out int totalCount)
        {
            try
            {
                return _bcDao.GetList(bc, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("BannerContentMgr-->GetList", ex);
            }
        }
        public int Add(BannerContent bc)
        {
            try
            {
                return _bcDao.Add(bc);
            }
            catch (Exception ex)
            {
                throw new Exception("BannerContentMgr-->Add", ex);
            }
        }
        public int Update(BannerContent bc)
        {
            try
            {
                return _bcDao.Update(bc);
            }
            catch (Exception ex)
            {
                throw new Exception("BannerContentMgr-->Update", ex);
            }
        }

    }
}
