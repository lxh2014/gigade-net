using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    [Serializable]
    public class OrderCancelMaster : PageBase
    {
        public uint cancel_id { get; set; }
        public uint order_id { get; set; }
        public uint cancel_status { get; set; }
        public string cancel_note { get; set; }
        public string bank_note { get; set; }
        public DateTime cancel_createdate { get; set; }
        public DateTime cancel_updatedate { get; set; }
        public string cancel_ipfrom { get; set; }
        public OrderCancelMaster()
        {
            cancel_id = 0;
            order_id = 0;
            cancel_status = 0;
            cancel_note = string.Empty;
            bank_note = string.Empty;
            cancel_ipfrom = string.Empty;
        }
    }
}
