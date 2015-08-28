using System;
using BLL.gigade.Model;

namespace Working
{
    class DayWork : RepeatWork
    {
        public DayWork(Schedule fst)
        {
            Initial(fst);
        }

        public override DateTime CurrentExecuteDate()
        {
            DateTime execDate = StartDate;//執行日期
            if (Now > StartDate)//當前時間>開始時間
            {
                int interval = (Now - StartDate).Days % RepeatCount;//距離下次執行日期的間隔
                execDate = Now.AddDays(interval);
                if (!NoEndDate && execDate > EndDate)//有結束日期&&執行日期>結束日期
                {
                    interval = ((EndDate - StartDate).Days % RepeatCount) - RepeatCount;//距離上次執行日的日期間隔
                    execDate = Now.AddDays(interval);
                }
            }
            return execDate;
        }
    }
}
