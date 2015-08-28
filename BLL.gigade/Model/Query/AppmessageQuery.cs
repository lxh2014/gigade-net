using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class AppmessageQuery : Appmessage
    {
        public DateTime messagedate_time { get; set; }
        public DateTime msg_start_time { get; set; }
        public DateTime msg_end_time { get; set; }
        public AppmessageQuery()
        {
            messagedate_time = DateTime.MinValue;
            msg_start_time = DateTime.MinValue;
            msg_end_time = DateTime.MinValue;
        }
    }
}
