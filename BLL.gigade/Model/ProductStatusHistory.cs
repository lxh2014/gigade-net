using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductStatusHistory : PageBase
    {
        public uint product_id { get; set; }
        public uint user_id { get; set; }
        public DateTime create_time { get; set; }
        public uint type { get; set; }
        public int product_status { get; set; }
        public string remark { get; set; }

        public ProductStatusHistory()
        {
            product_id = 0;
            user_id = 0;
            create_time = DateTime.MinValue;
            type = 0;
            product_status = 0;
            remark = string.Empty;
        }
    }
}
