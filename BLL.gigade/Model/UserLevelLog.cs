using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
      public   class UserLevelLog:PageBase
    {
        public int rowID { get; set; }
          public uint user_id { get; set; }
          public int user_order_amount { get; set; }
          public string ml_code_old { get; set; }
          public string ml_code_new { get; set; }
          public string ml_code_change_type { get; set; }
          public DateTime create_date_time { get; set; }
          public int year { get; set; }
          public int month { get; set; }

          public UserLevelLog()
          {
              rowID = 0;
              user_id = 0;
              user_order_amount = 0;
              ml_code_old =string.Empty;
              ml_code_new = string.Empty;
              ml_code_change_type = string.Empty;
              create_date_time = DateTime.MinValue;
              year = 0;
              month = 0;
          }
    }
}
