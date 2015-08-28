
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class PriceMasterProductCustomTemp : PriceMasterTemp
    {
        public int price_type { get; set; }
        public uint product_price_list { get; set; }
        public uint bag_check_money { get; set; }
        public uint show_listprice { get; set; }
        public uint product_mode { get; set; }
        public string site_name { get; set; }
        public string user_email { get; set; }
        public string user_level_name { get; set; }
        public string status { get; set; }
        public int combination { get; set; }//商品類型
        public int event_price_discount { get; set; }//活動價格折數
        public int event_cost_discount { get; set; }//活動成本折數
        public int price_discount { get; set; }
        public int cost_discount { get; set; }
        public PriceMasterProductCustomTemp()
        {
            price_type = 0;
            product_price_list = 0;
            bag_check_money = 0;
            show_listprice = 0;
            product_mode = 0;
            site_name = string.Empty;
            user_email = string.Empty;
            user_level_name = string.Empty;
            status = string.Empty;

            combination = 0;
            price_discount = 0;
            cost_discount = 0;
            event_price_discount = 0;
            event_cost_discount = 0;
        }

    }
}
