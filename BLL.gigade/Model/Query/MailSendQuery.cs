using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class MailSendQuery : MailSend
    {
        public string startT { get; set; }
        public string endT   { get; set; }
        public string search { get; set; }
        public void MailSend()
        {
            this.startT = string.Empty;
            this.endT = string.Empty;
            this.search = string.Empty;
        }
    }
}
