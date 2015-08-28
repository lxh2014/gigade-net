using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;

namespace BLL.gigade.Mgr
{
   public class PaymentTypeMgr : IPaymentTypeImplMgr
    {
       private IPaymentImplDao _payimdo;
        public PaymentTypeMgr(string connectionString)
        {
            _payimdo = new PaymentTypeDao(connectionString);
        }

        public List<Model.PaymentType> Myfkfs()
        {
            return _payimdo.Myfkfs();
        }
    }
}
