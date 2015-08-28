using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class TicketDetailQuery : TicketDetail
    {
        public string TimeStart { set; get; }
        public string TimeEnd { set; get; }
        public int flag { set; get; }
        public int MDID { set; get; }//master或detail_id
        public string ticket_code { set; get; }
        public TicketDetailQuery()
        {
            flag = 0;
            TimeStart = string.Empty;
            TimeEnd = string.Empty;
            MDID = 0;
            ticket_code = string.Empty;
        }
    }
} 
