using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Model
{
    [DBTableInfo("sms")]
    public class Sms : PageBase
    {

        public int id { get; set; }
        public int type { get; set; }
        public int serial_id { get; set; }
        public int order_id { get; set; }
        public string mobile { get; set; }
        public string subject { get; set; }
        public string content { get; set; }
        public DateTime estimated_send_time { get; set; }
        public int send { get; set; }
        public string sms_number { get; set; }
        public string trust_send { get; set; }
        public string memo { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }

        public Sms()
        {
            serial_id = 0;
            id = 0;
            type = 0;
            order_id = 0;
            mobile = "";
            subject = "";
            content = "";
            estimated_send_time = DateTime.MinValue;
            send = -1;
            sms_number = "";
            trust_send = "";
            memo = "";
            created = DateTime.MinValue;
            modified = DateTime.MinValue;
        }
    }
}