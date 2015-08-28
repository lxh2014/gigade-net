using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.gigade.Model;

namespace Working
{
     class RepeatWork : Work 
    {
        private RepeatWork work = null;
        /// <summary>
        /// 結束日期
        /// </summary>
        protected DateTime EndDate { get; set; }
        /// <summary>
        /// 是否“沒有結束日期”
        /// </summary>
        protected bool NoEndDate { get; set; }
         /// <summary>
         /// 重複頻率
         /// </summary>
        private int _repeatCount;
        protected int RepeatCount
        {
            get { return _repeatCount; }
            set
            {
                _repeatCount = value == 0 ? 1 : value;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="freSetTime"></param>
        protected override void Initial(Schedule freSetTime)
        {
            base.Initial(freSetTime);
            EndDate = freSetTime.duration_end.Date;
            NoEndDate = EndDate == DateTime.MinValue;
            RepeatCount = freSetTime.repeat_count;
        }

        protected RepeatWork() { }
        public RepeatWork(Schedule fst)
        {
            switch (fst.execute_type)
            {
                case "2D":
                    work = new DayWork(fst);
                    break;
                case "2W":
                    work = new WeekWork(fst);
                    break;
                case "2M":
                    work = new MonthWork(fst);
                    break;
                default:
                    throw new Exception("unaccepted date_value");
            }
        }

        public override DateTime CurrentExecuteDate()
        {
            return work.CurrentExecuteDate();
        }

    }
}
