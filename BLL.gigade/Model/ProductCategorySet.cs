using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    [DBTableInfo("product_category_set", PK_Col = "product_id")]
    public class ProductCategorySet
    {
        public int Id { get; set; }
        public uint Product_Id { get; set; }
        public uint Category_Id { get; set; }
        public uint Brand_Id { get; set; }
        public ProductCategorySet()
        {
            Id = 0;
            Product_Id = 0;
            Category_Id = 0;
            Brand_Id = 0;
        }
    }
}
