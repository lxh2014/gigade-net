using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class PaymentType:PageBase
    {
       public string payment_name { get; set; }
       public string payment_code{get;set;}

       public PaymentType()
       {
           payment_code = "";
           payment_name = "";
       }
    }
}
