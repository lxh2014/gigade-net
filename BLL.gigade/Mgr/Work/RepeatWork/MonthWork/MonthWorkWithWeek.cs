using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.gigade.Model;

namespace Working
{
    class MonthWorkWithWeek : MonthWork
    {
        /// <summary>
        /// 該月的第幾周
        /// </summary>
        protected int Week { get; set; }
        protected int Week_day { get; set; }
        public MonthWorkWithWeek(Schedule fst)
        {
            Initial(fst);
        }

        protected override void Initial(Schedule freSetTime)
        {
            base.Initial(freSetTime);
            Week = freSetTime.date_value;
            Week_day = freSetTime.week_day == "" ? 0 : Convert.ToInt32(freSetTime.week_day);
        }

        public override DateTime CurrentExecuteDate()
        {
            List<DateTime> list = new List<DateTime>();//用來存儲符合要求的執行時間

            DateTime dt = base.StartDate; ///獲得排程開始時間

            DateTime dtTime = dt;///定義一個按天增加的時間,

            while (DateTime.Compare(dtTime,DateTime.Now) < 0 && DateTime.Compare(dtTime,DateTime.Now.AddMonths(RepeatCount)) < 0)///循環獲得符合要求的時間,直至該時間大於當前時間,就停止循環
            {
                dtTime = CirculateMonth(dt); ///獲取符合排程中,第幾周,第幾天的日期
                list.Add(dtTime);              
                dt = dt.AddMonths(RepeatCount); ///遞增一次循環的時間
            }
            
            if(NoEndDate==false) ///如果存在排程結束時間
            {
                if (DateTime.Compare(dt, EndDate) > 0) ///該時間大於排程結束時間
                {
                    if (list.Count > 0)
                    {
                        for (int i = list.Count - 1; i >= 0; i--)
                        {
                            if (list[i] < EndDate)
                            {
                                dtTime = list[i];
                                break;
                            }
                        }
                    }
                    else 
                    {
                        dtTime = DateTime.MinValue;  ///將該時間賦值為最小時間
                    }
                }
                else if (DateTime.Compare(dt, EndDate) < 0) ///如果該時間小於排程結束時間
                {
                    return dtTime; ///返回該時間
                }
            }
            return dtTime;///返回該時間
        }


        /// <summary>
        /// 循環獲取的時間
        /// </summary>
        /// <param name="dt">循環時間</param>
        /// <returns>第一個大於當前時間的循環時間</returns>
        public DateTime CirculateMonth(DateTime dt)
        {
            dt =Convert.ToDateTime(dt.ToString("yyyy-MM")); ///將遞增后的時間初始為該月的1號
            if(Week_day < (int)dt.DayOfWeek && Week == 1) //特殊情況,例如:當設置每月第一周星期一,但某月的1號重星期三開始,則第一周沒有星期一 則特殊考慮
            {
                int num_day = (int)dt.DayOfWeek - 1; //獲得需要向前遞減的天數,例如: 7.31 7.30 直至符合要求
                dt = dt.AddDays(-num_day);
                return dt;
            }

            for (int i = 0; i < GetMaxDay(dt)-1;i++ ) ///從開始的1號到該月月底 進行遍歷循環
            {
                if (IsCirculateDay(dt)==true) ///如果得到的時間符合要求
                {
                    break;                  ///跳出循環
                }
                dt = dt.AddDays(1); ///否則繼續循環
            }
            return dt;
        }

        /// <summary>
        ///對時間進行判斷
        /// </summary>
        /// <param name="dt">要判斷的時間</param>
        /// <returns>複合條件,不符合條件</returns>
        public bool IsCirculateDay(DateTime dt)
        {
            int countWeek = GetWeekth(dt); ///獲得當天是該月的第幾個星期

            switch(Week_day)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    if (countWeek == Week && Week_day == (int)dt.DayOfWeek)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case 8:
                    break;
                case 9:
                    if (countWeek == Week && ((int)dt.DayOfWeek != 1 && (int)dt.DayOfWeek != 2 && (int)dt.DayOfWeek != 3 && (int)dt.DayOfWeek != 4 && (int)dt.DayOfWeek != 5))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case 10:
                    if (countWeek == Week && ((int)dt.DayOfWeek == 0 || (int)dt.DayOfWeek == 6))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }
            return false;
        }

        /// <summary>
        /// 獲得當天所屬星期是該月的第幾個星期
        /// </summary>
        /// <param name="dt">需要判斷的時間</param>
        /// <returns>第幾周</returns>
        public int GetWeekth(DateTime dt)
        {
            int countWeek = 1;
            //int day_temp = (int)dt.DayOfWeek == 0 ? 7 : day_temp = (int)dt.DayOfWeek;
            int day_temp = (int)dt.AddDays(-(dt.Day - 1)).DayOfWeek == 0 ? 7 : (int)dt.AddDays(-(dt.Day - 1)).DayOfWeek;

            for (int i = 0; i < dt.Day; i++)
            {
                if (day_temp == 7 && dt.Day - 1 != i)
                {
                    day_temp = 0;
                    countWeek++;
                }
                day_temp++;
            }
            return countWeek;
        }
    }
}
