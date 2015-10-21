using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EdmTrace:PageBase
    {
        public int log_id { get; set; }
        public int content_id { get; set; }
        public int email_id { get; set; }
        public DateTime first_traceback { get; set; }
        public DateTime last_traceback { get; set; }
        public int count { get; set; }
        public int success { get; set; }

        public EdmTrace()
        {
            log_id = 0;
            content_id = 0;
            email_id = 0;
            first_traceback = DateTime.Now;
            last_traceback = DateTime.Now;
            count = 0;
            success = 0;
        }
    }
}
