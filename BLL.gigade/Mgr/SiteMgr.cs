using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class SiteMgr : Impl.ISiteImplMgr
    {
        Dao.Impl.ISiteImplDao _siteDao;
        public SiteMgr(string connectionString)
        {
            _siteDao = new SiteDao(connectionString);
        }

        public List<Site> Query(Site query)
        {
            try
            {
                return _siteDao.Query(query);
            }
            catch (Exception ex)
            {

                throw new Exception("SiteMgr-->Query-->" + ex.Message, ex);
            }

        }

        public List<SiteQuery> QuerryAll(SiteQuery site, out int totalCount) 
        {
            try
            {
                return _siteDao.QuerryAll(site,out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("SiteMgr-->QuerryAll-->" + ex.Message, ex);
            }
        }
        public int InsertSite(SiteModel site) 
        {
            try
            {
                return _siteDao.InsertSite(site);
            }
            catch (Exception ex)
            {

                throw new Exception("SiteMgr-->InsertSite-->" + ex.Message, ex);
            }
        }
        public int UpSite(SiteModel site) 
        {
            try
            {
                return _siteDao.UpSite(site);
            }
            catch (Exception ex)
            {

                throw new Exception("SiteMgr-->UpSite-->" + ex.Message, ex);
            }
        
        }
        public int UpSiteStatus(SiteModel site)
        {
            try
            {
                return _siteDao.UpSiteStatus(site);
            }
            catch (Exception ex)
            {

                throw new Exception("SiteMgr-->upSiteStatus-->" + ex.Message, ex);
            }
        }
        public List<Site> GetSite(Site query)
        {
            try
            {
                return _siteDao.GetSite(query);
            }
            catch (Exception ex)
            {
                throw new Exception("SiteMgr-->GetSite-->" + ex.Message, ex);
            }
        }
    }
}
