
using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace BLL.gigade.Model
{
    //trial_record
    public class TrialRecord : PageBase
    {

        public int record_id { get; set; }
        public int trial_id { get; set; }
        public string user_email { get; set; }
        public int is_show_name { get; set; }
        public string user_name { get; set; }
        public int user_skin { get; set; }
        public string content { get; set; }
        public int status { get; set; }
        public int user_id { get; set; }
        public DateTime share_time { get; set; }
        public DateTime apply_time { get; set; }
        public string kuser { get; set; }
        public DateTime kdate { get; set; }
        public string muser { get; set; }
        public DateTime mdate { get; set; }
       
        public TrialRecord()
        {
            record_id = 0;
            trial_id = 0;
            user_email = string.Empty;
            is_show_name = 0;
            user_name = string.Empty;
            user_skin = 0;
            content = string.Empty;
            status = 1;
            share_time = DateTime.MinValue;
            apply_time = DateTime.MinValue;
            kuser = string.Empty;
            kdate = DateTime.MinValue;
            muser = string.Empty;
            mdate = DateTime.MinValue;
            user_id = 0;

        }

    }
}

