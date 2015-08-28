using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class OrderCancelMsg:PageBase
    {
        public uint cancel_id { get; set; }
        public uint order_id { get; set; }
        public uint cancel_type { get; set; }
        public uint cancel_status { get; set; }
        public string cancel_content { get; set; }
        public DateTime cancel_createdate { get; set; }
        public string cancel_ipfrom { get; set; }


        public OrderCancelMsg()
        {
            cancel_id = 0;
            order_id = 0;
            cancel_type = 0;
            cancel_status = 0;
            cancel_content = string.Empty;
            cancel_createdate =DateTime.Now;
            cancel_ipfrom = string.Empty;
        }
    }
}
