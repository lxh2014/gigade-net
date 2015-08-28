using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EventChinatrust:PageBase
    {
        public int row_id { set; get; }
        public string event_type { set; get; }
        public string event_id { set; get; }
        public string event_name { set; get; }
        public string event_desc { set; get; }
        public string event_banner { set; get; }
        public DateTime event_start_time { set; get; }
        public DateTime event_end_time { set; get; }
        public int event_active { set; get; }
        public int event_create_user { set; get; }
        public int event_update_user { set; get; }
        public DateTime event_create_time { set; get; }
        public DateTime event_update_time { set; get; }
        public DateTime user_register_time { set; get; }
        public EventChinatrust()
        {
            row_id = 0;
            event_type = string.Empty;
            event_id = string.Empty;
            event_name = string.Empty;
            event_desc = string.Empty;
            event_banner = string.Empty;
            event_start_time = DateTime.MinValue;
            event_end_time = DateTime.MinValue;
            event_active = 0;
            event_create_user = 0;
            event_update_user = 0;
            event_create_time = DateTime.MinValue;
            event_update_time = DateTime.MinValue;
            user_register_time = DateTime.MinValue;
        }
    }
}
