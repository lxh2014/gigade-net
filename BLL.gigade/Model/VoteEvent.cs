using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class VoteEvent : PageBase
    {
        public int event_id { get; set; }
        public string event_name { get; set; }
        public string event_desc { get; set; }
        public string event_banner { get; set; }
        public DateTime event_start { get; set; }
        public DateTime event_end { get; set; }
        public int word_length { get; set; }
        public int vote_everyone_limit { get; set; }
        public int vote_everyday_limit { get; set; }
        public int number_limit { get; set; }
        public string present_event_id { get; set; }
        public int create_user { get; set; }
        public DateTime create_time { get; set; }
        public int update_user { get; set; }
        public DateTime update_time { get; set; }
        public int event_status { get; set; }
        public int is_repeat { get; set; }
        public VoteEvent()
        {
            event_id = 0;
            event_name = string.Empty;
            event_desc = string.Empty;
            event_banner = string.Empty;
            event_start = DateTime.MinValue;
            event_end = DateTime.MinValue;
            word_length = 0;
            vote_everyone_limit = 0;
            vote_everyday_limit = 0;
            number_limit = 0;
            present_event_id = string.Empty;
            create_user = 0;
            create_time = DateTime.Now;
            update_user = 0;
            update_time = DateTime.Now;
            event_status = 0;
            is_repeat = 0;
        }
    }
}