using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EventPromoGift:PageBase
    {
        //贈品主鍵ID
        public int gift_id { get; set; }
        //贈品名稱
        public string gift_name { get; set; }
        //贈送的商品
        public int product_id { get; set; }
        //每次贈送商品數量
        public int product_num { get; set; }
        //購物金
        public int bonus { get; set; }
        //購物金倍數
        public int bonus_multiple { get; set; }
        //抵用卷
        public int welfare { get; set; }
        //抵用卷倍數
        public int welfare_multiple { get; set; }
        //贈品類型1:送商品2：購物金3：抵用卷
        public int gift_type { get; set; }
        //購買的數量
        public int quantity { get; set; }
        //購買的金額
        public int amount { get; set; }
        //活動編號
        public string event_id { get; set; }
        //創建人
        public int create_user { get; set; }
        //創建時間
        public DateTime create_time { get; set; }
        //修改人
        public int modify_user { get; set; }
        //修改時間
        public DateTime modify_time { get; set; }

        public EventPromoGift()
        {
            gift_id = 0;
            gift_name = string.Empty;
            product_id = 0;
            product_num = 0;
            bonus = 0;
            bonus_multiple = 0;
            welfare = 0;
            welfare_multiple = 0;
            gift_type = 0;
            quantity = 0;
            amount = 0;
            event_id = string.Empty;
            create_user = 0;
            create_time = DateTime.MinValue;
            modify_user = 0;
            modify_time = DateTime.MinValue;

        }
    }
}
