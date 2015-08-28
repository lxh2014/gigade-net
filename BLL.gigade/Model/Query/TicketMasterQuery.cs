using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class TicketMasterQuery : TicketMaster
    {
        public int course_id { set; get; }
        public string course_name { set; get; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public int order_start { get; set; }//用於查詢條件
        public int order_end { get; set; }

        public int vendor_id { get; set; }
        public int date { get; set; }
        public string ticketidorname { get; set; }
        public string select_ticket_con { get; set; }
      public string  order_payment_string { get; set; }
         public string s_order_createdate { get; set; }
         public string isSecret { get; set; }
         public int ticket_detail_id { get; set; }
         public int detail_status { get;set;}
        public TicketMasterQuery()
        {
            course_id = 0;
            course_name = string.Empty;
            start_date = DateTime.MinValue;
            end_date = DateTime.MinValue;
            order_start = 0;
            order_end = 0;
            ticketidorname = string.Empty;
            select_ticket_con = string.Empty;
            s_order_createdate = string.Empty;
            isSecret = string.Empty;
            ticket_detail_id = 0;
            detail_status = 0;
        }
    }
}
