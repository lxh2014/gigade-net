using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class PaperClassMgr : IPaperClassImplMgr
    {
        private IPaperClassImplDao _paperClassDao;
        public PaperClassMgr(string connectionString)
        {
            _paperClassDao = new PaperClassDao(connectionString);
        }
        public List<PaperClass> GetPaperClassList(PaperClass pc, out int totalCount)
        {
            try
            {
                return _paperClassDao.GetPaperClassList(pc, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("PaperClassMgr-->GetPaperClassList" + ex.Message, ex);
            }
        }

        public int Add(PaperClass pc)
        {
            try
            {
                return _paperClassDao.Add(pc);
            }
            catch (Exception ex)
            {
                throw new Exception("PaperClassMgr-->Add" + ex.Message, ex);
            }
        }

        public int Update(PaperClass pc)
        {
            try
            {
                return _paperClassDao.Update(pc);
            }
            catch (Exception ex)
            {
                throw new Exception("PaperClassMgr-->Update" + ex.Message, ex);
            }
        }

        public int UpdateClassID(PaperClass pc)
        {
            try
            {
                return _paperClassDao.UpdateClassID(pc);
            }
            catch (Exception ex)
            {
                throw new Exception("PaperClassMgr-->UpdateClassID" + ex.Message, ex);
            }
        }
        public int UpdateState(string id, int status)
        {
            try
            {
                return _paperClassDao.UpdateState(id, status);
            }
            catch (Exception ex)
            {
                throw new Exception("PaperClassMgr-->UpdateState" + ex.Message, ex);
            }
        }
        public int Delete(PaperClass pc)
        {
            try
            {
                return _paperClassDao.Delete(pc);
            }
            catch (Exception ex)
            {
                throw new Exception("PaperClassMgr-->Delete" + ex.Message, ex);
            }
        }
    }
}
