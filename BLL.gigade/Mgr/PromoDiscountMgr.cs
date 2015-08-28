using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class PromoDiscountMgr : IPromoDiscountImplMgr
    {
        IPromoDiscountImplDao _promoDisDao;
        public PromoDiscountMgr(string connectionStr)
        {
            _promoDisDao = new PromoDiscountDao(connectionStr);
        }
        #region IPromoDiscountImplMgr 成员

        public List<Model.PromoDiscount> GetPromoDiscount(PromoDiscount model)
        {
            return _promoDisDao.GetPromoDiscount(model);
        }


        public int Save(Model.PromoDiscount model)
        {
            try
            {
                return _promoDisDao.Save(model);
            }
            catch (Exception ex)
            {

                throw new Exception("PromoDiscountMgr-->Save-->" + ex.Message, ex);
            }

        }
        public int Update(Model.PromoDiscount model)
        {
            try
            {
                return _promoDisDao.Update(model);
            }
            catch (Exception ex)
            {

                throw new Exception("PromoDiscountMgr-->Update-->" + ex.Message, ex);
            }

        }

        public DataTable GetLimitByEventId(string event_id,int rid)
        {
            try
            {
                return _promoDisDao.GetLimitByEventId(event_id,rid);
            }
            catch (Exception ex)
            {

                throw new Exception("PromoDiscountMgr-->GetLimitByEventId-->" + ex.Message, ex);
            }


        }
        public int DeleteByRid(int rid)
        {
            try
            {
                return _promoDisDao.DeleteByRid(rid);
            }
            catch (Exception ex)
            {

                throw new Exception("PromoDiscountMgr-->DeleteByRid-->" + ex.Message, ex);
            }

        }

        public int DeleteByEventid(PromoDiscount model)
        {
            try
            {
                return _promoDisDao.DeleteByEventid(model);
            }
            catch (Exception ex)
            {

                throw new Exception("PromoDiscountMgr-->DeleteByEventid-->" + ex.Message, ex);
            }

        }

        #endregion
    }
}
