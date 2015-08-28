using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductCategorySetTemp : ProductCategorySet
    {
        public new string Product_Id { get; set; }
        public int Writer_Id { get; set; }
        public int Combo_Type { get; set; }
        public ProductCategorySetTemp()
        {
            Product_Id = "0";
            Writer_Id = 0;
            Combo_Type = 0;
        }
    }
}
