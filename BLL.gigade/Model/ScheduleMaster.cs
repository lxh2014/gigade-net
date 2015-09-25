using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ScheduleMaster : PageBase
    {
        public int rowid { get; set; }
        public string schedule_code { get; set; } //排程代碼
        public string schedule_name { get; set; } //排程名稱
        public string api { get; set; }
        public string description { get; set; }
        public int state { get; set; }
        public int previous_execute_time { get; set; }
        public int next_execute_time { get; set; }
        public int schedule_period_id { get; set; }
        public int create_user { get; set; }
        public int create_time { get; set; }
        public int change_user { get; set; }
        public int change_time { get; set; }

        public ScheduleMaster()
        {
            rowid = 0;
            schedule_code = string.Empty;
            schedule_name = string.Empty;
            api = string.Empty;
            description = string.Empty;
            state = 0;
            previous_execute_time = 0;
            next_execute_time = 0;
            schedule_period_id = 0;
            create_user = 0;
            create_time = 0;
            change_user = 0;
            change_time = 0;

        }
    }
}
