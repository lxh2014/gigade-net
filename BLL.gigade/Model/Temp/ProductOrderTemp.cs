using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Temp
{
    public class ProductOrderTemp
    {
        public string brand_name { get; set; }
        public string product_name { get; set; }
        public uint product_id { get; set; }
        public uint item_id { get; set; }
        public uint combination { get; set; }
        public Int64 c_num { get; set; }
        public DateTime create_time { get; set; }
        public DateTime product_start { get; set; }
        public DateTime product_end { get; set; }
        public string ignore_stock { get; set; }
        public string shortage { get; set; }
        public string brand_id { get; set; }
        public int parent_id { get; set; }

        /// <summary>
        /// 匯出時顯示的key欄位  add by zhuoqin0830w 2015/05/18
        /// </summary>
        public string combination_name { get; set; }

        public ProductOrderTemp() 
        {
            brand_name = string.Empty;
            product_name = string.Empty;
            product_id = 0;
            item_id = 0;
            combination = 0;
            combination_name = string.Empty;
            c_num = 0;
            create_time = DateTime.MinValue;
            product_start = DateTime.MinValue;
            product_end = DateTime.MinValue;
            ignore_stock = string.Empty;
            shortage = string.Empty;
            brand_id = string.Empty ;
            parent_id = 0;
        }
    }
}
