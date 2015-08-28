using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class ProductStatusHistoryCustom
    {
        public uint product_id { get; set; }
        public string user_username { get; set; }
        public DateTime create_time { get; set; }
        public string type { get; set; }
        public string product_status { get; set; }
        public string current_status { get; set; }
        public string remark { get; set; }
        public int create_channel { get; set; }

        public ProductStatusHistoryCustom()
        {
            product_id = 0;
            user_username = string.Empty;
            create_time = DateTime.MinValue;
            type = string.Empty;
            product_status = string.Empty;
            current_status = string.Empty;
            remark = string.Empty;
            create_channel = 0;
        }
    }
}
