using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class PromotionsAccumulateRateQuery : PromotionsAccumulateRate
    {
        public string group_name { get; set; }
        public int expired { get; set; }
        public DateTime newstart { get; set; }
        public string PointDollars { get; set; }
        public string payment_name { get; set; }
        public int payment_code { get; set; }
        public string user_username { get; set; }
        public PromotionsAccumulateRateQuery()
        {
            group_name = string.Empty;
            expired = 0;
            newstart = DateTime.MinValue;
            PointDollars = "1/1";
            payment_name = string.Empty;
            payment_code = 0;
            user_username = string.Empty;
        }
    }
}
