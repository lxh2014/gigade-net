using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public  class OrderPaymentHncbQuery: OrderPaymentHncb
    {
        public DateTime txtdate { set; get; }
        public OrderPaymentHncbQuery()
        {
        }
    }
}
