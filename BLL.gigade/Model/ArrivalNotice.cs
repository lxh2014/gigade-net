using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public  class ArrivalNotice:PageBase
    {
       public uint id { get; set; }
       public uint user_id { get; set; }
       public uint item_id { get; set; }
       public uint product_id { get; set; }
       public int status { get; set; }
       public uint create_time { get; set; }
       public int coming_time { get; set; }
       public string s_coming_time { get; set; }
       public string recommend { get; set; }
       public ArrivalNotice()
       {
           id = 0;
           user_id = 0;
           item_id = 0;
           product_id = 0;
           status = 0;
           create_time = 0;
           coming_time = 0;
           s_coming_time = string.Empty;
           recommend = string.Empty;
       }
    }
}
