using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{

    public class VendorBrandSetQuery : VendorBrand
    {
        public uint class_id { get; set; }
        public string class_name { get; set; }
        public uint vendor_id { get; set; }
        public string vendor_name_full { get; set; }
        public string vendor_name_simple { get; set; }
        public int SearchType { get; set; }
        public string SearchCondition { get; set; }
        public string classIds { get; set; }
        public string image_filename { get; set; }
        public uint image_sort { get; set; }
        public uint image_state { get; set; }
        public uint image_createdate { get; set; }
        public DateTime begin_time { get; set; }
        public DateTime end_time { get; set; }
        public DateTime eventstatrtime { get; set; }
        public DateTime eventendtime { get; set; }
        public uint creator { get; set; }
        public uint vendorstatus { get; set; }

        public string brand_logo { get; set; }

        public VendorBrandSetQuery()
        {
            image_sort = 0;
            image_state = 1;
            begin_time = DateTime.MinValue;
            end_time = DateTime.MinValue;
            eventstatrtime = DateTime.MinValue;
            eventendtime = DateTime.MinValue;
            creator = 0;
            vendorstatus = 0;
            brand_logo = string.Empty;
        }
    }
}
