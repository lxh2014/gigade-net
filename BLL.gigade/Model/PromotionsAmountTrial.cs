
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{

    public class PromotionsAmountTrial : PageBase
    {

        public int id { get; set; }
        public string name { get; set; }
        public string event_type { get; set; }
        public string event_id { get; set; }
        public int paper_id { get; set; }
        public string url { get; set; }
        public string site { get; set; }
        public int device { get; set; }
        public int freight_type { get; set; }
        public int group_id { get; set; }
        public int condition_id { get; set; }
        public int count_by { get; set; }
        public int num_limit { get; set; }
        public int gift_mundane { get; set; }
        public bool repeat { get; set; }
        public int product_id { get; set; }
        public string product_name { get; set; }
        public int sale_productid { get; set; }
        public int brand_id { get; set; }
        public uint category_id { get; set; }
        public string product_img { get; set; }
        public int market_price { get; set; }
        public int show_number { get; set; }
        public int apply_sum { get; set; }
        public int apply_limit { get; set; }
        public string event_img_small { get; set; }
        public string event_img { get; set; }
        public string event_desc { get; set; }
        public int active { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public string kuser { get; set; }
        public string muser { get; set; }
        public int status { get; set; }
         
        public PromotionsAmountTrial()
        {
            id = 0;
            name = string.Empty;
            event_type = string.Empty;
            event_id = string.Empty;
            paper_id = 0;
            url = string.Empty;
            site = string.Empty;
            device = 0;
            freight_type = 0;
            group_id = 0;
            condition_id = 0;
            count_by = 0;
            num_limit = 0;
            gift_mundane = 0;
            repeat = false;
            product_id = 0;
            product_name = string.Empty;
            sale_productid = 0;
            brand_id = 0;
            category_id = 0;
            product_img = string.Empty;
            market_price = 0;
            show_number = 0;
            apply_sum = 0;
            apply_limit = 0;
            event_img_small = string.Empty;
            event_img = string.Empty;
            event_desc = string.Empty;
            active = 0;
            start_date = DateTime.MinValue;
            end_date = DateTime.MinValue;
            created = DateTime.MinValue;
            modified = DateTime.MinValue;
            kuser = string.Empty;
            muser = string.Empty;
            status = 0;
        }
    }
}