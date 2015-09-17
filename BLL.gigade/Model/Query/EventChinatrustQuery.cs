using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class EventChinatrustQuery : EventChinatrust
    {
        public string create_name { set; get; }
        public string update_name { set; get; }
        public string s_event_banner { set; get; }
        public string event_id_name { set; get; }
        public int dateCondition { set; get; }
        public EventChinatrustQuery()
        {
            create_name = string.Empty;
            update_name = string.Empty;
            s_event_banner = string.Empty;
            event_id_name = string.Empty;
            dateCondition = 0;
        }
    }
}
