using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.gigade.Model;


namespace Working
{
    class Work:IWork
    {
        private Work work = null;
        protected DateTime Now { get; set; }
        /// <summary>
        /// 開始日期
        /// </summary>
        protected DateTime StartDate { get; set; }
        protected Work() { }
        public Work(Schedule freightSetTime)
        {
            switch (freightSetTime.type)
            {
                case 1:
                    this.work = new OnlyOneWork(freightSetTime);
                    break;
                case 2:
                    this.work = new RepeatWork(freightSetTime);
                    break; 
                case 3:
                    this.work = new SpecialWork(freightSetTime);
                    break;
                default:
                    throw new Exception("unaccepted type");
            }
        }

        public virtual DateTime CurrentExecuteDate()
        {
            return this.work.CurrentExecuteDate();
        }

        protected virtual void Initial(Schedule freSetTime)
        {
            Now = freSetTime.now.Date;
            StartDate = freSetTime.duration_start.Date;
        }
    }
}
