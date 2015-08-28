using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    [DBTableInfo("order_return_master")]
    public class OrderReturnMaster : PageBase
    {

        public uint return_id { get; set; }
        public uint order_id { get; set; }
        public uint vendor_id { get; set; }
        public uint return_status { get; set; }
        public string return_note { get; set; }
        public string bank_note { get; set; }
        public uint invoice_deal { get; set; }
        public uint package { get; set; }
        public uint return_zip { get; set; }
        public string return_address { get; set; }
        public string deliver_code { get; set; }
        public uint return_createdate { get; set; }
        public uint return_updatedate { get; set; }
        public string return_ipfrom { get; set; }

        public OrderReturnMaster()
        {
            return_id = 0;
            order_id = 0;
            vendor_id = 0;
            return_status = 0;
            return_note = string.Empty;
            bank_note = string.Empty;
            invoice_deal = 0;
            package = 0;
            return_zip = 0;
            return_address = string.Empty;
            deliver_code = string.Empty;
            return_createdate = 0;
            return_updatedate = 0;
            return_ipfrom = string.Empty;
        }
    }
}