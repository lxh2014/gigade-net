using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class OrderPaymentHitrustQuery : OrderPaymentHitrust
    {
        public DateTime createdate { set; get; }
        public string card_number { set; get; }
        public OrderPaymentHitrustQuery()
        {
            card_number = string.Empty;
        }
    
   
    }
}
