using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ScheduleLog : PageBase
    {
        public int rowid { get; set; }
        public string schedule_code { get; set; } //排程代碼
        public int schedule_period_id { get; set; }//Schedule_Period表rowid
        public int create_user { get; set; }
        public int create_time { get; set; }
        public int state { get; set; }
        public decimal request_cost { get; set; }
        public string ipfrom { get; set; }

        public ScheduleLog()
        {
            rowid = 0;
            schedule_code = string.Empty;
            schedule_period_id = 0;
            create_user = 0;
            create_time = 0;
            state = 0;
            request_cost = 0;
            ipfrom = string.Empty;

        }
    }
}
