using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EventChinaTrustBagMap:PageBase
    {
        public int map_id { set; get; }
        public int bag_id { set; get; }
        public uint product_id { set; get; }
        public string linkurl { set; get; }
        public string product_forbid_banner { set; get; }
        public string product_active_banner { set; get; }
        public int map_active { set; get; }
        public int map_sort { set; get; }
        public string ad_product_id { set; get; }
        public string product_desc { set; get; }

        

        public EventChinaTrustBagMap()
        {
            map_id = 0;
            bag_id =0;
            product_id = 0;
            linkurl = string.Empty;
            product_forbid_banner = string.Empty;
            product_active_banner = string.Empty;
            map_active = 0;
            map_sort = 0;
            ad_product_id = string.Empty;
            product_desc = string.Empty;
        }
    }
}
