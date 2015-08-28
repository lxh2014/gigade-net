/* 
* 文件名稱 :SitePageMgr.cs 
* 文件功能描述 :頁面表數據操作 
* 版權宣告 : 
* 開發人員 : shiwei0620j 
* 版本資訊 : 1.0 
* 日期 : 2014/10/14 
* 修改人員 : 
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class SitePageMgr : ISitePageImplMgr
    {
        private ISitePageImplDao _BannerPageDao;
        public SitePageMgr(string connectionString)
        {
            _BannerPageDao = new SitePageDao(connectionString);
        }

        #region SitePage列表頁 +GetSitePageList(SitePageQuery store, out int totalCount)
        public List<SitePageQuery> GetSitePageList(SitePageQuery store, out int totalCount)
        {
            try
            {
                return _BannerPageDao.GetSitePageList(store, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("SitePageMgr-->GetBannerPageList-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 查詢站點下的所有網頁+ List<BannerPage> GetPage(BannerPage bp)
        /// <summary>
        /// 查詢站點下的所有網頁
        /// </summary>
        /// <param name="bp"></param>
        /// <returns></returns>
        public List<SitePage> GetPage(SitePage bp)
        {
            try
            {
                return _BannerPageDao.GetPage(bp);
            }
            catch (Exception ex)
            {

                throw new Exception("SitePageMgr-->GetPage-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 新增數據
        public int Save(SitePageQuery model)
        {
            try
            {
                return _BannerPageDao.Save(model);
            }
            catch (Exception ex)
            {

                throw new Exception("SitePageMgr-->Save-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 更新數據
        public int Update(SitePageQuery model)
        {
            try
            {
                return _BannerPageDao.Update(model);
            }
            catch (Exception ex)
            {

                throw new Exception("SitePageMgr-->Update-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 更改頁面狀態 +UpdateStatus(SitePageQuery query)
      public int UpdateStatus(SitePageQuery query)
      {
          try
          {
              return _BannerPageDao.UpdateStatus(query);
          }
          catch (Exception ex)
          {
              throw new Exception("SitePageMgr-->UpdateStatus-->" + ex.Message, ex);
          }
      }
        #endregion
       
    }
}
