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
    public class CbjobMasterMgr : ICbjobMasterImplMgr
    {
        private ICbjobMasterImplDao _cbjobDao;
        public CbjobMasterMgr(string connectionString)
        {
            _cbjobDao = new CbjobMasterDao(connectionString);
        }

        public int Insert(CbjobMaster m)
        {
            try
            { 
                return _cbjobDao.Insert(m);
            }
            catch (Exception ex)
            {
                throw new Exception("CbjobMasterMgr-->Insert-->" + ex.Message, ex);
            }
        }
        public string Insertsql(CbjobMaster m)
        {
            try
            {
                return _cbjobDao.Insertsql(m);
            }
            catch (Exception ex)
            {
                throw new Exception("CbjobMasterMgr-->Insertsql-->" + ex.Message, ex);
            }
        }
        public List<CbjobMasterQuery> GetjobMaster(CbjobMasterQuery m, out int totalCount)
        {
            try
            {
                return _cbjobDao.GetjobMaster(m, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("CbjobMasterMgr-->GetjobMaster-->" + ex.Message, ex);
            }
        }
    }
}
