using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class BrowseDataMgr:IBrowseDataImplMgr
    {
        private IBrowseDataImplDao _IBrowseDataDao;

        public BrowseDataMgr(string connString)
        {
            _IBrowseDataDao = new BrowseDataDao(connString);
        }

        public DataTable GetBrowseDataList(BrowseDataQuery query, out int totalCount)
        {
            try
            {
                return _IBrowseDataDao.GetBrowseDataList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("BrowseDataMgr-->GetBrowseDataList-->" + ex.Message, ex);
            }
        }
    }
}
