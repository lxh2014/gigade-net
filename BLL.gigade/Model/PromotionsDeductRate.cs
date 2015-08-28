using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class PromotionsDeductRate : PageBase
    {

        public int id { get; set; }
        public string name { get; set; }
        public int group_id { get; set; }
        public int amount { get; set; }
        public int bonus_type { get; set; }
        public int dollar { get; set; }
        public int point { get; set; }
        public int rate { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public bool active { get; set; }
        public int condition_id { get; set; }
        public int status { get; set; }
        public int kuser { get; set; }
        public int muser { get; set; }
        public PromotionsDeductRate()
        {
            id = 0;
            name = string.Empty;
            group_id = 0;
            amount = 0;
            bonus_type = 0;
            dollar = 1;
            point = 1;
            rate = 0;
            start = DateTime.MinValue;
            end = DateTime.MinValue;
            created = DateTime.MinValue;
            modified = DateTime.MinValue;
            active = false;
            status = 1;
            condition_id = 0;
            kuser = 0;
            muser = 0;
        }
    }
}
