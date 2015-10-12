using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class SchedulePeriodQuery : SchedulePeriod
    {
        public string show_create_time { get; set; }
        public string show_change_time { get; set; }
        public string create_username { get; set; }
        public string change_username { get; set; }
        public string show_begin_datetime { get; set; }
        public string schedule_code_period { get; set; }
        public string show_period_type { get; set; }
        public SchedulePeriodQuery()
        {
            show_create_time = string.Empty;
            show_change_time = string.Empty;
            create_username = string.Empty;
            change_username = string.Empty;
            show_begin_datetime = string.Empty;
            schedule_code_period = string.Empty;
            show_period_type = string.Empty;

        }
    }
}
