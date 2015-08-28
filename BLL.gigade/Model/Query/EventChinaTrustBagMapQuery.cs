using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class EventChinaTrustBagMapQuery : EventChinaTrustBagMap
    {
        public string product_name { get; set; }
        public string link { get; set; }
       public string event_name{ get; set; }
       public string bag_name { get; set; }
       public int search_con { get; set; }
       public string con { get; set; }
       public string forbid_banner { set; get; }
       public string active_banner { set; get; }

        public EventChinaTrustBagMapQuery()
        {
            product_name = string.Empty;
            link = string.Empty;
            event_name = string.Empty;
            bag_name = string.Empty;
            search_con = 0;
            con = string.Empty;
            forbid_banner = string.Empty;
            active_banner = string.Empty;
        }
    }
}
