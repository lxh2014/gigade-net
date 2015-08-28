using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public class EdmGroupQuery:Model.UserEdm
    {
       public uint group_id { get; set; }
       public string group_name { get; set; }
       public uint group_total_email { get; set; }
       public uint group_createdate { get; set; }
       public uint group_updatedate { get; set; } 
    }
}
