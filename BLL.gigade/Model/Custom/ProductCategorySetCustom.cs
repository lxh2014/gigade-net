using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
namespace BLL.gigade.Model.Custom
{
    public class ProductCategorySetCustom : ProductCategorySet
    {
        public string Category_Name { get; set; }
        public ProductCategorySetCustom()
        {
            Category_Name = string.Empty;
        }
    }
}
