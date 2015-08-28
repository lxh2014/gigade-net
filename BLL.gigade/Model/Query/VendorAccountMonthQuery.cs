using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class VendorAccountMonthQuery : VendorAccountMonth
    {
        public string detail { get; set; }
        public string vendor_code { get; set; }
        public string vendor_name_simple { get; set; }
        public uint product_mode { get; set; }
        public string searchEmail { get; set; }
        public string searchName { get; set; }
        public string searchInvoice { get; set; }
        public int searchStatus { get; set; }
        public string content { get; set; }
        public int type { get; set; }
        public string keyworks { get; set; }
        public VendorAccountMonthQuery()
        {

            searchEmail = string.Empty;
            searchName = string.Empty;
            searchInvoice = string.Empty;
            searchStatus = -1;
            content = "";
            vendor_code = string.Empty;
            vendor_name_simple = string.Empty;
            product_mode = 0;
            type = 0;
            keyworks = string.Empty;

        }
    }
}
