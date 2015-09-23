using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class MailSender
    {
       public int sender_id { get; set; }
       public string sender_email { get; set; }
       public string sender_name { get; set; }
       public DateTime sender_createdate { get; set; }
       public DateTime sender_updatedate { get; set; }
       public int sender_create_userid { get; set; }
       public int sender_update_userid { get; set; }

       public MailSender()
       {
           sender_id = 0;
           sender_email = string.Empty;
           sender_name = string.Empty;
           sender_createdate = DateTime.Now;
           sender_updatedate = DateTime.Now;
           sender_create_userid = 0;
           sender_update_userid = 0;
       }
    }
}
