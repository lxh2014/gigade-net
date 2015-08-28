using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class OrderQuestion:PageBase
    {
       public uint question_id { get; set; }
       public uint order_id { get; set; }
       public string question_username { get; set; }
       public string question_phone { get; set; }
       public string question_email { get; set; }
       public uint question_type { get; set; }
       public string question_reply { get; set; }
       public int question_reply_time { get; set; }
       public uint question_status { get; set; }
       public string question_content { get; set; }
       public string question_ipfrom { get; set; }
       public uint question_createdate { get; set;}
       public string question_file { get; set; }
       public OrderQuestion()
       {
           question_id = 0;
           order_id = 0;
           question_username = string.Empty;
           question_phone = string.Empty;
           question_email = string.Empty;
           question_type = 0;
           question_reply = string.Empty;
           question_reply_time = 0;
           question_status = 0;
           question_content = string.Empty;
           question_ipfrom = string.Empty;
           question_createdate = 0;
           question_file = string.Empty;

       }
    }
}
