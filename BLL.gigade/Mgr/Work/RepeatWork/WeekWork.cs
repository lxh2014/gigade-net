using System;
using System.Collections.Generic;
using System.Linq;
using BLL.gigade.Model;

namespace Working
{
    class WeekWork : RepeatWork
    {
        public int[] DaysOfWeek { get; set; }

        public WeekWork(Schedule fst)
        {
            Initial(fst);
        }

        protected override void Initial(Schedule fst)
        {
            base.Initial(fst);
            DaysOfWeek = fst.week_day.Split(',')
                .Select(i => int.Parse(i))
                .OrderBy(i => i).ToArray();
        }

        public override DateTime CurrentExecuteDate()
        {
            DateTime execDate = StartDate;
            int interval = 0;//距離執行日的時間間隔
            int firstDay = DaysOfWeek.First(), lastDay = DaysOfWeek.Last();
            int startDay = StartDate.DayOfWeek == 0 ? 7 : (int)StartDate.DayOfWeek;//開始日期是星期幾
            #region if
            if (Now > StartDate)//當前時間>開始時間
            {
                int today = Now.DayOfWeek == 0 ? 7 : (int)Now.DayOfWeek;//今天是星期幾
                int toMonday = -(((Now - StartDate).Days % (7 * RepeatCount)) + (startDay - 1));//距离最近执行周还有几天
                if (toMonday > -7)//本周為執行周
                {
                    if (today > lastDay)
                    {
                        toMonday += 7 * RepeatCount;
                        interval = toMonday + firstDay - 1;
                    }
                    else
                    {
                        int recent = DaysOfWeek.First(d => d >= today);
                        interval = toMonday + recent - 1;
                    }
                }
                else
                {
                    toMonday += (7 * RepeatCount);
                    interval = toMonday + firstDay - 1;
                }
                execDate = Now.AddDays(interval);
                if (!NoEndDate && execDate > EndDate)//有結束日期&&執行日期>結束日期
                {
                    toMonday = -((EndDate - StartDate).Days % (7 * RepeatCount)) - (startDay - 1);// 距离上次执行周有几天
                    interval = toMonday + lastDay - 1;
                    if (toMonday == 0)
                    {
                        interval -= 7 * RepeatCount;
                    }
                    execDate = EndDate.AddDays(interval);
                }
            }
            #endregion
            #region else
            else//當前時間<=開始時間
            {
                if (startDay > lastDay)
                {
                    int toMonday = (7 * RepeatCount) + (startDay - 1);
                    interval = toMonday + (firstDay - 1);
                }
                else
                {
                    int recent = DaysOfWeek.First(d => d >= startDay);
                    interval = recent - startDay;
                }
                execDate = StartDate.AddDays(interval);
            }
            #endregion

            return execDate;
        }
    }
}
