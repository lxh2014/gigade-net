using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public class UsersLoginQuery:UsersLogin
    {
       public string username { get; set; }
       public DateTime serchstart { get; set; }
       public DateTime serchend { get; set; }
       public DateTime slogin_createdate { get; set; }
       public UsersLoginQuery()
       {
           username = string.Empty;
           serchstart = DateTime.MinValue;
           serchend = DateTime.MinValue;
           slogin_createdate = DateTime.MinValue;
       }
    }
}
