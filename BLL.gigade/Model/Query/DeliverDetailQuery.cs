using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class DeliverDetailQuery:DeliverDetail
    {
        public uint item_id { get; set; }
        public string product_name { get; set; }
        public string product_spec_name { get; set; }
        public uint product_freight_set { get; set; }
        public uint buy_num { get; set; }
        public uint detail_status { get; set; }
        public string sdetail_status { get; set; }
        public uint item_vendor_id { get; set; }
        public uint product_mode { get; set; }//Order_Detail表的product_mode
        public int combined_mode { get; set; }
        public int parent_id { get; set; }
        public string parent_name { get; set; }
        public uint item_mode { get; set; }
        public uint pack_id { get; set; }
        public uint parent_num { get; set; }
        public uint prod_mode { get; set; }//Product表的product_mode

        public DeliverDetailQuery()
        {
            item_id = 0;
            product_name = string.Empty;
            product_spec_name = string.Empty;
            product_freight_set = 0;
            buy_num = 0;
            detail_status = 0;
            sdetail_status =string.Empty;
            item_vendor_id = 0;
            product_mode = 0;
            combined_mode = 0;
            parent_id = 0;
            parent_name = string.Empty;
            item_mode = 0;
            pack_id = 0;
            parent_num = 0;
            prod_mode = 0;

        
        
        
        }
    }
}
