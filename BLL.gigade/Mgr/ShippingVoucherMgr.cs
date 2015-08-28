using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
   public class ShippingVoucherMgr
    {
       ShippingVoucherDao dao;
       public ShippingVoucherMgr(string connectionStr)
       {
           dao = new ShippingVoucherDao(connectionStr);
       }
       public List<ShippingVoucherQuery> GetList(ShippingVoucherQuery query,out int total)
       {
           try
           {
               return dao.GetList(query,out  total);
           }
           catch (Exception ex)
           {
               throw new Exception("ShippingVoucherMgr-->GetList-->" + ex.Message, ex);
           }
       }
    }
}
