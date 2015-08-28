using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class NewsLog:PageBase
    {
        public UInt64 log_id { get; set; }
        public uint news_id { get; set; }
        public uint user_id { get; set; }
        public string log_description { get; set; }
        public string log_ipfrom { get; set; }
        public uint log_createdate { get; set; }

        public NewsLog()
       {
           log_id = 0;
           news_id = 0;
           user_id = 0;
           log_description =string.Empty;
           log_ipfrom = string.Empty;
           log_createdate = 0;

       }
    }
}
