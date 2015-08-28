using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.gigade.Model;

namespace Working
{
    class MonthWorkWithDay : MonthWork
    {
        /// <summary>
        /// 該月的第幾天
        /// </summary>
        protected int Day { get; set; }

        public MonthWorkWithDay(Schedule fst)
        {
            Initial(fst);
        }

        protected override void Initial(Schedule freSetTime)
        {
            base.Initial(freSetTime);
            
            Day = freSetTime.date_value;
        }

        public override DateTime CurrentExecuteDate()
        {
            DateTime dt = StartDate;;///定義一個用來進行操作的時間

            if (Day > GetMaxDay(dt)) ///如果開始的天數 > 當月的最大天數,將最大天數賦予開始天數 (例如:2月第31天開始,但是2月最多只有28天,所以就是2月第28天開始)
            {
                Day = GetMaxDay(dt);
            }

            if(base.RepeatCount==1)///如果重複次數是按每月執行,則需要考慮排程開始的這個月的情況
            {
                DateTime dtDetachDt = Convert.ToDateTime(dt.ToString("yyyy-MM")); //獲得該月的一號
                dtDetachDt = dtDetachDt.AddDays(Day); ///下一次開始的日期
                if (DateTime.Compare(dtDetachDt, DateTime.Now) > 0)
                {
                    Now = dtDetachDt;
                    return Now;
                }
            }

            Now = CirculateMonth(dt);
            
            return Now;
        }



        /// <summary>
        /// 循環獲取的時間
        /// </summary>
        /// <param name="dt">循環其實的時間</param>
        /// <returns>第一個大於當前時間的循環時間</returns>
        public DateTime CirculateMonth(DateTime dt)
        {
            dt = dt.AddMonths(RepeatCount); ///循環遞增月份

            DateTime dtDetachDate = Convert.ToDateTime(dt.ToString("yyyy-MM")); ///獲得該月一號

            dtDetachDate = dtDetachDate.AddDays(Day-1); ///下一次開始的日期

            if (DateTime.Compare(dtDetachDate, DateTime.Now) < 0) ///如果開始的日期小於當前時間日期
            {
                dtDetachDate = CirculateMonth(dt); ///繼續尋找新的開始時間
            }
            else if (DateTime.Compare(dtDetachDate, DateTime.Now) > 0 && base.NoEndDate)///如果循環時間大於當前時間,並且沒有結束時間
            {
                return dtDetachDate; 
            }
            else if (DateTime.Compare(dtDetachDate, DateTime.Now) > 0 && base.NoEndDate == false && DateTime.Compare(dtDetachDate, base.EndDate) < 0)///如果循環時間大於當前時間,且有排程結束時間,且循環時間 < 排程結束時間
            {
                return dtDetachDate;
            }
            else ///否則最近一次執行時間超出了排程結束的時間,則向前尋找,直到找到一個小於排程結束時間的時間
            {
                while (dtDetachDate > base.EndDate)
                {
                    dtDetachDate = dtDetachDate.AddMonths(-(base.RepeatCount));
                }
                return dtDetachDate;
            }

            return dtDetachDate;
        }
    }
}
