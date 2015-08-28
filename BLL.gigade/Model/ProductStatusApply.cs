using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductStatusApply
    {
        public uint apply_id { get; set; }
        public uint product_id { get; set; }
        public uint prev_status { get; set; }
        public DateTime apply_time { get; set; }
        public uint online_mode { get; set; }

        public ProductStatusApply()
        {
            apply_id = 0;
            product_id = 0;
            prev_status = 0;
            apply_time = DateTime.MinValue;
            online_mode = 0;
        }
    }
}
