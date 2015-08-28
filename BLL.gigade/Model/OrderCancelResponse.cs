using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class OrderCancelResponse:PageBase
    {
        public uint response_id { get; set; }
        public uint cancel_id { get; set; }
        public uint user_id { get; set; }
        public string response_content { get; set; }
        public uint response_createdate { get; set; }
        public string response_ipfrom { get; set; }

        public OrderCancelResponse()
        {
            response_id = 0;
            cancel_id = 0;
            user_id = 0;
            response_content = string.Empty;
            response_createdate = 0;
            response_ipfrom = string.Empty;
        }
    }
}
