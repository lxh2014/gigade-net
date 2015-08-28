using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class NewPromoQuestionnaire : PageBase
    {
        public int row_id { get; set; }
        public string event_name { get; set; }
        public string event_desc { get; set; }
        public string event_id { get; set; }
        public string present_event_id { get; set; }
        public int group_id { get; set; }
        public string link_url { get; set; }
        public string promo_image { get; set; }
        public string device { get; set; }
        public int count_by { get; set; }
        public int count { get; set; }
        public int active_now { get; set; }
        public int new_user { get; set; }
        public DateTime new_user_date { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public int active { get; set; }
        public int kuser { get; set; }
        public int muser { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
       
        public NewPromoQuestionnaire()
        {
            row_id = 0;
            event_name = string.Empty;
            event_desc = string.Empty;
            event_id = string.Empty;
            present_event_id = string.Empty;
            group_id =0;
            link_url=string.Empty;
            promo_image = string.Empty;
            device = string.Empty;
            count_by = 0;
            count = 0;
            active_now = 0;
            new_user = 0;
            new_user_date = DateTime.MinValue;
            start = DateTime.MinValue;
            end = DateTime.MinValue;
            active = 0;
            kuser = 0;
            muser = 0;
            created = DateTime.MinValue;
            modified = DateTime.MinValue;
            
        }
    }
}
