using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class PromoTicket : PageBase
    {
        public int rid { get; set; }
        public string ticket_name { get; set; }
        public string event_id { get; set; }
        public string event_type { get; set; }
        public int active_now { get; set; }
        public int valid_interval { get; set; }
        public DateTime use_start { get; set; }
        public DateTime use_end { get; set; }
        public string kuser { get; set; }
        public DateTime kdate { get; set; }
        public string muser { get; set; }
        public DateTime mdate { get; set; }
        public int status { get; set; }

        public PromoTicket()
        {
            rid = 0;
            ticket_name = string.Empty;
            event_id = string.Empty;
            event_type = string.Empty;
            active_now = 0;
            valid_interval = 0;
            use_start = DateTime.MinValue;
            use_end = DateTime.MinValue;
            kuser = string.Empty;
            kdate = DateTime.MinValue;
            muser = string.Empty;
            mdate = DateTime.MinValue;
            status = 1;
        }

    }

}
