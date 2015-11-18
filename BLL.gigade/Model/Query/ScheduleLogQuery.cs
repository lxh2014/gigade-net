using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ScheduleLogQuery : ScheduleLog
    {
        public string create_username { get; set; }
        public string show_create_time { get; set; }
        public int start_time { get; set; }
        public int end_time { get; set; }
        public string schedule_name { get; set; }
        public ScheduleLogQuery()
        {
            create_username = string.Empty;
            show_create_time = string.Empty;
            start_time = 0;
            end_time = 0;
            schedule_name = string.Empty;
        }
    }
}
