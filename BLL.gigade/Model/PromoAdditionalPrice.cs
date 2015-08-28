using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class PromoAdditionalPrice : PageBase
    {
        public int id { get; set; }
        public string event_name { get; set; }
        public string event_desc { get; set; }
        public string event_type { get; set; }
        public int condition_id { get; set; }
        public int group_id { get; set; }
        public int class_id { get; set; }
        public int brand_id { get; set; }
        public int product_id { get; set; }
        public DateTime starts { get; set; }
        public DateTime end { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public Boolean active { get; set; }
        public int deliver_type { get; set; }
        public string device { get; set; }
        public string payment_code { get; set; }
        public int fixed_price { get; set; }
        public int category_id { get; set; }
        public int buy_limit { get; set; }
        public string kuser { get; set; }
        public string muser { get; set; }
        public string website { get; set; }
        public int status { get; set; }
        public int url_by { get; set; }
        public int discount { get; set; }
        public int left_category_id { get; set; }
        public int right_category_id { get; set; }

        public PromoAdditionalPrice()
        {
            id = 0;
            event_name = string.Empty;
            event_desc = string.Empty;
            event_type = string.Empty;
            condition_id = 0;
            group_id = 0;
            class_id = 0;
            brand_id = 0;
            product_id = 0;
            starts = DateTime.MinValue;
            end = DateTime.MinValue;
            created = DateTime.MinValue;
            modified = DateTime.MinValue;
            active = false;
            deliver_type = 0;
            device = string.Empty;
            payment_code = string.Empty;
            fixed_price = 0;
            category_id = 0;
            buy_limit = 0;
            kuser = string.Empty;
            muser = string.Empty;
            status = 1;
            url_by = 0;
            discount = 0;
            left_category_id = 0;
            right_category_id = 0;
        }
    }
}
