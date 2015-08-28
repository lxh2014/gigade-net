using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    /// <summary>
    /// 分類表管理
    /// </summary>
    public class Appcategory:PageBase
    {
        public int category_id { get; set; }
        public string category { get; set; }
        public int brand_id { get; set; }
        public string brand_name { get; set; }
        public string category1 { get; set; }
        public string category2 { get; set; }
        public string category3 { get; set; }
        public int product_id { get; set; }
        public string property { get; set; }

        public Appcategory()
        {
            category_id = 0;
            category = string.Empty;
            brand_id = 0;
            brand_name = string.Empty;
            category1 = string.Empty;
            category2 = string.Empty;
            category3 = string.Empty;
            product_id = 0;
            property = string.Empty;
        }

    }
}
