using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EventPromoAmountFare : PageBase
    {
        /// <summary>
        ///  活動編號
        /// </summary>
        public string event_id { get; set; }
        /// <summary>
        /// 流水號
        /// </summary>
        public int row_id { get; set; }
        /// <summary>
        /// 活動名稱
        /// </summary>
        public string event_name { get; set; }
        /// <summary>
        /// 活動描述
        /// </summary>
        public string event_desc { get; set; }
        /// <summary>
        /// 活動開始時間
        /// </summary>
        public DateTime event_start { get; set; }
        /// <summary>
        /// 活動結束時間
        /// </summary>
        public DateTime event_end { get; set; }
        /// <summary>
        /// 活動類型
        /// </summary>
        public string event_type { get; set; }
        /// <summary>
        /// 站台
        /// </summary>
        public string site_id { get; set; }
        /// <summary>
        /// 創建人
        /// </summary>
        public int create_user { get; set; }
        /// <summary>
        /// 創建時間
        /// </summary>
        public DateTime create_time { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public int modify_user { get; set; }
        /// <summary>
        /// 修改時間
        /// </summary>
        public DateTime modify_time { get; set; }
        /// <summary>
        /// 會員條件編號
        /// </summary>
        public int user_condition_id { get; set; }
        /// <summary>
        /// 設定商品方式
        /// </summary>
        public int condition_type { get; set; }
        /// <summary>
        /// 裝置
        /// </summary>
        public int device { get; set; }
        /// <summary>
        /// 活動狀態
        /// </summary>
        public int event_status { get; set; }
        /// <summary>
        /// 購買的總數量
        /// </summary>
        public int quantity { get; set; }
        /// <summary>
        /// 低溫總數量
        /// </summary>
        public int low_quantity { get; set; }
        /// <summary>
        /// 購買的總金額
        /// </summary>
        public int amount { get; set; }
        /// <summary>
        /// 低溫總金額
        /// </summary>
        public int low_amount { get; set; }
        /// <summary>
        /// 常溫總數量
        /// </summary>
        public int normal_quantity { get; set; }
        /// <summary>
        /// 常溫總金額
        /// </summary>
        public int normal_amount { get; set; }
        /// <summary>
        /// 是否減免低溫運費
        /// </summary>
        public int free_freight_low { get; set; }
        /// <summary>
        /// 是否減免常溫運費
        /// </summary>
        public int free_freight_normal { get; set; }

        /// <summary>
        ///附加字段
        /// 站台名稱
        /// </summary>
        public string site_name { get; set; }
        /// <summary>
        /// 附加字段
        /// 修改人姓名
        /// </summary>
        public string user_name { get; set; }



        public EventPromoAmountFare()
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
            quantity = 0;
            low_quantity = 0;
            amount = 0;
            low_amount = 0;
            normal_quantity = 0;
            normal_amount = 0;
            free_freight_low = 0;
            free_freight_normal = 0;

            site_name = string.Empty;
            user_name = string.Empty;
        }
    }
}
