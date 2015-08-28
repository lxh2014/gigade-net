using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
 public   class EdmContent:PageBase
    {
     public uint content_id { get; set; }
     public uint group_id { get; set; }
     public uint content_status { get; set; }
     public uint content_email_id { get; set; }
     public uint content_start { get; set; }
     public uint content_end { get; set; }
     public uint content_range { get; set; }
     public uint content_single_count { get; set; }
     public uint content_click { get; set; }
     public uint content_person { get; set; }
     public uint content_send_success { get; set; }
     public uint content_send_failed { get; set; }
     public string content_from_name { get; set; }
     public string content_from_email { get; set; }
     public string content_reply_email { get; set; }
     public uint content_priority { get; set; }
     public string content_title { get; set; }
     public string content_body { get; set; }
     public uint content_createdate { get; set; }
     public uint content_updatedate { get; set; }
     public int info_epaper_id { get; set; }
     public EdmContent()
     {
         content_id = 0;
         group_id = 0;
         content_status = 1;
         content_email_id = 0;
         content_start = 0;
         content_end = 0;
         content_range =300;
         content_single_count = 100;
         content_click = 0;
         content_person = 0;
         content_send_success = 0;
         content_send_failed = 0;
         content_from_name = string.Empty;
         content_from_email = string.Empty;
         content_reply_email = string.Empty;
         content_priority = 3;
         content_title = string.Empty;
         content_body = string.Empty;
         content_createdate = 0;
         content_updatedate = 0;
         info_epaper_id = 0;
     }
    }
}
