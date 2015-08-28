using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class WebContentTypeSetupMgr : IWebContentTypeSetupImplMgr
    {
        private IWebContentTypeSetupImplDao _setUpDao;
        public WebContentTypeSetupMgr(string conStr)
        {
            _setUpDao = new WebContentTypeSetupDao(conStr);
        }
        public List<WebContentTypeSetup> Query(WebContentTypeSetup model)
        {
            try
            {
                return _setUpDao.Query(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentTypeSetupMgr-->Query-->" + ex.Message, ex);
            }
        }
        public DataTable QueryPageStore(WebContentTypeSetup model)
        {
            try
            {
                return _setUpDao.QueryPageStore(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentTypeSetupMgr-->QueryPageStore-->" + ex.Message, ex);
            }
        }
        public DataTable QueryAreaStore(WebContentTypeSetup model)
        {
            try
            {
                return _setUpDao.QueryAreaStore(model);
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentTypeSetupMgr-->QueryAreaStore-->" + ex.Message, ex);
            }
        }
    }
}
