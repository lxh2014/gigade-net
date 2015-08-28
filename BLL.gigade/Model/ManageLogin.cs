using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ManageLogin:PageBase
    {
        public int login_id { set; get; }
        public int user_id { set; get; }
        public string login_ipfrom { set; get; }
        public int login_createdate { set; get; }
        public ManageLogin()
        {
            login_id = 0;
            user_id = 0;
            login_ipfrom = string.Empty;
            login_createdate = 0;
        }
    }
}
