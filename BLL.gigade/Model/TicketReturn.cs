using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class TicketReturn:PageBase
    {
       public int tr_id { get; set; }
       public int ticket_master_id { get; set; }
       public string tr_note { get; set; }
       public string tr_bank_note { get; set; }
       public int tr_update_user { get; set; }
       public int tr_create_user { get; set; }
       public DateTime tr_create_date { get; set; }
       public DateTime tr_update_date { get; set; }
       public string tr_ipfrom { get; set; }
       public int tr_money { get; set; }
       public int tr_status { get; set; }
       public string tr_reason_type { get; set; }

       public TicketReturn()
       {
           tr_id = 0;
           ticket_master_id = 0;
           tr_note = string.Empty;
           tr_bank_note = string.Empty;
           tr_update_user = 0;
           tr_create_user = 0;
           tr_create_date = DateTime.MinValue;
           tr_update_date = DateTime.MinValue;
           tr_ipfrom = string.Empty;
           tr_money = 0;
           tr_status = 0;
           tr_reason_type = string.Empty;
       }
    }
}
