using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class VoteArticleMgr
    {
        private VoteArticleDao _votearticleDao;
        public VoteArticleMgr(string connectionStr)
        {
            _votearticleDao = new VoteArticleDao(connectionStr);
        }
        public List<VoteArticleQuery> GetAll(VoteArticleQuery query, out int totalCount)
        {
            try
            {
                return _votearticleDao.GetAll(query,out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteArticleMgr-->GetAll-->" + ex.Message, ex);
            }
        }
        public int Save(VoteArticleQuery m)
        {
            try
            {
                return _votearticleDao.Save(m);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteArticleMgr-->UpdateStatus-->" + ex.Message, ex);
            }
        }
        public int Update(VoteArticleQuery m)
        {
            try
            {
                return _votearticleDao.Update(m);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteArticleMgr-->UpdateStatus-->" + ex.Message, ex);
            }
        }
        public int UpdateStatus(VoteArticleQuery m)
        {
            try
            {
                return _votearticleDao.UpdateStatus(m);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteArticleMgr-->UpdateStatus-->" + ex.Message, ex);
            }
        }
        public List<VoteArticleQuery> GetArticle()
        {
            try
            {
                return _votearticleDao.GetArticle();
            }
            catch (Exception ex)
            {
                throw new Exception("VoteArticleMgr-->GetArticle-->" + ex.Message, ex);
            }
        }
        public int SelectByArticleName(VoteArticleQuery m)
        {
            try
            {
                return _votearticleDao.SelectByArticleName(m);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteArticleMgr-->SelectByArticleName-->" + ex.Message, ex);
            }
        }

        public int SelMaxSort(VoteArticleQuery query)
        {
            try
            {
                return _votearticleDao.SelMaxSort(query);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteArticleMgr-->SelMaxSort-->" + ex.Message, ex);
            }
        }
    }
}