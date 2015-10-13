using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ScheduleConfig : PageBase
    {
        public int rowid { get; set; }
        public string schedule_code { get; set; } //排程代碼
        public string parameterCode { get; set; } //
        public string value { get; set; }
        public string parameterName { get; set; }
        public int create_user { get; set; }
        public int create_time { get; set; }
        public int change_user { get; set; }
        public int change_time { get; set; }

        public ScheduleConfig()
        {
            rowid = 0;
            schedule_code = string.Empty;
            parameterCode = string.Empty;
            value = string.Empty;
            parameterName = string.Empty;
            create_user = 0;
            create_time = 0;
            change_user = 0;
            change_time = 0;

        }
    }
}
