using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class UserLifeCustom
    {
        public uint user_id { get; set; }
        public int user_marriage { get; set; }
        public int child_num { get; set; }
        public int vegetarian_type { get; set; }
        public int like_fivespice { get; set; }
        public string like_contact { get; set; }
        public int like_time { get; set; }
        public int work_type { get; set; }
        public uint cancel_edm_time { get; set; }
        public uint disable_time { get; set; }
        public uint cancel_info_time { get; set; }
        public int user_educated { get; set; }
        public int user_salary { get; set; }
        public int user_religion { get; set; }
        public int user_constellation { get; set; }
        //資料表外字段
        public string cancel_edm_date { get; set; }//net 時間
        public string disable_date { get; set; }//net 時間
        public string cancel_info_date { get; set; }//net 時間
        public UserLifeCustom()
        {
            user_id = 0;
            user_marriage = 0;
            child_num = 0;
            vegetarian_type = 0;
            like_fivespice = 0;
            like_contact = string.Empty;
            like_time = 0;
            work_type = 0;
            cancel_edm_time = 0;
            disable_time = 0;
            cancel_info_time = 0;
            user_educated = 0;
            user_salary = 0;
            user_religion = 0;
            user_constellation = 0;

            cancel_edm_date = string.Empty;
            disable_date = string.Empty;
            cancel_info_date = string.Empty;
        }
    }
}
