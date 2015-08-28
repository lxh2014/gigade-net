using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class OrderCancelMasterQuery:OrderCancelMaster
    {
        public UInt64 subtotal { get; set; }
        public uint detail_id { get; set; }
        public uint slave_id { get; set; }
        public uint item_id { get; set; }
        public uint product_freight_set { get; set; }
        public string product_name { get; set; }
        public string product_spec_name { get; set; }
        public string detail_note { get; set; }
        public uint single_money { get; set; }
        public uint buy_num { get; set; }
        public uint deduct_bonus { get; set; }
        public string cancle_status_string { get; set; }
        public string product_freight_set_string { get; set; }
        public OrderCancelMasterQuery()
        {
            subtotal = 0;
            detail_id = 0;
            slave_id = 0;
            item_id = 0;
            product_freight_set = 1;
            product_name = string.Empty;
            product_spec_name = string.Empty;
            detail_note = string.Empty;
            single_money = 0;
            buy_num = 0;
            deduct_bonus = 0;
        }










    }
}
