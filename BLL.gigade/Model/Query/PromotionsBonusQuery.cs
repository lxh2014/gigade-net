using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class PromotionsBonusQuery : PromotionsBonus
    {
        public int expired { get; set; }//是否過期
        public string group_name { get; set; }
        public string hdname { set; get; }
        public DateTime startbegin { get; set; }
        public string user_username { get; set; }
        public PromotionsBonusQuery()
        {
            expired = 0;
            group_name = string.Empty;
            startbegin = DateTime.MaxValue;
            user_username = string.Empty;
        }
    }
}
