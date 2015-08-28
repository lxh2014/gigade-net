using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
   public class PaymentTypeDao : IPaymentImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        public PaymentTypeDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        #region 獲取付款的方式
        public List<Model.PaymentType> Myfkfs()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select payment_name,payment_code from payment_type");
            return _access.getDataTableForObj<PaymentType>(sb.ToString());
        }
        #endregion
    }
}
