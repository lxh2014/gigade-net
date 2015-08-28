using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    /// <summary>
    /// 外站商品組合表
    /// </summary>
    public class ProductMapSet
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public uint rid { get; set; }
        /// <summary>
        /// 外站商品對照表rid--->product_item_map_rid
        /// </summary>
        public uint map_rid { get; set; }
        /// <summary>
        /// 規格id -->product_item.item_id
        /// </summary>
        public uint item_id { get; set; }
        /// <summary>
        /// 數量
        /// </summary>
        public uint set_num { get; set; }

        public ProductMapSet()
        {
            rid = 0;
            map_rid = 0;
            item_id = 0;
            set_num = 0;
        }
    }
}
