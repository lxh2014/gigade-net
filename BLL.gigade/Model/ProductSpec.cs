using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    [DBTableInfo("product_spec")]
    public class ProductSpec : PageBase
    {
        public uint spec_id { get; set; }
        public uint product_id { get; set; }
        public uint spec_type { get; set; }
        public string spec_name { get; set; }
        public uint spec_sort { get; set; }
        public uint spec_status { get; set; }
        public string spec_image { get; set; }
        public ProductSpec()
        {
            spec_id = 0;
            product_id = 0;
            spec_type = 0;
            spec_name = string.Empty;
            spec_sort = 0;
            spec_status = 0;
            spec_image = string.Empty;
        }
    }
}
