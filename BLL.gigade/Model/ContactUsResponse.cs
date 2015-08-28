using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ContactUsResponse:PageBase
    {
        public uint response_id { get; set; }
        public uint question_id { get; set; }
        public uint response_type { get; set; }
        public uint user_id { get; set; }
        public string response_content { get; set; }
        public int response_createdate { get; set; }
        public string response_ipfrom { get; set; }

        public ContactUsResponse()
        {
            response_id = 0;
            question_id = 0;
            user_id = 0;
            response_content = string.Empty;
            response_createdate = 0;
            response_ipfrom = string.Empty;
        }
    }
}
