using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
  public  class HGLogin
    {
      public uint order_id { get; set; }
      public string merchant_pos { get; set; }
      public string terminal_pos { get; set; }
      public int response_code { get; set; }
      public string response_message { get; set; }
      public string enc_idno { get; set; }
      public string chk_sum { get; set; }
      public int remain_point { get; set; }
      public string token { get; set; }
      public string mask_name { get; set; }
      public string mask_id { get; set; }
      public int transaction_time { get; set; }
      public int createdAt { get; set; }

      public HGLogin()
      {
          order_id = 0;
          merchant_pos = string.Empty;
          terminal_pos = string.Empty;
          response_code = 0;
          response_message = string.Empty;
          enc_idno = string.Empty;
          chk_sum = string.Empty;
          remain_point = 0;
          token = string.Empty;
          mask_name = string.Empty;
          mask_id = string.Empty;
          transaction_time = 0;
          createdAt = 0;
      }
    }
}
