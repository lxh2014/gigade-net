using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public class OrderPaymentNcccQuery:OrderPaymentNccc
    {
       public DateTime nccc_createdates { get; set; }
       public OrderPaymentNcccQuery()
       {
           nccc_createdates = DateTime.MinValue; 
       }
    }
}
