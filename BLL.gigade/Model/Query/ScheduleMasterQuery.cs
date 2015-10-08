using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ScheduleMasterQuery : ScheduleMaster
    {
        public string create_username { get; set; }
        public string change_username { get; set; }
        public string sschedule_state { get; set; }
        public string show_previous_execute_time { get; set; }
        public string show_next_execute_time { get; set; }
        public string show_create_time { get; set; }
        public string show_change_time { get; set; }

        public ScheduleMasterQuery()
        {
            create_username = string.Empty;
            change_username = string.Empty;
            sschedule_state = string.Empty;
            show_previous_execute_time = string.Empty;
            show_next_execute_time = string.Empty;
            show_create_time = string.Empty;
            show_change_time = string.Empty;
        }
    }
}
