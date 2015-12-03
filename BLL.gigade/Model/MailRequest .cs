using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class MailRequest : PageBase
    { 
        public int request_id { get; set; }
        public int priority { get; set; }
        public int user_id { get; set; }
        public string sender_address { get; set; }
        public string sender_name { get; set; }
        public string receiver_address { get; set; }
        public string receiver_name { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public int importance { get; set; }
        public DateTime schedule_date { get; set; }
        public DateTime valid_until_date { get; set; }
        public int retry_count { get; set; }
        public DateTime last_sent { get; set; }
        public DateTime next_send { get; set; }
        public int max_retry { get; set; }
        public string sent_log { get; set; }
        public string success_action { get; set; }
        public string fail_action { get; set; }
        public DateTime request_createdate { get; set; }
        public DateTime request_updatedate { get; set; }

        public string extra_send { get; set; }
        public string extra_no_send { get; set; }
        public bool is_outer { get; set; }
        public int group_id { get; set; }
        public int content_id { get; set; }

        public string bodyData { get; set; }
        public int template_id { get; set; }

        public MailRequest()
        {
            request_id = 0;
            priority = 0;
            group_id = 0;
            content_id = 0;
            user_id = 0;
            sender_address = string.Empty;
            sender_name = string.Empty;
            receiver_address = string.Empty;
            receiver_name = string.Empty;
            subject = string.Empty;
            body = string.Empty;
            importance = 0;
            schedule_date = DateTime.Now;
            valid_until_date = DateTime.Now;
            retry_count = 0;
            last_sent = DateTime.Now;
            next_send = DateTime.Now;
            max_retry = 0;
            sent_log = string.Empty;
            request_createdate = DateTime.Now;
            request_updatedate = DateTime.Now;

            extra_send = string.Empty;
            extra_no_send = string.Empty;
            is_outer = true;
            bodyData = string.Empty;
            template_id = 0;
        }

    }
}
