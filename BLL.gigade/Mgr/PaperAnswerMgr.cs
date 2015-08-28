using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class PaperAnswerMgr : IPaperAnswerImplMgr
    {
        private IPaperAnswerImplDao _paperAnswerDao;
        public PaperAnswerMgr(string connectionString)
        {
            _paperAnswerDao = new PaperAnswerDao(connectionString);
        }

        public List<PaperAnswer> GetPaperAnswerList(PaperAnswer pa, out int totalCount)
        {
            try
            {
                return _paperAnswerDao.GetPaperAnswerList(pa, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("PaperAnswerMgr-->GetPaperAnswerList" + ex.Message, ex);
            }
        }

        public DataTable Export(PaperAnswer pa)
        {
            try
            {
                return _paperAnswerDao.Export(pa);
            }
            catch (Exception ex)
            {
                throw new Exception("PaperAnswerMgr-->Export" + ex.Message, ex);
            }
        }
        public DataTable GetPaperClassID(PaperClass pc)
        {
            try
            {
                return _paperAnswerDao.GetPaperClassID(pc);
            }
            catch (Exception ex)
            {
                throw new Exception("PaperAnswerMgr-->GetPaperClassID" + ex.Message, ex);
            }
        }
        public DataTable GetPaperAnswerUser(PaperAnswer pa)
        {
            try
            {
                return _paperAnswerDao.GetPaperAnswerUser(pa);
            }
            catch (Exception ex)
            {
                throw new Exception("PaperAnswerMgr-->GetPaperAnswerUser" + ex.Message, ex);
            }
        }
        public DataTable ExportSinglePaperAnswer(PaperAnswer pa)
        {
            try
            {
                return _paperAnswerDao.Export(pa);
            }
            catch (Exception ex)
            {
                throw new Exception("PaperAnswerMgr-->ExportSinglePaperAnswer" + ex.Message, ex);
            }
        }
    }
}
