using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public class SalesUserQuery:SalesUser
    {
       public string user_name { set; get; }
       public SalesUserQuery()
       {
           user_name = string.Empty;
       }

    }
}
