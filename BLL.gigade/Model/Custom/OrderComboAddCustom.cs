using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class OrderComboAddCustom
    {
        public uint product_id { get; set; }
        public string out_product_id { get; set; }
        public uint item_id { get; set; }
        public string product_name { get; set; }
        public uint spec1 { get; set; }
        public uint spec2 { get; set; }
        public string spec1_show { get; set; }
        public string spec2_show { get; set; }
        /// <summary>
        /// 商品售價
        /// </summary>
        public int product_cost { get; set; }
        public int stock { get; set; }
        public int buynum { get; set; }
        public Int64 g_must_buy { get; set; }
        public int price_type { get; set; }
        public int buy_limit { get; set; }
        /// <summary>
        /// 標記組合類型 2:固定組合,3：無規格任選組合
        /// </summary>
        public Int64 child { get; set; }
        public int childCount { get; set; }
        /// <summary>
        /// 商品成本
        /// </summary>
        public int cost { get; set; }
        /// <summary>
        /// 商品活動成本
        /// </summary>
        public int event_cost { get; set; }
        /// <summary>
        /// 活動結束時間
        /// </summary>
        public uint event_end { get; set; }
        /// <summary>
        /// 活動開始時間
        /// </summary>
        public uint event_start { get; set; }
        /// <summary>
        /// 商品活動價
        /// </summary>
        public int event_price { get; set; }

        public OrderComboAddCustom()
        {
            product_id = 0;
            item_id = 0;
            product_name = string.Empty;
            spec1 = 0;
            spec2 = 0;
            spec1_show = string.Empty;
            spec2_show = string.Empty;
            product_cost = 0;
            stock = 0;
            buynum = 0;
            price_type = 0;
            buy_limit = 0;
            childCount = 0;
            cost = 0;
            event_cost = 0;
            event_end = 0;
            event_start = 0;
            event_price = 0;
        }
    }
}