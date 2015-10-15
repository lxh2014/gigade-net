using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class SchedulePeriod : PageBase
    {
        public int rowid { get; set; }
        public string schedule_code { get; set; } //
        public uint period_type { get; set; } //排程輪詢方式
        public uint period_nums { get; set; }
        public int begin_datetime { get; set; }
        public uint current_nums { get; set; }
        public uint limit_nums { get; set; }
        public int create_user { get; set; }
        public int create_time { get; set; }
        public int change_user { get; set; }
        public int change_time { get; set; }

        public SchedulePeriod()
        {
            rowid = 0;
            schedule_code = string.Empty;
            period_type = 0;
            period_nums = 0;
            begin_datetime = 0;
            limit_nums = 0;
            create_user = 0;
            create_time = 0;
            change_user = 0;
            change_time = 0;

        }
    }
}
