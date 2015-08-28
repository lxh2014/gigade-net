using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ScheduleRelation
    {
        public int rowid { get; set; }
        public int schedule_id { get; set; }
        public string relation_table { get; set; }
        public int relation_id { get; set; }
        public Schedule schedule { get; set; }

        public ScheduleRelation()
        {
            rowid = 0;
            schedule_id = 0;
            relation_table = string.Empty;
            relation_id = 0;
            schedule = new Schedule();
        }
    }
}
