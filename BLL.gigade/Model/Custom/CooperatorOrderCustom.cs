using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class CooperatorOrderCustom:ProductItem
    {
        public string coop_product_id { get; set; }
        public uint buynum { get; set; }
        public uint product_cost { get; set; }
        public uint sumprice { get; set; }
        public string msg { get; set; }
        public string product_name { get; set; }
        public string parent_id { get; set; }
        public int s_must_buy { get; set; }
        public int ignore_stock { get; set; }
        /// <summary>
        /// 對應於該商品在價格表中的id
        /// </summary>
        public uint price_master_id { get; set; }
        //組合商品群組id
        public int group_id { get; set; }

        /// <summary>
        /// 價格類型
        /// </summary>
        public int price_type { get; set; }

        public CooperatorOrderCustom()
        {
            coop_product_id = string.Empty;
            buynum = 0;
            product_cost = 0;
            sumprice = 0;
            msg = string.Empty;
            product_name = string.Empty;
            s_must_buy = 0;
            price_master_id = 0;
            price_type = 0;
            ignore_stock = 0;//add 2014/08/25
        }

    }
}
