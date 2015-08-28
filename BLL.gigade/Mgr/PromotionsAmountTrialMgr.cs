using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class PromotionsAmountTrialMgr : IPromotionsAmountTrialImplMgr
    {
        private IPromotionsAmountTrialImplDao _pagDao;
        public PromotionsAmountTrialMgr(string connectionstring)
        {
            _pagDao = new PromotionsAmountTrialDao(connectionstring);
        }
         
        public List<Model.Query.PromotionsAmountTrialQuery> Query(Model.Query.PromotionsAmountTrialQuery query, out int totalCount)
        {
            try
            {
                return _pagDao.Query(query, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("PromotionsAmountTrialMgr-->Query-->" + ex.Message, ex);
            }
        }

        public int Save(Model.Query.PromotionsAmountTrialQuery query)
        {
            try
            {
                return _pagDao.Save(query);
            }
            catch (Exception ex)
            {

                throw new Exception("PromotionsAmountTrialMgr-->Save-->" + ex.Message, ex);
            }
        }

        public int Update(Model.Query.PromotionsAmountTrialQuery query)
        {
            try
            {
                return _pagDao.Update(query);
            }
            catch (Exception ex)
            {

                throw new Exception("PromotionsAmountTrialMgr-->Update-->" + ex.Message, ex);
            }
        }

        public int Delete(int id, string event_id)
        {
            throw new NotImplementedException();
        }

        public Model.Query.PromotionsAmountTrialQuery Select(int id)
        {
            try
            {
                return _pagDao.Select(id);
            }
            catch (Exception ex)
            {

                throw new Exception("PromotionsAmountTrialMgr-->Select-->" + ex.Message, ex);
            }
        }

        public int UpdateActive(Model.Query.PromotionsAmountTrialQuery model)
        {
            try
            {
                return _pagDao.UpdateActive(model);
            }
            catch (Exception ex)
            {

                throw new Exception("PromotionsAmountTrialMgr-->UpdateActive-->" + ex.Message, ex);
            }
        }
        public PromotionsAmountTrial GetModel(int id)
        {
            try
            {
                return _pagDao.GetModel(id);
            }
            catch (Exception ex)
            {

                throw new Exception("PromotionsAmountTrialMgr-->GetModel-->" + ex.Message, ex);
            }
        }
    }
}
