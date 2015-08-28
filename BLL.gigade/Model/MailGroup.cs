using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class MailGroup:PageBase
    {
        public int row_id { get; set; }
        public string group_name { get; set; }
        public string remark { get; set; }
        public int status { get; set; }
        public DateTime create_time { get; set; }
        public int create_user { get; set; }
        public DateTime update_time { get; set; }
        public int update_user { get; set; }
        public string group_code { get; set; }

        public MailGroup()
        {
            row_id = 0;
            group_name = string.Empty;
            remark = string.Empty;
            status = -1;
            create_time = DateTime.MinValue;
            create_user = 0;
            update_time = DateTime.MinValue;
            update_user = 0;
            group_code = string.Empty;
        }
    }
}
