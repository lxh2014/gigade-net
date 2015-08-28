using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class OrderDeliver : PageBase
    {
        public uint deliver_id { get; set; }
        public uint slave_id { get; set; }
        public uint deliver_status { get; set; }
        public uint deliver_store { get; set; }
        public string deliver_code { get; set; }
        public uint deliver_time { get; set; }
        public string deliver_note { get; set; }
        public uint deliver_createdate { get; set; }
        public uint deliver_updatedate { get; set; }
        public string deliver_ipfrom { get; set; }

        public OrderDeliver()
        {
            deliver_id = 0;
            slave_id = 0;
            deliver_status = 0;
            deliver_store = 0;
            deliver_code = string.Empty;
            deliver_time = 0;
            deliver_note = string.Empty;
            deliver_createdate = 0;
            deliver_updatedate = 0;
            deliver_ipfrom = string.Empty;
        }
    }
}
