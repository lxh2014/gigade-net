using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
 public   class EventChinaTrustBagQuery : EventChinaTrustBag
    {
     public string create_user { get; set; }
     public string update_user { get; set; }
     public string create_time { get; set; }
     public string update_time { get; set; }
     public string start_time { get; set; }
     public string end_time { get; set; }
     public string show_start_time { get; set; }
     public string show_end_time { get; set; }
     public int date { get; set; }
     public string s_bag_banner { get; set; }
     public EventChinaTrustBagQuery()
     {
         create_user = string.Empty;
         update_user = string.Empty;
         create_time = string.Empty;
         update_time = string.Empty;
         start_time = string.Empty;
         end_time = string.Empty;
         show_start_time = string.Empty;
         show_end_time = string.Empty;
         date = 0;
         s_bag_banner = string.Empty;
     }
    }
}
