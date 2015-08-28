using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class UserRecommend : PageBase
    {
        public uint id { get; set; }
        public int event_id { get; set; }
        public int user_id { get; set; }
        public string name { get; set; }
        public string mail { get; set; }
        public string user_ip { get; set; }
        public int recommend_user_id { get; set; }
        public string recommend_user_ip { get; set; }
        public int recommend_commodity_id { get; set; }
        public uint is_recommend { get; set; }
        public DateTime createtime { get; set; }
        public DateTime updatetime { get; set; }



        public UserRecommend()
        {
            id = 0;
            event_id = 0;
            user_id = 0;
            name = string.Empty;
            mail = string.Empty;
            user_ip = string.Empty;
            recommend_user_id = 0;
            recommend_user_ip = string.Empty;
            recommend_commodity_id = 0;
            is_recommend = 0;
            createtime = DateTime.Now;
            updatetime = DateTime.Now;
       
        }
    }
}
