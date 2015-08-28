using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class VendorBrandQuery : VendorBrand
    {
        public string searchContent { get; set; }
        public string searchVendor { get; set; }
        public int searchState { get; set; }
        public string story_createname { get; set; }
        public string story_updatename { get; set; }
        public DateTime date_start { get; set; }
        public DateTime date_end { get; set; }
        public bool isExport { get; set; }
        public int vendorState { get; set; }//供應商狀態 add by jiaohe0625j 
        public string vendor_name_simple { get; set; }//訂單管理 品牌營業額add by chaojie1124j

        public VendorBrandQuery()
        {
            searchContent = string.Empty;
            searchVendor = string.Empty;
            searchState = 0;
            story_createname = string.Empty;
            story_updatename = string.Empty;
            date_start = DateTime.MinValue;
            date_end = DateTime.MinValue;
            isExport = false;
            vendorState = 0;
            vendor_name_simple = string.Empty;
        }
    }
}
