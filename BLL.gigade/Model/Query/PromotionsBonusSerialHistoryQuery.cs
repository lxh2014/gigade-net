using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class PromotionsBonusSerialHistoryQuery : PromotionsBonusSerialHistory
    {
        public string user_email { get; set; }
        public int myid { get; set; }
        public PromotionsBonusSerialHistoryQuery()
        {
            user_email = "";
            myid = 0;
        }
    }
}
