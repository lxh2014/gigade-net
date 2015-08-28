using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class SmsLog : PageBase
    {
        public int id { get; set; }
        public int sms_id { get; set; }
        public int provider { get; set; }
        public int success { get; set; }
        public string code { get; set; }
        public string free_sms_id { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public SmsLog()
        {
            id = 0;
            sms_id = 0;
            provider=0;
            success=0;
            code=string.Empty;
            free_sms_id=string.Empty;
            created=DateTime.MinValue;
            created=DateTime.MinValue;
        }
    }
}
