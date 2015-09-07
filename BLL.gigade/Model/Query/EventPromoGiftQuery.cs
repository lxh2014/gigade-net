using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class EventPromoGiftQuery : EventPromoGift
    {
        public string product_name { get; set; }//商品名稱

        public EventPromoGiftQuery()
        {
            product_name = string.Empty;
        }
    }
}
