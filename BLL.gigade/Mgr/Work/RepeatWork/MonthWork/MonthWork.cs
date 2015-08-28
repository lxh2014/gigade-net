using System;
using BLL.gigade.Model;

namespace Working
{
    class MonthWork : RepeatWork
    {
        MonthWork monthWork=null;

        protected MonthWork() { }
        public MonthWork(Schedule fst)
        {
            switch(fst.month_type)
            {
                case 1:
                    monthWork=new MonthWorkWithDay(fst);
                    break;
                case 2:
                    monthWork = new MonthWorkWithWeek(fst);
                    break;
                default:
                    throw new Exception("unaccpted month_type");
                    
            }
        }

        public override DateTime CurrentExecuteDate()
        {
           return monthWork.CurrentExecuteDate();
        }

        /// <summary>
        /// 獲得當月的最大天數
        /// </summary>
        /// <param name="dt">一個具體的時間</param>
        /// <returns>該月份的天數</returns>
        public int GetMaxDay(DateTime dt)
        {
            int m = dt.Month;
            switch (m)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return 31;
                case 4:
                case 6:
                case 9:
                case 11:
                    return 30;
                case 2:
                    if (DateTime.IsLeapYear(dt.Year))
                        return 29;
                    else
                        return 28;
            }
            return 31;
        }
    }
}
