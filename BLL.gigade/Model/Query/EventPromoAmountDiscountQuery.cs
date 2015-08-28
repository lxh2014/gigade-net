using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public class EventPromoAmountDiscountQuery : EventPromoAmountDiscount
    {
       /// <summary>
       /// 站台名稱
       /// </summary>
        public string site_name { get; set; }
       /// <summary>
       /// 修改人姓名
       /// </summary>
        public string user_name { get; set; }
        public EventPromoAmountDiscountQuery()
        {
            site_name = string.Empty;
            user_name = string.Empty;
        }
    }
}
