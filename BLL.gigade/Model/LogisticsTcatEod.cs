using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class LogisticsTcatEod : PageBase
    {
        public int delivery_type { get; set; }
        public string delivery_number { get; set; }
        public int order_id { get; set; }
        public string freight_set { get; set; }
        public string delivery_distance { get; set; }
        public string package_size { get; set; }
        public int cash_collect_service { get; set; }
        public int cash_collect_amount { get; set; }
        public string receiver_zip { get; set; }
        public string receiver_address { get; set; }
        public DateTime delivery_date { get; set; }
        public string estimate_arrival { get; set; }
        public string package_name { get; set; }
        public string delivery_note { get; set; }
        public int create_user_id { get; set; }
        public DateTime create_date { get; set; }
        public DateTime upload_time { get; set; }

        public LogisticsTcatEod()
        {
            delivery_type = 0;
            delivery_number = string.Empty;
            order_id = 0;
            freight_set = string.Empty;
            delivery_distance = string.Empty;
            package_size = string.Empty;
            cash_collect_service = 0;
            cash_collect_amount = 0;
            receiver_zip = string.Empty;
            receiver_address = string.Empty;
            delivery_date = DateTime.MinValue;
            estimate_arrival = string.Empty;
            package_name = string.Empty;
            delivery_note = string.Empty;
            create_user_id = 0;
            create_date = DateTime.MinValue;
            upload_time = DateTime.MinValue;

        }
    }
}