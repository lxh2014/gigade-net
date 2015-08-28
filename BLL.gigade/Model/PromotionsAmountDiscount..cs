using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    //promotions_amount_discount
    [DBTableInfo("promotions_amount_discount")]
    public class PromotionsAmountDiscount : PageBase
    {

        public int id { get; set; }
        public string name { get; set; }
        public int group_id { get; set; }
        public int class_id { get; set; }
        public int brand_id { get; set; }
        public uint category_id { get; set; }
        public int product_id { get; set; }
        public int type { get; set; }
        public int amount { get; set; }
        public int quantity { get; set; }
        public int discount { get; set; }
        public int vendor_coverage { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public bool active { get; set; }
        public string event_desc { get; set; }
        public string event_type { get; set; }
        public int condition_id { get; set; }
        public int device { get; set; }
        public int payment_code { get; set; }
        public string kuser { get; set; }
        public string muser { get; set; }
        public string site { get; set; }
        public int status { get; set; }
        public int url_by { get; set; }

        public PromotionsAmountDiscount()
        {
            id = 0;
            name = string.Empty;
            group_id = 0;
            class_id = 0;
            brand_id = 0;
            category_id = 0;
            product_id = 0;
            type = 0;
            amount = 0;
            quantity = 0;
            discount = 0;
            vendor_coverage = 10;
            start = DateTime.MinValue;
            end = DateTime.MinValue;
            created = DateTime.MinValue;
            modified = DateTime.MinValue;
            active = false;
            //active = 0;
            event_desc = string.Empty;
            event_type = string.Empty;
            condition_id = 0;
            device = 0;
            payment_code = 0;
            kuser = string.Empty;
            muser = string.Empty;
            site = string.Empty;
            status = 1;
            url_by = 0;

        }
    }
}