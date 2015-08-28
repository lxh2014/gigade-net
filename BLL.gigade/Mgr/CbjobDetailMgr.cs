using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class CbjobDetailMgr : ICbjobDetailImplMgr
    {
        private ICbjobDetailImplDao _cbjobDao;
        public CbjobDetailMgr(string connectionString)
        {
            _cbjobDao = new CbjobDetailDao(connectionString);
        }

        public List<Model.Query.CbjobDetailQuery> GetMessage(Model.Query.CbjobDetailQuery cbjobQuery, out int totalCount)
        {
            try
            {
                return _cbjobDao.GetMessage(cbjobQuery, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("CbjobDetailMgr-->GetMessage-->" + ex.Message, ex);
            }
        }

        public int DeleteCbjobmessage(Model.Query.CbjobDetailQuery cbjobQuery)
        {
            try
            {
                return _cbjobDao.DeleteCbjobmessage(cbjobQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("CbjobDetailMgr-->DeleteCbjobmessage-->" + ex.Message, ex);
            }
        }

        public int UpdateCbjobMaster(Model.Query.CbjobDetailQuery cbjobQuery)
        {
            try
            {
                return _cbjobDao.UpdateCbjobMaster(cbjobQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("CbjobDetailMgr-->UpdateCbjobMaster-->" + ex.Message, ex);
            }
        }

        public int UpdateCbjobstaid(Model.Query.CbjobDetailQuery cbjobQuery)
        {
            try
            {
                return _cbjobDao.UpdateCbjobstaid(cbjobQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("CbjobDetailMgr-->UpdateCbjobstaid-->" + ex.Message, ex);
            }
        }


        public int FupanComplete(Model.Query.CbjobDetailQuery cbjobQuery)
        {
            try
            {
                return _cbjobDao.FupanComplete(cbjobQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("CbjobDetailMgr-->FupanComplete-->" + ex.Message, ex);
            }
        }

        public string insertsql(CbjobDetail cb)
        {
            try
            {
                return _cbjobDao.insertsql(cb);
            }
            catch (Exception ex)
            {
                throw new Exception("CbjobDetailMgr-->insertsql-->" + ex.Message, ex);
            }
        }

        public int InsertSql(string sql)
        {
            try
            {
                return _cbjobDao.InsertSql(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("CbjobDetailMgr-->InsertSql-->" + ex.Message, ex);
            }
        }
    }
}
