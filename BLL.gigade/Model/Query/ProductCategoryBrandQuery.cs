using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ProductCategoryBrandQuery : ProductCategoryBrand
    {
        public string category_name { get; set; }
        public string brand_name { get; set; }
        public string search_content { get; set; }
        public int search_ID { get; set; }
        public string banner_catename { get; set; }
        public ProductCategoryBrandQuery()
        {
            category_name = string.Empty;
            brand_name = string.Empty;
            search_content = string.Empty;
            search_ID = 0;
            banner_catename = string.Empty;
        }
    }
}
