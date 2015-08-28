using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ManageUserQuery : ManageUser
    {
        public string user_name { set; get; }
        public string userid { set; get; }
        public DateTime creattime { set; get; }
        public DateTime updtime { set; get; }
        public DateTime lastlogin { set; get; }
        public string search_status { set; get; }
        public string login_sum { set; get; }

        public ManageUserQuery()
        { 
            user_name = string.Empty;
        }
    }
}
