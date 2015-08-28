using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
  public  class TrialShareQuery : TrialShare
    {

      public string s_share_time { get; set; }
      public string event_name { get; set; }
      public string real_name { get; set; }
      public string gender { get; set; }

      public TrialShareQuery()
      {
          s_share_time = string.Empty;
          event_name = string.Empty;
          real_name = string.Empty;
          gender = string.Empty;
      }

    }
}
