using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class ScheduleItemCustom : ScheduleItem
    {
        public string schedule_name { get; set; }
        public string desc { get; set; }
        public string tabType { get; set; }
        public string keyStr { get; set; }
        public string valueStr { get; set; }
        public ScheduleItemCustom() {
            schedule_name = string.Empty;
            desc = string.Empty;
            tabType = string.Empty;
            keyStr = string.Empty;
            valueStr = string.Empty;
        }
    }
}
