using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class HappyGoMgr : IHappyGoImplMgr
    {
        private IHappyGoImplDao _IHappyGoDao;

        public HappyGoMgr(string connectionString)
        {
            _IHappyGoDao = new HappyGoDao(connectionString);
        }

        public List<HgDeduct> GetHGDeductList(uint order_id)
        {
            try
            {
                return _IHappyGoDao.GetHGDeductList(order_id);
            }
            catch (Exception ex)
            {

                throw new Exception("HappyGoMgr-->GetHGDeductList-->" + ex.Message, ex);
            }
        }


        public  List<HgAccumulate> GetHGAccumulateList(uint order_id)
        {
            try
            {
                return _IHappyGoDao.GetHGAccumulateList(order_id);
            }
            catch (Exception ex)
            {

                throw new Exception("HappyGoMgr-->GetHGAccumulateList-->" + ex.Message, ex);
            }
        }

        public  List<HgAccumulateRefund> GetHGAccumulateRefundList(uint order_id)
        {
            try
            {
                return _IHappyGoDao.GetHGAccumulateRefundList(order_id);
            }
            catch (Exception ex)
            {

                throw new Exception("HappyGoMgr-->GetHGAccumulateRefundList-->" + ex.Message, ex);
            }
        }

        public  List<HgDeductRefund> GetHgDeductRefundList(uint order_id)
        {
            try
            {
                return _IHappyGoDao.GetHgDeductRefundList(order_id);
            }
            catch (Exception ex)
            {

                throw new Exception("HappyGoMgr-->GetHgDeductRefundList-->" + ex.Message, ex);
            }
        }

        public  List<HgDeductReverse> GetHgDeductReverseList(uint order_id)
        {
            try
            {
                return _IHappyGoDao.GetHgDeductReverseList(order_id);
            }
            catch (Exception ex)
            {

                throw new Exception("HappyGoMgr-->GetHgDeductReverseList-->" + ex.Message, ex);
            }
        }
    }
}
