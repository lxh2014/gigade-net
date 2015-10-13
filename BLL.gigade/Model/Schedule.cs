using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Schedule
    {
        public int schedule_id { get; set; }//自增ID
        public string schedule_name { get; set; }//排程名稱
        public int type { get; set; }//1:單次執行,2:多次執行3:特殊規律執行
        public string execute_type { get; set; }//執行類別:每月執行:2M,每週執行:2W,每日執行:2D
        public int day_type { get; set; }//每日頻率判斷,1：為每日單次執行,2：為每日重複執行
        public int month_type { get; set; }//每月頻率判斷,1：為每月單次執行,2：為每月重複執行
        public int date_value { get; set; } //2D:該值為null,2W:該值為0~4:2M:該值為1~31//當為特殊排程時 該值為執行時間(天數)
        public int repeat_count { get; set; }//重複的次數 //當為特殊排程時 該值為重複星期數(天數)
        public int repeat_hours { get; set; } //重複的小時數
        public int time_type { get; set; } //時間單位(用於repeat_hours的後綴) 1:H(時) 2:M(分):3:S(秒)
        public string week_day { get; set; } // 星期   //當為特殊排程時,該值為觸發時間
        public DateTime start_time { get; set; }//執行時間精確到second //當為特殊排程時,該值為觸發時間時分秒部份
        public DateTime end_time { get; set; }//重複小時結束時間//當為特殊排程時,該值為觸發時間時分秒結束部份
        public DateTime duration_start { get; set; }//該流程的開始時間
        public DateTime duration_end { get; set; }//該流程的結束時間
        public DateTime now { get; set; }//當前時間
        public string execute_days { get; set; }///執行時間
        public string trigger_time { get; set; }///觸發時間
        public int item_type { get; set; }
        public int key1 { get; set; }
        public int key2 { get; set; }
        public int key3 { get; set; }
        public int value1 { get; set; }
        public int value2 { get; set; }
        public int value3 { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string desc { get; set; }
        /// <summary>
        /// 創建人
        /// </summary>
        public int create_user { get; set; }
        public string create_user_name { get; set; }

        /// <summary>
        /// 創建時間
        /// </summary>
        public DateTime create_date { get; set; }


        public Schedule()
        {
            schedule_id = 0;
            item_type = 0;
            key1 = 0;
            key2 = 0;
            key3 = 0;
            value1=0;
            value2=0;
            value3 = 0;
            schedule_name = string.Empty;
            type = 0;
            execute_type = string.Empty;
            day_type = 0;
            month_type = 0;
            date_value = 0;
            repeat_count = 0;
            repeat_hours = 0;
            time_type = 0;
            week_day = string.Empty;
            start_time = DateTime.MinValue;
            end_time = DateTime.MinValue;
            duration_start = DateTime.MinValue;
            duration_end = DateTime.MaxValue;
            now = DateTime.Now;
            desc = string.Empty;
            create_user = 0;
            create_date = DateTime.MinValue;
            execute_days = string.Empty;
            trigger_time = string.Empty;
        }
    }
}
