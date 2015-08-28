
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using System.Data;


namespace BLL.gigade.Mgr
{
    public class PromoTicketMgr : IPromoTicketImplMgr
    {
        private IPromoTicketImplDao _promoTicketDao;
        public PromoTicketMgr(string connectionString)
        {
            _promoTicketDao = new PromoTicketDao(connectionString);
        }
        public int Save(PromoTicket model)
        {
            try
            {
                return _promoTicketDao.Save(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotcketMgr-->Save-->" + ex.Message, ex);
            }
        }

        public int Update(PromoTicket model)
        {
            try
            {
                return _promoTicketDao.Update(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotcketMgr-->Update-->" + ex.Message, ex);
            }
        }
        public int Delete(int rid)
        {
            try
            {
                return _promoTicketDao.Delete(rid);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotcketMgr-->Delete-->" + ex.Message, ex);
            }
        }
        public PromoTicket Query(int rid)
        {
            try
            {
                return _promoTicketDao.Query(rid);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotcketMgr-->Query-->" + ex.Message, ex);
            }
        }
    }
}
