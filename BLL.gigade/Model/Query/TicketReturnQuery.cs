using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class TicketReturnQuery : TicketReturn
    {
        public int trcl_id { get; set; }
        public int trcl_last_status { get; set; }
        public int trcl_new_status { get; set; }
        public int trcl_new_money { get; set; }
        public int trcl_last_money { get; set; }
        public int trcl_create_user { get; set; }
        public int trcl_create_date { get; set; }
        public string trcl_note { get; set; }
        public string parameterCode { get; set; }
        public string parameterName { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public TicketReturnQuery()
        {
            trcl_id = 0;
            tr_id = 0;
            trcl_last_status = 0;
            trcl_new_status = 0;
            trcl_new_money = 0;
            trcl_last_money = 0;
            trcl_create_user = 0;
            trcl_create_date = 0;
            trcl_note = string.Empty;
            parameterName = string.Empty;
            parameterCode = string.Empty;
            start_date = DateTime.MinValue;
            end_date = DateTime.MinValue;
        }
    }
}
