using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EventPromoUserCondition:PageBase
    {
        //會員條件ID
        public int condition_id { get; set; }
        //活動名稱
        public string condition_name { get; set; }
        //會員級別
        public string level_id { get; set; }
        //首次購買時間
        public int first_buy_time { get; set; }
        //會員註冊時間起
        public int reg_start { get; set; }
        //會員註冊時間截止日期
        public int reg_end { get; set; }
        //最少消費次數
        public int buy_times_min { get; set; }
        //最多消費次數
        public int buy_times_max { get; set; }
        //至少消費金額
        public int buy_amount_min { get; set; }
        //最多消費金額
        public int buy_amount_max { get; set; }
        //創建人
        public int create_user { get; set; }
        //創建時間
        public DateTime create_time { get; set; }
        //修改人
        public int modify_user { get; set; }
        //修改時間
        public DateTime modify_time { get; set; }
        //會員群組
        public int group_id { get; set; }

        public EventPromoUserCondition()
        {
            condition_id = 0;
            condition_name = string.Empty;
            level_id = string.Empty;
            first_buy_time = 0;
            reg_start = 0;
            reg_end = 0;
            buy_times_min = 0;
            buy_times_max = 0;
            buy_amount_min = 0;
            buy_amount_max = 0;
            create_user = 0;
            create_time = DateTime.MinValue;
            modify_user = 0;
            modify_time = DateTime.MinValue;
            group_id = 0;
        }
    }
}
