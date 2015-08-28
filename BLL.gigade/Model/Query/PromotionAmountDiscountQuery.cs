using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class PromotionAmountDiscountQuery : PromotionsAmountDiscount
    {
        //public int id { get; set; }
        public string event_id { get; set; }
        //public string name { get; set; }
        //public string event_desc { get; set; }
       // public string event_type { get; set; }
        public string class_name { get; set; }
        public string brand_name { get; set; }
        //public int class_id { get; set; }
       // public int brand_id { get; set; }
        //public int product_id { get; set; }
        //public int amount { get; set; }
        //public int quantity { get; set; }
        //public int discount { get; set; }
        //public uint category_id { get; set; }
        public string banner_image { get; set; }
        public string category_link_url { get; set; }
        public string group_name { get; set; }
        //public int condition_id { get; set; }
        public string condition_name { get; set; }
        public string devicename { get; set; }
        //public int vendor_coverage { get; set; }
        //public string site { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public int active { get; set; }
        public int expired { get; set; }//查詢是否過期
        //public int url_by { get; set; }
        public string siteId { get; set; }
        public int isallclass { get; set; }//是否全館
        public string user_username { get; set; }
        public PromotionAmountDiscountQuery()
        {
            //id = 0;
            event_id = string.Empty;
            //name = string.Empty;
            //event_desc = string.Empty;
            //event_type = string.Empty;
            class_name = string.Empty;
            //class_id = 0;
            //brand_id = 0;
            //product_id = 0;
            brand_name = string.Empty;
            //amount = 0;
            //quantity = 0;
            //discount = 0;
            //category_id = 0;
            //condition_id = 0;
            banner_image = string.Empty;
            category_link_url = string.Empty;
            group_name = string.Empty;
            condition_name = string.Empty;
            devicename = string.Empty;
            //vendor_coverage = 0;
            site = string.Empty;
            startdate = DateTime.MinValue;
            enddate = DateTime.MinValue;
            active = 0;
            expired = 0;
            //url_by = 0;
            siteId = string.Empty;
            isallclass = 0;
            user_username = string.Empty;
        }

    }
}
