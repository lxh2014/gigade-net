using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class PromoAdditionalPriceQuery : PromoAdditionalPrice
    {
        public string category_link_url { get; set; }
        public string banner_image { get; set; }
        public string group_name { get; set; } 
        public string parameterName { get; set; }
        public string payment_name { get; set; }
        public string bank { get; set; }
        public int expired { get; set; }
        public string PTname { get; set; }
        public string event_id { get; set; }
        public string deliver_name { get; set; }
        public string banner_link_url { get; set; }
        public string category_ipfrom { get; set; }
        public string device_name { get; set; }
        public string price_master_in { get; set; }
        public string user_username { get; set; }
        public PromoAdditionalPriceQuery()
        {
            banner_image = string.Empty;
            category_link_url = string.Empty;
            group_name = string.Empty;
            payment_name = string.Empty;
            bank = string.Empty;
            expired = 0;
            PTname = string.Empty;
            event_id = string.Empty;
            deliver_name = string.Empty;
            device_name = string.Empty;
            banner_link_url = string.Empty;
            category_ipfrom = string.Empty;
            price_master_in = string.Empty;
            user_username = string.Empty;
        }

    }
}
