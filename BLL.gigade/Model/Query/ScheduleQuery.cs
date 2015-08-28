using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ScheduleQuery : Schedule
    {
        public int SearchType { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public ScheduleQuery()
        {
            Start = DateTime.MinValue;
            End = DateTime.MaxValue;
            SearchType = 0;
        }

    }
}
