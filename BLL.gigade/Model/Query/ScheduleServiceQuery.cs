using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ScheduleServiceQuery : ScheduleService
    {
        public string create_username { get; set; }
        public string change_username { get; set; }

        public ScheduleServiceQuery()
        {
            create_username = string.Empty;
            change_username = string.Empty;
        }
    }
}
