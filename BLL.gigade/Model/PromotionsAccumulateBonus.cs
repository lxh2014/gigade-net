using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class PromotionsAccumulateBonus : PageBase
    {
        public uint id { get; set; }
        public string name { get; set; }
        public int group_id { get; set; }
        public DateTime startTime { get; set; }
        public DateTime end { get; set; }
        public uint bonus_rate { get; set; }
        public int extra_point { get; set; }
        public int bonus_expire_day { get; set; }
        public bool new_user { get; set; }
        public uint repeat { get; set; }
        public int present_time { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public bool active { get; set; }
        public int muser { get; set; }
        public string event_desc { get; set; }
        public string event_type { get; set; }
        public int condition_id { get; set; }
        public int device { get; set; }
        public string payment_code { get; set; }
        public int kuser { get; set; }
        public DateTime new_user_date { get; set; }
        public int status { get; set; }
        public PromotionsAccumulateBonus()
        {
            id = 0;
            name = string.Empty;
            group_id = 0;
            device = 1;
            bonus_rate = 1;
            present_time = 0;
            payment_code = string.Empty;
            kuser = 0;
            startTime = DateTime.MinValue;
            end = DateTime.MinValue;
            created = DateTime.MinValue;
            modified = DateTime.MinValue;
            active = false;
            new_user_date = DateTime.MinValue;
            condition_id = 0;
            status = 1;
        }
    }
}
