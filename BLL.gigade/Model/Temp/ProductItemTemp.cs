using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductItemTemp : ProductItem
    {
        public new string Product_Id { get; set; }
        public int Writer_Id { get; set; }
        public ProductItemTemp()
        {
            Product_Id = "0";
            Writer_Id = 0;
        }
    }
}
