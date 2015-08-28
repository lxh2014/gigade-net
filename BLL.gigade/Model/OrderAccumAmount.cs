using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class OrderAccumAmount:PageBase
    {
        public int event_id { set; get; }//編號
        public DateTime event_start_time { set; get; }//開始時間
        public DateTime event_end_time { set; get; }//結束時間
        public string event_desc { set; get; }//活動描述
        public string event_name { set; get; }//活動名稱
        public DateTime event_desc_start { set; get; }//描述開始時間
        public DateTime event_desc_end { set; get; }//描述結束時間
        public int event_status { set; get; }//狀態
        public int event_create_user { set; get; }//創建人
        public DateTime event_create_time { set; get; }//創建時間
        public int event_update_user { set; get; }//修改人
        public DateTime event_update_time { set; get; }//修改時間
        public int accum_amount { set; get; }//累積金額

        public OrderAccumAmount()
        {
            event_id = 0;
            event_start_time = DateTime.MinValue;
            event_end_time = DateTime.MinValue;
            event_desc = string.Empty;
            event_name = string.Empty;
            event_desc_start = DateTime.MinValue;
            event_desc_end = DateTime.MinValue;
            event_status = 1;
            event_create_user = 0;
            event_create_time = DateTime.MinValue;
            event_update_user = 0;
            event_update_time = DateTime.MinValue;
            accum_amount = 0;

        }

    }
}
