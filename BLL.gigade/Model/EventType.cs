using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EventType : PageBase
    {
        public int et_id { get; set; }
        public string et_name { get; set; }
        public string et_date_parameter { get; set; }
        public DateTime et_starttime { get; set; }
        public DateTime et_endtime { get; set; }
        public EventType()
        {
            et_id = 0;
            et_name = string.Empty;
            et_date_parameter = string.Empty;
            et_starttime = DateTime.MinValue;
            et_endtime = DateTime.MinValue;
        }
    }
}
