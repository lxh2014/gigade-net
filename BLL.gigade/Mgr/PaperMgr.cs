using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;

namespace BLL.gigade.Mgr
{
    public class PaperMgr : IPaperImplMgr
    {
        private IPaperImplDao _paperDao;
        public PaperMgr(string connectionString)
        {
            _paperDao = new PaperDao(connectionString);
        }
        public List<Model.Paper> GetPaperList(Model.Paper p, out int totalCount)
        {
            try
            {
                return _paperDao.GetPaperList(p, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("PaperMgr-->GetPaperList" + ex.Message, ex);
            }
        }

        public int Add(Model.Paper p)
        {
            try
            {
                return _paperDao.Add(p);
            }
            catch (Exception ex)
            {
                throw new Exception("PaperDao-->Add" + ex.Message, ex);
            }
        }

        public int Update(Model.Paper p)
        {
            try
            {
                return _paperDao.Update(p);
            }
            catch (Exception ex)
            {
                throw new Exception("PaperDao-->Update" + ex.Message, ex);
            }
        }
        public int UpdateState(Model.Paper p)
        {
            try
            {
                return _paperDao.UpdateState(p);
            }
            catch (Exception ex)
            {
                throw new Exception("PaperDao-->UpdateState" + ex.Message, ex);
            }
        }
    }
}
