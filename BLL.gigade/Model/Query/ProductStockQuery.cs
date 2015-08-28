using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ProductStockQuery
    {
        public string item_id { get; set; }
        public string product_id { get; set; }
        public string item_stock{get;set;}
        public string item_alarm { get; set; }
        public string product_name { get; set; }
        public string spec_name1 { get; set; }
        public string spec_name2 { get; set; }
        public string product_mode { get; set; }
        public string prepaid { get; set; }
        public int type { get; set; }
        public string spec_status { get; set; }
        public string spec_status2 { get; set; }
        public string remark { get; set; }

        public ProductStockQuery()
        {
            item_id = "0";
            product_id = "0";
            item_stock = "0";
            item_alarm = "0";
            product_name = string.Empty;
            spec_name1 = string.Empty;
            spec_name2 = string.Empty;
            product_mode = string.Empty;
            prepaid = string.Empty;
            type = 0;
        }
    }
}
