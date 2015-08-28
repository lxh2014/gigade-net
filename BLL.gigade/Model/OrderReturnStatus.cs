using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class OrderReturnStatus:PageBase
    {
       public int ors_id { get; set; }
       public int ors_order_id { get; set; }
       public int ors_status { get; set; }
       public string ors_remark { get; set; }
       public DateTime ors_createdate { get; set; }
       public int ors_createuser { get; set; }

       public OrderReturnStatus()
       {
           ors_id = 0;
           ors_order_id = 0;
           ors_status = 0;
           ors_remark = string.Empty;
           ors_createdate = DateTime.MinValue;
           ors_createuser = 0;

       }
    }
}
