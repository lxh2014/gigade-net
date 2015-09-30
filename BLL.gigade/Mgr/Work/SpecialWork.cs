using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Working
{
    class SpecialWork : Work
    {
        private SpecialWork work = null;
        protected int repeat_count = 1;///特殊星期的重複次數        
        protected DateTime duration_start;
        protected DateTime duration_end;

        protected override void Initial(Schedule fst)
        {
            base.Initial(fst);
            repeat_count = fst.repeat_count;///初始化
            duration_start = fst.duration_start;///排程開始時間
            duration_end = fst.duration_end;///排程結束時間
        }

        protected SpecialWork() { }
        public SpecialWork(Schedule fst) 
        {

            switch (fst.execute_type)
            {
                case "2W":
                    work = new SpecialWeekWork(fst);
                    break;
                default:
                    work = new SpecialWeekWork(fst);
                    break;
            }
        }

        public override DateTime CurrentExecuteDate()
        {
            return work.CurrentExecuteDate();
        }
    }
}
