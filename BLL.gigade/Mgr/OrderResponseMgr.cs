using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class OrderResponseMgr : IOrderResponseIMgr
    {
        private IOrderResponseIDao _ordao;

        public OrderResponseMgr(string connectionString)
    {
            _ordao = new OrderResponseDao(connectionString);
        }

        public int insert(Model.OrderResponse or)
        {
            try
            {
                return _ordao.insert(or);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderQuestionMgr-->insert-->" + ex.Message, ex);
            }
        }
    }
}
