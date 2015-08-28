using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class LogisticsDetail:PageBase
    {
        public int rid { get; set; }
        public int deliver_id { get; set; }
        public int delivery_store_id { get; set; }
        public int logisticsType { get; set; }
        public DateTime set_time { get; set; }
        public LogisticsDetail()
        {
            rid = 0;
            deliver_id = 0;
            delivery_store_id = 0;
            logisticsType = 0;
            set_time = DateTime.MinValue;
        }
    }
}
