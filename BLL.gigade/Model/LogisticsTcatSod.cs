using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class LogisticsTcatSod:PageBase
    {
        public string delivery_number { get; set; }
        public int order_id { get; set; }
        public DateTime delivery_status_time { get; set; }
        public string status_id { get; set; }
        public string station_name { get; set; }
        public string customer_id { get; set; }
        public string status_note { get; set; }
        public string specification { get; set; }
        public DateTime create_date { get; set; }
       
        public LogisticsTcatSod()
        {
            delivery_number = string.Empty;
            order_id = 0;
            delivery_status_time = DateTime.MinValue;
            status_id = string.Empty;
            station_name = string.Empty;
            customer_id = string.Empty;
            status_note = string.Empty;
            specification = string.Empty;
            create_date = DateTime.MinValue;
        }
    }
}
