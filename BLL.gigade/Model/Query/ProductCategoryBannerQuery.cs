using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ProductCategoryBannerQuery : ProductCategoryBanner
    {
        public string category_father_name { get; set; }
        public string banner_catename { get; set; }
        public string categoryIds { get; set; }
        public ProductCategoryBannerQuery()
        {
            categoryIds = string.Empty;
            category_father_name = string.Empty;
            banner_catename = string.Empty;
        }
    }
}
