using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ProductCategorySetQuery : ProductCategorySet
    {
        public string product_ids { get; set; }
        public ProductCategorySetQuery()
        {
            product_ids = string.Empty;
        }
    }
}
