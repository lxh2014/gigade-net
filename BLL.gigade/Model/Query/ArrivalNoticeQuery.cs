using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public class ArrivalNoticeQuery:ArrivalNotice
    {
       public string user_name { get; set; }
       public string  product_name { get; set; }
       public DateTime s_create_time { get; set; }
       public int condition { get; set; }
       public string searchCon { get; set; }
       public int item_stock { get; set; }
       public string user_email { get; set; }
       public ArrivalNoticeQuery()
    {
           user_name = string.Empty;
           product_name = string.Empty;
           s_create_time = DateTime.MinValue;
           condition = 0;
           searchCon = string.Empty;
           item_stock = 0;
           user_email = string.Empty;
       }
    }
}
