using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class SalesUser
    {
       public uint s_id { set; get; }
       public uint user_id { set; get; }
       public uint status { set; get; }
       public uint type { set; get; }
       public uint creator { set; get; }
       public uint createdate { set; get; }
       public uint modifier { set; get; }
       public uint modify_time { set; get; }
       public SalesUser()
       {
           s_id = 0;
           user_id = 0;
           status = 0;
           type = 0;
           creator = 0;
           modifier = 0;
           modify_time = 0;
       }
    }
}
