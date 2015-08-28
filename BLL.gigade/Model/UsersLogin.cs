using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class UsersLogin:PageBase
    {
       public uint login_id { get; set; }
       public uint user_id { get; set; }
       public string login_ipfrom { get; set; }
       public uint login_createdate { get; set; }
       public UsersLogin()
       {
           login_ipfrom = string.Empty;
           login_createdate = 0;
           user_id = 0;
           login_id = 0;
       }

    }
}
