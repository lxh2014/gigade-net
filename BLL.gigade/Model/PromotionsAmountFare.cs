using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;


namespace BLL.gigade.Model
{
    //promotions_amount_fare
    [DBTableInfo("promotions_amount_fare")]
    public class PromotionsAmountFare : PageBase
    {
        public int id { get; set; }
        public string name { get; set; }
        public string display { get; set; }
        public int delivery_store { get; set; }
        public int group_id { get; set; }
        public int class_id { get; set; }
        public int brand_id { get; set; }
        public uint category_id { get; set; }
        public int product_id { get; set; }
        public int type { get; set; }
        public int amount { get; set; }
        public int quantity { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public bool active { get; set; }
        public string muser { get; set; }
        public string event_desc { get; set; }
        public string event_type { get; set; }
        public int condition_id { get; set; }
        public int device { get; set; }
        public string kuser { get; set; }
        public int fare_percent { get; set; }
        public string payment_code { get; set; }
        public int off_times { get; set; }
        public int url_by { get; set; }
        public string site { get; set; }
        public int status { get; set; }
        public int vendor_coverage { get; set; }
        public PromotionsAmountFare()
        {

            id = 0;
            name = string.Empty;
            display = string.Empty;
            delivery_store = 1;
            group_id = 0;
            class_id = 0;
            brand_id = 0;
            category_id = 0;
            product_id = 0;
            type = 0;
            amount = 0;
            quantity = 0;
            start = DateTime.MinValue;
            end = DateTime.MinValue;
            created = DateTime.MinValue;
            modified = DateTime.MinValue;
            active = false;//默認不啟用
            muser = string.Empty;
            event_desc = string.Empty;
            event_type = string.Empty;
            condition_id = 0;
            device = 1;
            payment_code = string.Empty;
            kuser = string.Empty;
            fare_percent = 0;
            off_times = 0;
            url_by = 0;
            site = string.Empty;
            status = 1;//未刪
            vendor_coverage = 0;
        }
    }
}
