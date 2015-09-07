using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class EventPromoGiftMgr : IEventPromoGiftImplMgr
    {

        private IEventPromoGiftImplDao _iepGiftDao;
        private string conn;
        public EventPromoGiftMgr(string connectionStr)
        {
            _iepGiftDao = new EventPromoGiftDao(connectionStr);
            this.conn = connectionStr;
        }
        public List<Model.Query.EventPromoGiftQuery> GetList(string event_id)
        {
            try
            {
                List<Model.Query.EventPromoGiftQuery> stores = _iepGiftDao.GetList(event_id);
                foreach (Model.Query.EventPromoGiftQuery item in stores)
                {
                    if (item.gift_type == 1)
                    {
                        ProductMgr _pDao = new ProductMgr(conn);
                        if (item.product_id != 0)
                        {
                            item.product_name = _pDao.GetNameForID(item.product_id, 0, 0, 1);
                        }
                    }
                }

                return stores;
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoGiftMgr-->GetList-->" + ex.Message, ex);
            }
        }
    }
}
