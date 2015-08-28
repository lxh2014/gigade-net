using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class InvoiceSliveInfo:PageBase
    {
        public int invoice_slive_id { get; set; }
        public int invoice_id { get; set; }
        public int order_id { get; set; }
        public int item_id { get; set; }
        public uint invoice_status { get; set; }
        public uint invoice_allowance { get; set; }
        public string product_name { get; set; }
        public string product_spec_name { get; set; }
        public int sort { get; set; }
        public int single_money { get; set; }
        public int sub_deduct_bonus { get; set; }
        public int buy_num { get; set; }
        public int subtotal { get; set; }
        public string slive_note { get; set; }
        public int slive_createdate { get; set; }


        public InvoiceSliveInfo()
        {
            invoice_slive_id = 0;
            invoice_id = 0;
            order_id = 0;
            item_id = 0;
            invoice_status = 0;
            invoice_allowance = 0;
            product_name = string.Empty;
            product_spec_name = string.Empty;
            sort = 0;
            single_money = 0;
            sub_deduct_bonus = 0;
            buy_num = 0;
            subtotal = 0;
            slive_note = string.Empty;
            slive_createdate = 0;
        }
    }
}
