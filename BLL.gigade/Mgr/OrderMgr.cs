using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class OrderMgr : IOrderImplMgr
    {

          private IOrderImplDao _orderDao;
          public OrderMgr(string connectionString)
        {
            _orderDao = new OrderDao(connectionString);
        }
          public bool ThingsMethod(string[] rows, OrderDeliver order, OrderSlaveMaster master, string Descriptions) 
          {
              try
              {
                  return _orderDao.ThingsMethod(rows, order, master, Descriptions);
              }
              catch (Exception ex)
              {
                  throw new Exception("OrderMgr-->ThingsMethod-->" + ex.Message, ex);
              }
          }
          public bool SelfThingsMethod(DataTable _dtSms, OrderDeliver query, string Descriptions)
          {
              try
              {
                  return _orderDao.SelfThingsMethod(_dtSms, query, Descriptions);
              }
              catch (Exception ex)
              {
                  throw new Exception("OrderMgr-->SelfThingsMethod-->" + ex.Message, ex);
              }
          }
    }
}
