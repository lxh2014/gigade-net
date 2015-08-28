using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
  public  class TrialShare:PageBase
    {
        public int share_id { get; set; }
        public int trial_id { get; set; }
        public int user_id { get; set; }
        public int is_show_name { get; set; }
        public string user_name { get; set; }
        public int user_gender { get; set; }
        public string content { get; set; }
        public DateTime share_time { get; set; }
        public int status { get; set; }


        public TrialShare()
        {
            share_id = 0;
            trial_id = 0;
            user_id = 0;
            is_show_name = 0;
            user_name = string.Empty;
            user_gender = 0;
            content = string.Empty;
            share_time = DateTime.MinValue;
            status = 0;

        }
    }
}
