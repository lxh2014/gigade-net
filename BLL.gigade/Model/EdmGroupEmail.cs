using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class EdmGroupEmail:PageBase
    {
       public uint email_id { get; set; }
       public uint group_id { get; set; }
       public string email_name { get; set; }
       public uint email_status { get; set; }
       public uint email_createdate { get; set; }
       public uint email_updatedate { get; set; }
    }
}
