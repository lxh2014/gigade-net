using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class OrderPaymentNcccMgr : IOrderPaymentNcccImplMgr
    {
       private IOrderPaymentNcccImplDao _IorderpaymentDao;
       public OrderPaymentNcccMgr(string connectionString)
        {
            _IorderpaymentDao = new OrderPaymentNcccDao(connectionString);
        }

       public DataTable OrderPaymentNccc(SinopacDetailQuery store, string sql)
        {
            try
            {
                return _IorderpaymentDao.OrderPaymentNccc(store,sql);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderPaymentNcccMgr-->OrderPaymentNccc-->" + ex.Message, ex);
            }
        }
    }
}
