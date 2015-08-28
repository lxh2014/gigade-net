using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductItemMap : PageBase
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public uint rid { get; set; }
        /// <summary>
        /// 外站編號
        /// </summary>
        public uint channel_id { get; set; }
        /// <summary>
        /// 外站商品編號
        /// </summary>
        public string channel_detail_id { get; set; }
        /// <summary>
        /// 吉甲地商品細項編號 
        /// </summary>
        public uint item_id { get; set; }
        /// <summary>
        /// 外站商品名稱
        /// </summary>
        public string product_name { get; set; }

        ///<summary>
        ///商品附加信息
        /// <summary>
        public string prod_sz { get; set; } 
        
        /// <summary>
        /// 外站商品成本
        /// </summary>
        public int product_cost { get; set; }
        /// <summary>
        /// 外站商品售價
        /// </summary>
        public int product_price { get; set; }
        /// <summary>
        /// 組合商品ID
        /// </summary>
        public uint product_id { get; set; }

        public string group_item_id { get; set; }

        //edit by hufeng0813w   2014/07/02 Reason:對照需要有自己的站臺價格
        public uint price_master_id { get; set; }

        public string msg { get; set; }

        public ProductItemMap()
        {
            rid = 0;
            item_id = 0;
            channel_id = 0;
            channel_detail_id = string.Empty;
            product_name = string.Empty;
            product_cost = 0;
            product_price = 0;
            product_id = 0;
            group_item_id = string.Empty;
            price_master_id = 0;
            msg = string.Empty;
            prod_sz = string.Empty;
        }

    }
}
