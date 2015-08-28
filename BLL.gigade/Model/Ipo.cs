using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class Ipo:PageBase
    {
       public int row_id { set; get; }
       public string po_id { set;get; }
       public string vend_id { set; get; }
       public string buyer { set; get; }
       public DateTime sched_rcpt_dt { set; get; }
       public string po_type { set; get; }
       public string po_type_desc { set; get; }
       public DateTime cancel_dt { set; get; }
       public string msg1 { set; get; }
       public string msg2 { set; get; }
       public string msg3 { set; get; }
       public int create_user { set; get; }
       public DateTime create_dtim { set; get; }
       public int change_user{ set; get; }
       public DateTime change_dtim { set; get; }
       public int status { set; get; }
       public Ipo()
       {
           row_id = 0;
           po_id = string.Empty;
           vend_id = string.Empty;
           buyer = string.Empty;
           sched_rcpt_dt = DateTime.MinValue;
           po_type = string.Empty;
           po_type_desc = string.Empty;
           cancel_dt = DateTime.MinValue;
           msg1 = string.Empty;
           msg2 = string.Empty;
           msg3 = string.Empty;
           create_user = 0;
           create_dtim = DateTime.MinValue;
           change_user = 0;
           change_dtim = DateTime.MinValue;
           status = 0;
       
       }
    }
}
