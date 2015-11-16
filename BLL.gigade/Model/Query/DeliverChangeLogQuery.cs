using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class DeliverChangeLogQuery : DeliverChangeLog
    {
        
        public DateTime time_start { get; set; }
        public DateTime time_end { get; set; }
        public int dcl_create_type { get; set; }
        public string dcl_create_username { get; set; }
        public string dcl_create_musername { get; set; }
        public string dcl_user_name { get; set; }
        public string dcl_user_email { get; set; }

        public DeliverChangeLogQuery()
        {
            time_start = DateTime.MinValue;
            time_end = DateTime.MinValue;
            dcl_create_type = 0;
            dcl_create_username = string.Empty;
            dcl_create_musername = string.Empty;
            dcl_user_name = string.Empty;
            dcl_user_email = string.Empty;
        }
    }
}
