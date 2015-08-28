using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class NewPromoRecord : PageBase
    {
        public int row_id { get; set; }
        public string event_id { get; set; }
        public string message { get; set; }
        public string event_type { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string ip { get; set; }
        public DateTime create_time { get; set; }
        public string user_tel { get; set; }
        public string user_address { get; set; }
        public string user_mail { get; set; }
        public DateTime user_reg_date { get; set; }

        public NewPromoRecord()
        {
            row_id = 0;
            event_id = string.Empty;
            message = string.Empty;
            event_type = string.Empty;
            user_id = 0;
            user_name = string.Empty;
            ip = string.Empty;
            create_time = DateTime.MinValue;
            user_tel = string.Empty;
            user_address = string.Empty;
            user_mail = string.Empty;
            user_reg_date = DateTime.MinValue;
        }
    }

}
