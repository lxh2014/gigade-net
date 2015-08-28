using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class EventChinaTrustBag:PageBase
    {
       public int bag_id { get; set; }
       public string bag_name { get; set; }
       public string bag_desc { get; set; }
       public string bag_banner { get; set; }
       public DateTime bag_start_time { get; set; }
       public DateTime bag_end_time { get; set; }
       public int bag_active { get; set; }
       public int bag_create_user { get; set; }
       public int bag_update_user { get; set; }
       public DateTime bag_create_time { get; set; }
       public DateTime bag_update_time { get; set; }
       public DateTime bag_show_start_time { get; set; }
       public DateTime bag_show_end_time { get; set; }
       public string event_id { get; set; }
       public int product_number { get; set; }

       public EventChinaTrustBag()
       {
           bag_id = 0;
           bag_name =string.Empty;
           bag_desc = string.Empty;
           bag_banner = string.Empty;
           bag_start_time = DateTime.MinValue;
           bag_end_time = DateTime.MinValue;
           bag_active = 0;
           bag_create_user = 0;
           bag_update_user = 0;
           bag_create_time = DateTime.MinValue;
           bag_update_time = DateTime.MinValue;
           bag_show_start_time = DateTime.MinValue;
           bag_show_end_time = DateTime.MinValue;
           event_id = string.Empty;
           product_number = 0;
       }
    }
}
