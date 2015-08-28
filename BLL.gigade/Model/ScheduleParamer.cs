using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ScheduleParamer:PageBase
    {
        public int para_id { get; set; }
        public string para_value { get; set; }
        public string para_name { get; set; }
        public int para_status { get; set; }
        public string schedule_code { get; set; }

        public ScheduleParamer()
        {
            para_id = 0;
            para_value = string.Empty;
            para_name = string.Empty;
            para_status = 0;
            schedule_code = string.Empty;
        }
    }
}
