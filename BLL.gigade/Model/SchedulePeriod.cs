using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class SchedulePeriod : PageBase
    {
        public int rowid { get; set; }
        public int schedule_id { get; set; } //
        public string period_type { get; set; } //排程輪詢方式
        public int year { get; set; }
        public int month { get; set; }
        public int week { get; set; }
        public int day { get; set; }
        public int hour { get; set; }
        public int minute { get; set; }
        public int begin_datetime { get; set; }
        public int current_nums { get; set; }
        public int limit_nums { get; set; }
        public int create_user { get; set; }
        public int create_time { get; set; }
        public int change_user { get; set; }
        public int change_time { get; set; }

        public SchedulePeriod()
        {
            rowid = 0;
            schedule_id = 0;
            period_type = string.Empty;
            year = 0;
            month = 0;
            week = 0;
            day = 0;
            hour = 0;
            minute = 0;
            begin_datetime = 0;
            limit_nums = 0;
            create_user = 0;
            create_time = 0;
            change_user = 0;
            change_time = 0;

        }
    }
}
