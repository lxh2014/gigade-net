using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class PromotionsDeductRateQuery : PromotionsDeductRate
    {
        public string group_name { set; get; }
        public int expired { set; get; }
        public DateTime startdate { set; get; }
        public string points { set; get; }
        public string user_username { get; set; }
        public PromotionsDeductRateQuery()
        {
            group_name = string.Empty;
            expired = 0;
            user_username = string.Empty;
        }
    }
}
