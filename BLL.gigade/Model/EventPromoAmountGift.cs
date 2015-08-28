using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EventPromoAmountGift : PageBase
    {
        //活動編號
        public string event_id { get; set; }
        //流水號
        public int row_id { get; set; }
        //活動名稱
        public string event_name { get; set; }
        //活動描述
        public string event_desc { get; set; }
        //活動開始時間
        public DateTime event_start { get; set; }
        //活動結束時間
        public DateTime event_end { get; set; }
        //活動類型
        public string event_type { get; set; }
        //站台
        public string site_id { get; set; }
        //創建人
        public int create_user { get; set; }
        //創建時間
        public DateTime create_time { get; set; }
        //修改人
        public int modify_user { get; set; }
        //修改時間
        public DateTime modify_time { get; set; }
        //會員條件編號
        public int user_condition_id { get; set; }
        //設定商品方式 
        public int condition_type { get; set; }
        //裝置
        public int device { get; set; }
        //活動狀態
        public int event_status { get; set; }

        public EventPromoAmountGift()
        {
            event_id = string.Empty;
            row_id = 0;
            event_name = string.Empty;
            event_desc = string.Empty;
            event_start = DateTime.MinValue;
            event_end = DateTime.MinValue;
            event_type = string.Empty;
            site_id = string.Empty;
            create_user = 0;
            create_time = DateTime.MinValue;
            modify_user = 0;
            modify_time = DateTime.MinValue;
            user_condition_id = 0;
            condition_type = 0;
            device = 0;
            event_status = 0;
        }
    }
}
