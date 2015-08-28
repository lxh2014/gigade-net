using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EventPromoShoppingcart : PageBase
    {
        //編號
        public int row_id { get; set; }
        //活動類型
        public string event_type { get; set; }
        //購物車編號
        public int cart_id { get; set; }
        //活動編號
        public string event_id { get; set; }
        //活動開始時間
        public DateTime event_start { get; set; }
        //活動結束時間
        public DateTime event_end { get; set; }
        //活動狀態
        public int event_status { get; set; }
        //創建人
        public int create_user { get; set; }
        //創建時間
        public DateTime create_time { get; set; }
        //修改人
        public int modify_user { get; set; }
        //修改時間
        public DateTime modify_time { get; set; }

        public EventPromoShoppingcart()
        {
            row_id = 0;
            event_type = string.Empty;
            cart_id = 0;
            event_id = string.Empty;
            event_start = DateTime.MinValue;
            event_end = DateTime.MinValue;
            event_status = 0;
            create_user = 0;
            create_time = DateTime.MinValue;
            modify_user = 0;
            modify_time = DateTime.MinValue;
        }
    }
}
