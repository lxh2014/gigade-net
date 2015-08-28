using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ManageUser : PageBase
    {
        public string name { get; set; }
        public string callid { get; set; }
        public uint user_id { get; set; }
        public string user_username { get; set; }
        public string user_email { get; set; }
        public string user_delete_email { get; set; }
        public string user_password { get; set; }
        public string user_confirm_code { get; set; }
        public uint user_status { get; set; }
        public uint user_login_attempts { get; set; }
        public uint user_lastvisit { get; set; }
        public uint user_last_login { get; set; }
        public uint manage { get; set; }
        public uint user_createdate { get; set; }
        public uint user_updatedate { get; set; }
        public string erp_id { get; set; }

        public ManageUser()
        {
            name = string.Empty;
            callid = string.Empty;
            user_id = 0;
            user_delete_email = string.Empty;
        }
    }
}
 