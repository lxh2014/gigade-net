using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public class IlocQuery:Iloc
    {
       public string change_users { get; set; }
       public string startiloc { get; set; }
       public string endiloc { get; set; }
       public DateTime starttime { get; set; }
       public DateTime endtime { get; set; }

       public string abd;

       public IlocQuery()
       {
           change_users = string.Empty;
           startiloc = string.Empty;
           endiloc = string.Empty;
       }
    }
}
