using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class MailLog
    {
        public int priority { get; set; }
        //public int group_id { get; set; }
        //public int content_id { get; set; }
        public int user_id { get; set; }
        public string sender_address { get; set; }
        public string sender_name { get; set; }
        public string receiver_address { get; set; }
        public string receiver_name { get; set; }
        public string subject { get; set; }
        public int importance { get; set; }
        public DateTime schedule_date { get; set; }
        public DateTime valid_until_date { get; set; }
        public int retry_count { get; set; }
        public DateTime last_sent { get; set; }
        public string sent_log { get; set; }
        public int send_result { get; set; }
        public DateTime request_createdate { get; set; }
        public DateTime request_updatedate { get; set; }
        public DateTime log_createdate { get; set; }
         
        public MailLog()
        {

        }
    }

}
