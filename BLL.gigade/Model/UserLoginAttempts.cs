using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class UserLoginAttempts : PageBase
    {
        public int login_id { get; set; }
        public string login_mail { get; set; }
        public string login_ipfrom { get; set; }
        public int login_createdate { get; set; }
        public int login_type { get; set; }

        public int user_id { get; set; }
        public int slogin_createdate { get; set; }
        public int elogin_createdate { get; set; }
        public int sumtotal { get; set; }
        public int ismail { get; set; }
        public UserLoginAttempts()
        {
            login_id = 0;
            login_mail = string.Empty;
            login_ipfrom = string.Empty;
            login_type = 0;
            login_createdate = 0;
            slogin_createdate = 0;
            elogin_createdate = 0;
            sumtotal = 0;
            user_id = 0;
            ismail = 0;
        }
    }
}
