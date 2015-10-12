using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ScheduleLogQuery : ScheduleLog
    {
        public string create_username { get; set; }
        public ScheduleLogQuery()
        {
            create_username = string.Empty;
        }
    }
}
