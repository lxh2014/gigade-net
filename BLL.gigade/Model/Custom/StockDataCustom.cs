using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class StockDataCustom
    {
        public uint product_id { get; set; }
        public uint item_id { get; set; }
        public string product_name { get; set; }
        public string prod_sz { get; set; }
        public string spec_name1 { get; set; }
        public string spec_name2 { get; set; }
        public int item_stock { get; set; }
        public int ignore_stock { get; set; }
        public int shortstatus { get; set; }//增加一個shortage edit by hufeng0813w 2014/05/22 
        public string vendor_product_id { get; set; }
    }
}
