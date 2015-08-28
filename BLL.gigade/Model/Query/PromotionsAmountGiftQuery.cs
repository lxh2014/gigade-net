using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class PromotionsAmountGiftQuery : PromotionsAmountGift
    {
        public string banner_image { get; set; }
        public string category_link_url { get; set; }
        public string group_name { get; set; }
        public string brand_name { get; set; }
        public string condition_name { get; set; }
        public string freight { get; set; }
        public string eventtype { get; set; }
        public string payment { get; set; }
        public string devicename { get; set; }
        public int expired { get; set; }//查詢是否過期
        public int item_id { get; set; }
        public string category_ipfrom { get; set; }
        public uint category_father_id { get; set; } 
        public string event_ids { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public string sitename { get; set; }
        public int quanguan { get; set; }
        public int newactive { get; set; }
        public string selcon { get; set; }
        public string user_username { get; set; }

        public PromotionsAmountGiftQuery()
        {
            banner_image = string.Empty;
            category_link_url = string.Empty;
            group_name = string.Empty;
            brand_name = string.Empty;
            condition_name = string.Empty;
            freight = string.Empty;
            eventtype = string.Empty;
            payment = string.Empty;
            devicename = string.Empty;
            expired = 0;
            item_id = 0;
            category_ipfrom = string.Empty;
            category_father_id = 0;
            event_ids = string.Empty;
            startdate = DateTime.MinValue;
            enddate = DateTime.MinValue;
            sitename = string.Empty;
            newactive = 0;
            quanguan = 0;
            event_type = string.Empty;
            selcon = string.Empty;
            user_username = string.Empty;
        }

    }

}
