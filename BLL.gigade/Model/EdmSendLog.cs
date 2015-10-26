using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EdmSendLog : PageBase
    {
        public int log_id { get; set; }
        public int content_id { get; set; }
        public int test_send { get; set; }
        public int receiver_count { get; set; }
        public DateTime schedule_date { get; set; }
        public DateTime expire_date { get; set; }
        public DateTime createdate { get; set; }
        public int create_userid { get; set; }

        public bool test_send_end { get; set; }
        public int elcm_id { get; set; }
        public int email_group_id { get; set; }
        public EdmSendLog()
        {
            log_id = 0;
            content_id = 0;
            test_send = 0;
            receiver_count = 0;
            schedule_date = DateTime.Now;
            expire_date = DateTime.Now;
            createdate = DateTime.Now;
            create_userid = 0;
            test_send_end = true;
            elcm_id = 0;
            email_group_id = 0;
        }
    }
}
