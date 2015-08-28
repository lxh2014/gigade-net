using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade
{
    public class OrderMasterStatus : PageBase
    {
        public UInt64 serial_id { get; set; }
        public uint order_id { get; set; }
        public uint order_status { get; set; }
        public string status_description { get; set; }
        public string status_ipfrom { get; set; }
        public uint status_createdate { get; set; }
        public OrderMasterStatus()
        {
            serial_id = 0;
            order_id = 0;
            order_status = 0;
            status_description = string.Empty;
            status_createdate = 0;
            status_ipfrom = "127.0.0.1";
        }

    }
}
