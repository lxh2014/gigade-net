using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class PromotionsAmountFareQuery : PromotionsAmountFare
    {
        public int expired { get; set; }//查詢是否過期
        public string banner_image { get; set; }
        public string group_name { get; set; }
        public string condition_name { get; set; }
        public DateTime start_time { get; set; }
        public string site_name { get; set; }
        public string typeName { get; set; }
        public DateTime end_time { get; set; }
        public string payment_name { get; set; }
        public string event_type_name { get; set; }
        public string deviceName { get; set; }
        public string category_ipfrom { get; set; }
        public string category_link_url { get; set; }
        public uint category_father_id { get; set; }
        public string event_id { get; set; }
        public string brand_name { get; set; }
        public string class_name { get; set; }
        public int allClass { get; set; }
        public string user_username { get; set; }

        public PromotionsAmountFareQuery()
        {
            site_name = string.Empty;
            event_id = string.Empty;
            banner_image = string.Empty;
            group_name = string.Empty;
            condition_name = string.Empty;
            start_time = DateTime.MinValue;
            end_time = DateTime.MinValue;
            typeName = string.Empty;
            payment_name = string.Empty;
            event_type_name = string.Empty;
            deviceName = string.Empty;
            expired = 0;
            category_ipfrom = string.Empty;
            category_link_url = string.Empty;
            category_father_id = 0;
            brand_name = string.Empty;
            class_name = string.Empty;
            allClass = 0;
            user_username = string.Empty;
           
        }
    }
}
