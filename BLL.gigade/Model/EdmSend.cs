using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EdmSend:PageBase
    {
        public uint email_id { get; set; }
        public uint send_status { get; set; }
        public string email_name { get; set; }
        public uint open_total { get; set; }
        public uint send_datetime { get; set; }
        public uint open_first { get; set; }
        public uint open_last { get; set; }
        public EdmSend()
        {
            email_id = 0;
            send_status = 0;
            email_name = string.Empty;
            open_total = 0;
            send_datetime = 0;
            open_first = 0;
            open_last = 0;
        }
    }
}
