using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    [DBTableInfo("promotions_amount_gift")]
    public class PromotionsAmountGift : PageBase
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
        public bool repeat { get; set; }
        public int gift_id { get; set; }
        public uint deduct_welfare { get; set; }
        public int bonus_type { get; set; }
        public uint mailer_id { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public int active { get; set; }
        public string event_desc { get; set; }
        public string event_type { get; set; }
        public int condition_id { get; set; }
        public int device { get; set; }
        public string payment_code { get; set; }
        public int gift_type { get; set; }
        public int ticket_id { get; set; }
        public string ticket_name { get; set; }
        public int count_by { get; set; }
        public int number { get; set; }
        public int num_limit { get; set; }
        public int active_now { get; set; }
        public int valid_interval { get; set; }
        public DateTime use_start { get; set; }
        public DateTime use_end { get; set; }
        public string kuser { get; set; }
        public string muser { get; set; }
        public int url_by { get; set; }
        public string url { get; set; }
        public string banner_file { get; set; }
        public int status { get; set; }
        public string site { get; set; }
        public int vendor_coverage { get; set; }
        public int gift_product_number { get; set; }
        public int freight_price { get; set; }
        public string delivery_category { get; set; }
        public int gift_mundane { get; set; }
        public string event_id { get; set; }
        public int bonus_state { get; set; }
        public int point { get; set; }
        public int dollar { get; set; }
        public int dividend { get; set; }
        public PromotionsAmountGift()
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
            repeat = false;
            gift_id = 0;
            deduct_welfare = 0;
            bonus_type = 0;
            mailer_id = 0;
            start = DateTime.MinValue;
            end = DateTime.MinValue;
            created = DateTime.MinValue;
            modified = DateTime.MinValue;
            active = 0;
            event_desc = string.Empty;
            event_type = string.Empty;
            condition_id = 0;
            device = 1;
            payment_code = string.Empty;
            gift_type = 0;
            ticket_id = 0;
            ticket_name = string.Empty;
            count_by = 1;
            num_limit = 0;
            number = 0;
            active_now = 0;
            valid_interval = 0;
            use_start = DateTime.MinValue;
            use_end = DateTime.MinValue;
            kuser = string.Empty;
            muser = string.Empty;
            url_by = 0;
            url = string.Empty;
            banner_file = string.Empty;
            status = 0;
            site = string.Empty;
            vendor_coverage = 0;
            gift_product_number = 0;
            freight_price = 0;
            delivery_category = string.Empty;
            gift_mundane = 0;
            event_id = string.Empty;
            bonus_state = 0;//0:一般 1:試吃 2：紅利折抵
            point = 0;
            dollar = 0;
            dividend = 0;//紅利類型0.無1.點2:點+金3:金4比率固定5:非固定 add by shuangshuang0420j 2014.11.10 11:38:26
        }
    }
}
