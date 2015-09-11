using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public class OrderPaymentNcccQuery:OrderPaymentNccc
    {
       public DateTime nccc_createdates { get; set; }
       public string pan_bankname { get; set; }
       public bool isSecret { get; set; }
       public OrderPaymentNcccQuery()
       {
           nccc_createdates = DateTime.MinValue;
           pan_bankname = string.Empty;
           isSecret = true;
       }
    }
}
