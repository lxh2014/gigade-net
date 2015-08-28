/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：SiteConfigMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/9/12 13:50:59 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class SiteConfigMgr : ISiteConfigImplMgr
    {
        private ISiteConfigImplDao _siteConfigDao;
        public SiteConfigMgr(string xmlPath)
        {
            _siteConfigDao = new SiteConfigDao(xmlPath);
        }

        #region ISiteConfigImplMgr 成员

        public List<SiteConfig> Query()
        {
            try
            {
                return _siteConfigDao.Query();
            }
            catch (Exception ex)
            {

                throw new Exception("SiteConfigMgr-->Query-->" + ex.Message, ex);
            }

        }

        public SiteConfig GetConfigByName(string configName)
        {
            try
            {
                return _siteConfigDao.GetConfigByName(configName);
            }
            catch (Exception ex)
            {

                throw new Exception("SiteConfigMgr-->GetConfigByName-->" + ex.Message, ex);
            }

        }

        public bool UpdateNode(SiteConfig newConfig)
        {
            try
            {
                return _siteConfigDao.UpdateNode(newConfig);
            }
            catch (Exception ex)
            {

                throw new Exception("SiteConfigMgr-->UpdateNode-->" + ex.Message, ex);
            }

        }

        #endregion
    }
}
