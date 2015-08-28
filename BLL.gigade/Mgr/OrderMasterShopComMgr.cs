using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class OrderMasterShopComMgr
    {
        private OrderMasterShopCom _MarketOrderDao;
        public OrderMasterShopComMgr(string connectionStr)
        {
            _MarketOrderDao = new OrderMasterShopCom(connectionStr);
        }


        public List<MarketOrderQuery> GetMarketOrderExcel(MarketOrderQuery q)
        {
            try
            {
                return _MarketOrderDao.GetMarketOrderExcel(q);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterShopComMgr-->GetMarketOrderExcel-->" + ex.Message, ex);
            }

        }

    }
}
