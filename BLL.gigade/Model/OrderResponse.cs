using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class OrderResponse:PageBase
    {
        public uint response_id { get; set; }
        public uint question_id { get; set; }
        public uint response_type { get; set; }
        public uint user_id { get; set; }
        public string response_content { get; set; }
        public string response_ipfrom { get; set; }
        public uint response_createdate { get; set; }

        public OrderResponse()
    {
            response_id = 0;
            question_id = 0;
            response_type = 0;
            user_id = 0;
            response_content = string.Empty;
            response_ipfrom = string.Empty;
            response_createdate = 0;

        }

    }
}
