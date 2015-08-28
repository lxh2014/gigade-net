using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
namespace BLL.gigade.Model.Custom
{
    public class ProductCategorySetCustomTemp : ProductCategorySetTemp
    {
        public string Category_Name { get; set; }
        public ProductCategorySetCustomTemp()
        {
            Category_Name = string.Empty;
        }
    }
}
