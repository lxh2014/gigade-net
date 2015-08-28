using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductSpecTemp:ProductSpec
    {
        public new string product_id { get; set; }
        public int Writer_Id { get; set; }

        public ProductSpecTemp()
        {
            product_id = "0";
            Writer_Id = 0;
        }
    }
}
