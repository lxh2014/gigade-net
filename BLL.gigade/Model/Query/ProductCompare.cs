using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ProductCompare : PageBase
    {
        public uint rid { get; set; }
        public uint product_id { get; set; }
        public uint item_id { get; set; }
        public uint channel_id { get; set; }
        public string channel_detail_id { get; set; }
        public string product_p_name { get; set; }
        public string product_name { get; set; }
        public string prod_sz { get; set; }
        public uint spec_id_1 { get; set; }
        public uint spec_id_2 { get; set; }
        public string spec_name_1 { get; set; }
        public string spec_name_2 { get; set; }
        /// <summary>
        /// 對照成本
        /// </summary>
        public int product_cost { get; set; }
        /// <summary>
        /// 對照價格
        /// </summary>
        public int product_price { get; set; }
        /// <summary>
        /// 原商品成本
        /// </summary>
        public int cost { get; set; }
        /// <summary>
        /// 原商品價格
        /// </summary>
        public int price { get; set; }
        public ProductCompare()
        {
            rid = 0;
            product_id = 0;
            item_id = 0;
            channel_id = 0;
            channel_detail_id = string.Empty;
            product_p_name = string.Empty;
            product_name = string.Empty;
            prod_sz = string.Empty;
            spec_id_1 = 0;
            spec_id_2 = 0;
            spec_name_1 = string.Empty;
            spec_name_2 = string.Empty;
            product_cost = 0;
            product_price = 0;
            cost = 0;
            price = 0;
        }
    }
}
