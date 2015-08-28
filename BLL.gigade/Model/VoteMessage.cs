using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class VoteMessage : PageBase
    {
        public int message_id { get; set; }
        public int article_id { get; set; }
        public DateTime create_time { get; set; }
        public int create_user { get; set; }
        public string ip { get; set; }
        public int message_status { get; set; }
        public string message_content { get; set; }
        public DateTime update_time { get; set; }
        public int update_user { set; get; }


        public VoteMessage()
        {
            message_id = 0;
            article_id = 0;
            create_time = DateTime.MinValue;
            create_user = 0;
            ip = string.Empty;
            message_status = 0;
            message_content = string.Empty;
            update_time = DateTime.MinValue;
            update_user =0;
        }
    }
}