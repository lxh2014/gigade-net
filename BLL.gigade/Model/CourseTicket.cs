using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class CourseTicket : PageBase
    {
        public int ticket_id { get; set; }
        public int ticket_detail_id { get; set; }
        public string ticket_code { get; set; }
        public DateTime modify_date { get; set; }
        public int modify_user { get; set; }
        public int flag { get; set; } //票券狀態(0:未使用 1:已使用)
        public CourseTicket()
        {
            ticket_id = 0;
            ticket_detail_id = 0;
            ticket_code = string.Empty;
            modify_date = DateTime.MinValue;
            modify_user = 0;
            flag = 0;
        }
    }
}
