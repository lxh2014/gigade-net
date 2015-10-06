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
        public int create_user { get; set; }
        public int create_time { get; set; }
        public string ipfrom { get; set; }

        public ScheduleLog()
        {
            rowid = 0;
            schedule_code = string.Empty;
            create_user = 0;
            create_time = 0;
            ipfrom = string.Empty;

        }
    }
}
