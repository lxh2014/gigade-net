using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class EventPromoGiftQuery : EventPromoGift
    {
        public int gift_ware { get; set; }//購物金、抵用券共用額度
        public int gift_num { get; set; }//購物金、抵用券共用倍數
        public string product_name { get; set; }//商品名稱

        public EventPromoGiftQuery()
        {
            gift_ware = 0;
            gift_num = 0;
            product_name = string.Empty;
        }
    }
}
