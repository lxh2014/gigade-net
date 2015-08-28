using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EventPromoProduct : PageBase
    {
        //流水號
        public int row_id { get; set; }
        //活動類型
        public string event_type { get; set; }
        //站臺編號
        public string site_id { get; set; }
        //商品編號
        public int product_id { get; set; }
        //商品參與數量
        public int product_num_limit { get; set; }
        //活動編號
        public string event_id { get; set; }
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
        //活動開始時間
        public DateTime event_start { get; set; }
        //活動結束時間
        public DateTime event_end { get; set; }

        //商品名稱
        public string product_name { get; set; }
        public EventPromoProduct()
        {
            row_id = 0;
            event_type = string.Empty;
            site_id = string.Empty;
            product_id = 0;
            product_num_limit = 0;
            event_id = string.Empty;
            event_status = 0;
            create_user = 0;
            create_time = DateTime.MinValue;
            modify_user = 0;
            modify_time = DateTime.MinValue;
            event_start = DateTime.MinValue;
            event_end = DateTime.MinValue;
            product_name = string.Empty;
        }
    }
}
