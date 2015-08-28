using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
  public  class MailGroupMap
    {
      public int row_id { get; set; }
      public int  group_id { get; set; }
      public int user_id { get; set; }
      public int status { get; set; }
      public DateTime create_time { get; set; }
      public int create_user { get; set; }
      public DateTime update_time { get; set; }
      public int update_user { get; set; }

      public MailGroupMap()
      {
          row_id = 0;
          group_id = 0;
          user_id = 0;
          status = -1;
          create_time =DateTime.MinValue;
          create_user = 0;
          update_time = DateTime.MinValue;
          update_user = 0;
      }
    }
}
