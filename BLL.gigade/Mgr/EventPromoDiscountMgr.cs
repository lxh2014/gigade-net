using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
  public  class EventPromoDiscountMgr : IEventPromoDiscountImplMgr
    {
        private IEventPromoDiscountImplDao  IDiscountDao;
        private readonly string conn;
        public EventPromoDiscountMgr(string connectionStr)
        {
            IDiscountDao = new EventPromoDiscountDao(connectionStr);
            this.conn = connectionStr;
        }
        public List<Model.EventPromoDiscount> GetList(string event_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                return IDiscountDao.GetList(event_id);
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoDiscountMgr-->GetList-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
