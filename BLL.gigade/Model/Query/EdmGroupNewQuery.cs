using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public  class EdmGroupNewQuery:EdmGroupNew
    {
       public string is_member_edm_string { get; set; }
       public string group_name_list { get; set; }

       public  EdmGroupNewQuery()
       {
           is_member_edm_string=string.Empty;
           group_name_list = string.Empty;
       }
    }
}
