using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    //shop_class
    [DBTableInfo("shop_class")]
    public class ShopClass : PageBase
    {

        public uint class_id { get; set; }
        public string class_name { get; set; }
        public uint class_sort { get; set; }
        public uint class_status { get; set; }
        public string class_content { get; set; }
        public uint class_createdate { get; set; }
        public uint class_updatedate { get; set; }
        public string class_ipfrom { get; set; }

        public ShopClass()
        {
            class_id = 0;
            class_name = string.Empty;
            class_sort = 0;
            class_status = 1;
            class_content = string.Empty;
            class_createdate = 0;
            class_updatedate = 0;
            class_ipfrom = string.Empty;
        }
    }
}