using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class IpoNvdMgr
    {
        private IpoNvdDao _IpoNvdDao;

        public IpoNvdMgr(string connectionString)
        {
            _IpoNvdDao = new IpoNvdDao(connectionString);
        }
        public List<IpoNvdQuery> GetIpoNvdList(IpoNvdQuery query, out int totalcount)
        {
            try
            {
                return _IpoNvdDao.GetIpoNvdList(query, out totalcount);
            }
            catch (Exception ex)
            {
                throw new Exception("IpoNvdMgr-->GetIpoNvdList-->" + ex.Message, ex);
            }
        }
        public int CreateTallyList(IpoNvdQuery query, string id)
        {
            try
            {
                return _IpoNvdDao.CreateTallyList(query,id);
            }
            catch (Exception ex)
            {
                throw new Exception("IpoNvdMgr-->CreateTallyList-->" + ex.Message, ex);
            }
        }
    }
}