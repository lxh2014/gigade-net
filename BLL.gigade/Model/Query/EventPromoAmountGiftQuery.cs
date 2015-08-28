using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class EventPromoAmountGiftQuery : EventPromoAmountGift
    {
        public string site_name { get; set; }//站台名稱
        public string user_name { get; set; }//修改人姓名
        public EventPromoAmountGiftQuery()
        {
            site_name = string.Empty;
            user_name = string.Empty;
        }
    }
}
