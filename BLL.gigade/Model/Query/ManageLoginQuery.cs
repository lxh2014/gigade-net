using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ManageLoginQuery : ManageLogin
    {
        public DateTime login_createtime { set; get; }
        public string user_name { set; get; }
        public UInt32 loginID { set; get; }
        public long login_start { set; get; }
        public long login_end { set; get; }
        public ManageLoginQuery()
        {
            login_createtime = DateTime.MinValue;
            user_name = string.Empty;
            loginID = 0;
            login_start = 0;
            login_end = 0;
        }
    }
}
