using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EdmEmail : PageBase
    {
        public uint email_id { get; set; }
        public string email_name { get; set; }
        public string email_address { get; set; }
        public int email_check { get; set; }
        public int email_sent { get; set; }     
        public int email_user_unknown { get; set; }
        public int email_click { get; set; }
        public int email_createdate { get; set; }
        public int email_updatedate { get; set; }
        public EdmEmail()
        {
            email_id = 0;
            email_name = string.Empty;
            email_address = string.Empty;
            email_check = 0;
            email_sent = 0;
            email_user_unknown = 0;
            email_click = 0;
            email_createdate = 0;
            email_updatedate = 0;
        }
    }
}
