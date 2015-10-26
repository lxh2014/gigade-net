using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EdmTraceEmail : PageBase
    {
        public int email_id { get; set; }
        public string email { get; set; }
        public string name { get; set; }


        public EdmTraceEmail()
        {
            email_id = 0;
            email = string.Empty;
            name = string.Empty;
        }
    }
}
