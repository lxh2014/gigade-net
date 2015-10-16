using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
  public  class EmailGroup:PageBase
    {
      public int group_id { get; set; }
      public string  group_name { get; set; }
      public DateTime group_createdate { get; set; }
      public DateTime group_updatedate { get; set; }
      public int group_create_userid { get; set; }
      public int group_update_userid { get; set; }

      public string email_address { get; set; }
      public string name { get; set; }
      public Int64 count { get; set; }
      public string user_username { get; set; }
      public EmailGroup()
      {
          group_id = 0;
          group_name = string.Empty;
          group_createdate =DateTime.Now;
          group_updatedate = DateTime.Now;
          group_create_userid = 0;
          group_update_userid = 0;
          email_address = string.Empty;
          name = string.Empty;
          count = 0;
          user_username = string.Empty;
      }
    }
}
