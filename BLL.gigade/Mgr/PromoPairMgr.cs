/*
 jialei0706h 
 datatime:20140922
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class PromoPairMgr : IPromoPairImplMgr
    {
        private IPromoPairImplDao _PPairDao;
        public PromoPairMgr(string connectionString)
        {
            _PPairDao = new PromoPairDao(connectionString);             
        }
        public int Save(PromoPair promopair, PromoPairQuery PPquery)
        {
            try
            {
                return _PPairDao.SaveTwo(promopair, PPquery);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairMgr-->Save-->" + ex.Message, ex);
            }
        }
        public int Update(PromoPair promopair, PromoPairQuery PPQuery)
        {
            try
            {
                return _PPairDao.Update(promopair, PPQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairMgr-->Update-->" + ex.Message, ex);
            }
        }
        public int Delete(Model.PromoPair promopair)
        {
            try
            {
                return _PPairDao.Delete(promopair);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairMgr-->Delete-->" + ex.Message, ex);
            }
        }
        public List<Model.Query.PromoPairQuery> Query(Model.Query.PromoPairQuery store, out int totalCount)
        {
            return _PPairDao.QueryAll(store, out totalCount);
        }
        public PromoPair GetMOdel(int id)
        {
            try
            {
                return _PPairDao.GetPPModel(id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairMgr-->GetModel-->" + ex.Message, ex);
            }
        }
        public int Save(PromoPair promopair)
        {
            try
            {
                return _PPairDao.SaveOne(promopair);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairMgr-->Save-->" + ex.Message, ex);
            }
         
        }
        public int SaveTwo(PromoPair promopair,PromoPairQuery ppQuery)
        {
            try
            {
                return _PPairDao.SaveTwo(promopair, ppQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairMgr-->Save-->" + ex.Message, ex);
            }
        }
        public DataTable Select(PromoPair Model)
        {
            try
            {
                return _PPairDao.Select(Model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairMgr-->Select-->" + ex.Message, ex);
            }
        }
        public DataTable SelCategoryID(PromoPair Model)
        {
            try
            {
                return _PPairDao.SelCategoryID(Model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairMgr-->SelCategoryID-->" + ex.Message, ex);
            }
        }
        public string CategoryID(PromoPair Model)
        {
            try
            {
                return _PPairDao.CategoryID(Model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairMgr-->CategoryID-->" + ex.Message, ex);
            }
        }
        public PromoPair GetModelById(int id)
        {
            try
            {
                return _PPairDao.GetModelById(id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairMgr-->GetModelById-->" + ex.Message, ex);
            }
        }
        public PromoPairQuery Select(int id)
        {
            try
            {
                return _PPairDao.Select(id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairMgr-->Select-->" + ex.Message, ex);
            }
        }

        public int UpdateActive(PromoPairQuery store)
        {
            try
            {
                return _PPairDao.UpdateActive(store);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoPairMgr-->UpdateActive-->" + ex.Message, ex);
            }
        }
    }
}
