using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    [DBTableInfo("vendor_cate_set")]
    public class VendorCateSet : PageBase
    {

        public int rid { get; set; }
        public int vendor_id { get; set; }
        public string cate_code_big { get; set; }
        public string cate_code_middle { get; set; }
        public string cate_code_serial { get; set; }
        public string tax_type { get; set; }

        public VendorCateSet()
        {
            rid = 0;
            vendor_id = 0;
            cate_code_big = string.Empty;
            cate_code_middle = string.Empty;
            cate_code_serial = string.Empty;
            tax_type = string.Empty;
        }
    }
}