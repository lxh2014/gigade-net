using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class MailUser : PageBase
    {
        public int row_id { set; get; }
        public string user_mail { set; get; }
        public string user_name { set; get; }
        public int status { set; get; }
        public string user_pwd { set; get; }
        public DateTime create_time { set; get; }
        public int create_user { set; get; }
        public DateTime update_time { set; get; }
        public int update_user { set; get; }

        public MailUser()
        {
            row_id = 0; 
            user_mail = string.Empty;
            user_name = string.Empty;
            status = -1;
            user_pwd = string.Empty;
            create_time = DateTime.MinValue;
            create_user = 0;
            update_time = DateTime.MinValue;
            update_user = 0;
        }
    }
}
