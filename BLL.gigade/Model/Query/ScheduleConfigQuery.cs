using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ScheduleConfigQuery : ScheduleConfig
    {
        public string create_username { get; set; }
        public string change_username { get; set; }
        public string show_create_time { get; set; }
        public string show_change_time { get; set; }

        public ScheduleConfigQuery()
        {
            create_username = string.Empty;
            change_username = string.Empty;
            show_create_time = string.Empty;
            show_change_time = string.Empty;
        }
    }
}
