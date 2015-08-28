using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class MemberEvent : PageBase
    {
        public int rowID { get; set; }
        public string me_name { get; set; }
        public string me_desc { get; set; }
        public DateTime me_startdate { get; set; }
        public DateTime me_enddate { get; set; }
        public int et_id { get; set; }
        public int me_birthday { get; set; }
        public string event_id { get; set; }
        public string me_big_banner { get; set; }
        public string me_banner_link { get; set; }
        public int me_bonus_onetime { get; set; }
        public string ml_code { get; set; }
        public int me_status { get; set; }
        public DateTime k_date { get; set; }
        public int k_user { get; set; }
        public DateTime m_date { get; set; }
        public int m_user { get; set; }
        public MemberEvent()
        {
            rowID = 0;
            me_name = string.Empty;
            me_desc = string.Empty;
            me_startdate = DateTime.MinValue;
            me_enddate = DateTime.MinValue;
            me_birthday = 0;
            event_id = string.Empty;
            me_big_banner = string.Empty;
            me_banner_link = string.Empty;
            me_bonus_onetime = 0;
            me_status = 0;
            ml_code = string.Empty;
            k_date = DateTime.MinValue;
            k_user = 0;
            m_date = DateTime.MinValue;
            m_user = 0;
        }
    }
}
