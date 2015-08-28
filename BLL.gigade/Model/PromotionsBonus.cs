using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class PromotionsBonus : PageBase
    {
        public int id { get; set; }
        public string name { get; set; }
        public int group_id { get; set; }
        public int type { get; set; }
        public int amount { get; set; }
        public int days { get; set; }
        public bool new_user { get; set; }
        public bool repeat { get; set; }
        public bool multiple { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public bool active { get; set; }
        public int condition_id { get; set; }
        public int status { get; set; }
        public int kuser { get; set; }
        public int muser { get; set; }

        public PromotionsBonus()
        {
            id = 0;
            name = string.Empty;
            group_id = 0;
            type = 0;
            amount = 0;
            days = 90;
            new_user = false;
            repeat = false;
            multiple = false;
            start = DateTime.MinValue;
            end = DateTime.MinValue;
            created = DateTime.MinValue;
            modified = DateTime.MinValue;
            active = false;
            condition_id = 0;
            status = 1;
            kuser = 0;
            muser = 0;
        }
    }
}
