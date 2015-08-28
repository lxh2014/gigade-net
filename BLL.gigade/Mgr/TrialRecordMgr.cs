using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
namespace BLL.gigade.Mgr
{
    public class TrialRecordMgr : ITrialRecordImplMgr
    { 
        private ITrialRecordImplDao _ITrialRecordDao;
        public TrialRecordMgr(string connectionString)
        {
            _ITrialRecordDao = new TrialRecordDao(connectionString);
        }
        public DataTable GetShareList(Model.Query.TrialShareQuery query, out int totalCount)
        {
            try
            {
                return _ITrialRecordDao.GetShareList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("TrialRecordMgr-->GetShareList-->" + ex.Message, ex);
            }
        }
        public int TrialRecordUpdate(TrialRecordQuery query)
        {
            try
            {
                return _ITrialRecordDao.TrialRecordUpdate(query);
            }
            catch (Exception ex)
            {

                throw new Exception("TrialRecordMgr-->TrialRecordUpdate-->" + ex.Message, ex);
            }
        }
        public int TrialRecordSave(TrialShareQuery query)
        {
            try
            {
                return _ITrialRecordDao.TrialRecordSave(query);
            }
            catch (Exception ex)
            {
                
                throw new Exception("TrialRecordMgr-->TrialRecordSave-->" + ex.Message, ex);
            }
        }
        public TrialRecordQuery GetTrialRecordById(TrialRecordQuery query)
        {
            try
            {
                return _ITrialRecordDao.GetTrialRecordById(query);
            }
            catch (Exception ex)
            {

                throw new Exception("TrialRecordMgr-->GetTrialRecordById-->" + ex.Message, ex);
            }
        }
        public List<Model.Query.TrialRecordQuery> GetTrialRecordList(TrialRecordQuery store, out int totalCount)
        {
            try
            {
                return _ITrialRecordDao.GetTrialRecordList(store,out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("TrialRecordMgr-->GetTrialRecordList-->" + ex.Message, ex);
            }
        }


        public DataTable GetSumCount(PromotionsAmountTrialQuery query)
        {
            try
            {
                return _ITrialRecordDao.GetSumCount(query);
            }
            catch (Exception ex)
            {
                throw new Exception("TrialRecordMgr-->GetSumCount-->" + ex.Message, ex);
            }
        }
        public bool VerifyMaxCount(TrialRecordQuery query)
        {
            try
            {
                return _ITrialRecordDao.VerifyMaxCount(query);
            }
            catch (Exception ex)
            {
                throw new Exception("TrialRecordMgr-->VerifyMaxCount-->" + ex.Message, ex);
            }
        }
        public TrialShare GetTrialShare(TrialShare model)
        {
            try
            {
                return _ITrialRecordDao.GetTrialShare(model);
            }
            catch (Exception ex)
            {
                throw new Exception("TrialRecordMgr-->GetTrialShare-->" + ex.Message, ex);
            }
        }
    }
}
