using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    [DBTableInfo("user_condition")]
    public class UserCondition : PageBase
    {

        public int condition_id { get; set; }
        public string condition_name { get; set; }
        public int reg_start { get; set; }
        public int reg_end { get; set; }
        public int reg_interval { get; set; }
        public int buy_times_min { get; set; }
        public int buy_times_max { get; set; }
        public int buy_amount_min { get; set; }
        public int buy_amount_max { get; set; }
        public int last_time_start { get; set; }
        public int last_time_end { get; set; }
        public int last_time_interval { get; set; }
        public int join_channel { get; set; }
        public int status { get; set; }
        public DateTime reg_startDateTime { set; get; }
        public DateTime reg_endDateTime { set; get; }
        public DateTime last_time_startDateTime { set; get; }
        public DateTime last_time_endDateTime { set; get; }
        public UserCondition()
        {
            condition_id = 0;
            condition_name = string.Empty;
            reg_start = 0;
            reg_end = 0;
            reg_interval = 0;
            buy_times_min = 0;
            buy_times_max = 0;
            buy_amount_min = 0;
            buy_amount_max = 0;
            last_time_start = 0;
            last_time_end = 0;
            last_time_interval = 0;
            join_channel = 0;
            status =0;
        }
    }
}