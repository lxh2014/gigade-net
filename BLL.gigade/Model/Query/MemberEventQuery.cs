using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class MemberEventQuery : MemberEvent
    {
        public string ml_id { get; set; }
        public string memberName { get; set; }
        public string ml_name { get; set; }
        //活動類型表
        public string et_name { get; set; }
        public string et_date_parameter { get; set; }
        public string et_starttime { get; set; }
        public string et_endtime { get; set; }
        public string timestart { get; set; }
        public string timeend { get; set; }

        public string s_me_banner_link { get; set; }
        public MemberEventQuery()
        {
            ml_id = string.Empty;
            memberName = string.Empty;
            ml_name = string.Empty;
            et_name = string.Empty;
            et_date_parameter = string.Empty;
            et_starttime = string.Empty;
            et_endtime = string.Empty;
            timeend = string.Empty;
            timestart = string.Empty;
            s_me_banner_link = string.Empty;
        }
    }
}
