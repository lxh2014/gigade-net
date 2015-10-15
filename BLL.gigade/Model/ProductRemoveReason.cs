using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductRemoveReason:PageBase
    {
        public int row_id { get; set; }
        public uint product_id { get; set; }
        public int product_num { get; set; }
        public string create_name { get; set; }
        public int create_time { get; set; }

        public ProductRemoveReason()
        {
            row_id = 0;
            product_id = 0;
            product_num = 0;
            create_name = string.Empty;
            create_time = 0;
        }
    }
}
