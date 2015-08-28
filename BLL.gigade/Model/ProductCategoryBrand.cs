using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductCategoryBrand : PageBase
    {
        public int row_id { get; set; }
        public int banner_cate_id { get; set; }
        public uint category_id { get; set; }
        public string category_name { get; set; }
        public uint category_father_id { get; set; }
        public string category_father_name { get; set; }
        public int depth { get; set; }
        public uint brand_id { get; set; }
        public DateTime createdate { get; set; }

        public ProductCategoryBrand()
        {
            row_id = 0;
            banner_cate_id = 0;
            category_id = 0;
            category_name = string.Empty;
            category_father_id = 0;
            category_father_name = string.Empty;
            depth = 0;
            brand_id = 0;
            createdate = DateTime.MinValue;
        }
    }
}
