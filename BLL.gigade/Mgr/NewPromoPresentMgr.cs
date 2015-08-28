using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using DBAccess;

namespace BLL.gigade.Mgr
{
    public class NewPromoPresentMgr : INewPromoPresentImplMgr
    {
        private INewPromoPresentImplDao _newPresentDao;
        private IDBAccess _access;
        private string connStr;

        public NewPromoPresentMgr(string connectionStr)
        {
            _newPresentDao = new NewPromoPresentDao(connectionStr);
            _access = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }


        public DataTable NewPromoPresentList(Model.Query.NewPromoPresentQuery query, out int totalCount)
        {
            try
            {
                return _newPresentDao.GetNewPromoPresentList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoPresentMgr-->NewPromoPresentList-->" + ex.Message, ex);
            }
        }


        public int InsertNewPromoPresent(Model.Query.NewPromoPresentQuery newPresent)
        {
            try
            {
                return _access.execCommand(_newPresentDao.InsertNewPromoPresent(newPresent));
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoPresentMgr-->InsertNewPromoPresent-->" + ex.Message, ex);
            }
        }


        public string GetProductnameById(int id)
        {
            try
            {
                return _newPresentDao.GetProductnameById(id);
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoPresentMgr-->GetProductnameById-->" + ex.Message, ex);
            }
        }


        public int UpdateNewPromoPresent(Model.Query.NewPromoPresentQuery newPresent)
        {
            try
            {
                return _access.execCommand(_newPresentDao.UpdateNewPromoPresent(newPresent));
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoPresentMgr-->UpdateNewPromoPresent-->" + ex.Message, ex);
            }
        }


        public int UpdateActive(Model.Query.NewPromoPresentQuery newPresent)
        {
            try
            {
                return _access.execCommand(_newPresentDao.UpdateActive(newPresent));
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoPresentMgr-->UpdateActive-->" + ex.Message, ex);
            }
        }


        public int DeleteNewPromoPresent(Model.Query.NewPromoPresentQuery newPresent)
        {
            try
            {
                return _access.execCommand(_newPresentDao.DeleteNewPromoPresent(newPresent));
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoPresentMgr-->DeleteNewPromoPresent-->" + ex.Message, ex);
            }
        }

        public int GetNewPromoPresent(string event_id)
        {
            try
            {
                return _newPresentDao.GetNewPromoPresent(event_id);
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoPresentMgr-->GetNewPromoPresent-->" + ex.Message, ex);
            }
        }


        public int GetNewPromoPresentMaxId()
        {
            try
            {
                return _newPresentDao.GetNewPromoPresentMaxId();
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoPresentMgr-->GetNewPromoPresentMaxId-->" + ex.Message, ex);
            }
        }
    }
}
