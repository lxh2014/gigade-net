using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Working
{
    class SpecialWeekWork : SpecialWork
    {
        bool flag = false;///如果flag 的值為true意味 該星期是執行星期,flase則為非執行星期
        bool isTrue = false;///是否 使用setFlag 方法
        bool isHour = true;///是否啟用小時
        int index = 0;//用來記錄執行星期執行了多少個小時
        int count = 1;///重複判斷依據
        int excuteStart = 0;///記錄出發時間的開始時間
        string executeDay = "";///執行時間
        string trigger_time = "";
        int addTime = 24;///添加的時間
        DateTime start_time { get; set; }//執行時間精確到second //當為特殊排程時,該值為觸發時間時分秒部份
        DateTime end_time { get; set; }//重複小時結束時間//當為特殊排程時,該值為觸發時間時分秒結束部份

        protected override void Initial(Schedule fst)
        {
            base.Initial(fst);
            if(repeat_count==0)
            {
                repeat_count = 1;
            }
            executeDay = fst.execute_days;///暫用描述代替      
            trigger_time = fst.trigger_time;
            start_time = DateTime.MinValue;
            end_time = DateTime.MaxValue;
        }

        public SpecialWeekWork(Schedule fst)
        {
            Initial(fst);
        }

        public override DateTime CurrentExecuteDate()
        {
            DateTime triggerTime = DateTime.MinValue;
            try
            {
                string[] sourceDate = trigger_time.Split(',');///獲得按小時計算的星期區間
                if (start_time == DateTime.MinValue && end_time == DateTime.MaxValue)///判斷是否啟用小時
                {
                    isHour = false;
                }
                if (repeat_count == 1)///如果重複次數為1則設flag永為true
                {
                    flag = true;
                    isTrue = true;
                    addTime = 1;
                    duration_start = DateTime.Now;///如果為每週執行,則直接設置排程開始時間為當前時間
                }
                triggerTime = GetTime(duration_start, sourceDate);///獲得觸發時間

                int NumTrigger = IsTriggerByWeek(sourceDate, triggerTime);/*找出所對應執行時間的索引*/

                int ExecuteTime = GetExecuteTime(NumTrigger - 1);///得到執行時間
                int ExecuteTimeHour = ExecuteTime * 24;///將執行時間換成小時

                //int diffHour = ((int)triggerTime.DayOfWeek-1)*24 - triggerTime.Hour;///將觸發事件換成小時
                /*每一個區間值記錄了各自第一天的執行時間,第二天的...就要相應的減1.所以要減去時間差*/
                triggerTime = triggerTime.AddHours(ExecuteTimeHour - GetDiffDay(triggerTime));///計算出 最近出貨時間
                if (triggerTime > duration_end)
                {
                    triggerTime = DateTime.MinValue;
                }
            }
            catch (Exception ex)
            {
                
                throw new Exception("CurrentExecuteDate"+ex.Message,ex);
            }

            return triggerTime;///返回最後時間
        }

        private int GetDiffDay(DateTime triggerDt)
        {
            int temp = ((int)triggerDt.DayOfWeek - 1) * 24 + triggerDt.Hour;
            temp = temp - excuteStart;
            if (temp > 0)
            {
                return temp;
            }
            return 0;
        }

        /// <summary>
        /// 通過重複次數,確定flag的真假,true:該星期為可執行星期,false:該星期為非執行星期
        /// </summary>
        private bool SetFlag(DateTime dateTime)
        {
            bool tempFlag = false;
            int repeat = repeat_count;///重複次數
            if ((int)dateTime.DayOfWeek == 1)///如果當前時間=星期一
            {
                count++;                    ///count 自動加1 表示遞增一周
                if (count == repeat)  ///如果count滿足循環周
                {
                    count = 0;  ///count從新開始循環
                    tempFlag = true;///臨時變量tempFlag的值取反
                }
            }

            return tempFlag;///返回tempFlag
        }

        private DateTime GetTime(DateTime p_dateTime, string [] sourceDate)
        {
            try
            {
                return Circulate(p_dateTime, sourceDate);///開始循環找出合理時間
            }
            catch (Exception ex)
            {
                throw new Exception("SpecialWeekWork-->GetTime" + ex.Message, ex);
            }
        }

        private DateTime Circulate(DateTime dt, string[] sourceDate)
        {   
            int triggerFlag = -1;

            if (isTrue == false)///如果每次都執行,則不使用SetFlag方法進行循環設置,減少代碼邏輯
            {
                flag = SetFlag(dt);///設置flag的狀態
            }
            try
            {
                if (flag == true)///如果是執行星期則開始按小時計算,防止按天加漏掉小時區間
                {
                    triggerFlag = IsTriggerByWeek(sourceDate, dt);///是否包含該區間如果包含,返回所對應的數組索引:該索引用來進行加減計算
                    if (triggerFlag != -1)///如果包含時間,且是執行星期
                    {
                        DateTime result =GetRealTime(dt);
                        if (result != DateTime.MinValue)
                        {
                            return result;
                        }
                    }
                }
                              
                dt = dt.AddHours(addTime);//添加時間
                return Circulate(dt, sourceDate);
            }
            catch (Exception ex)
            {
                new Exception("SpecialWeekWork-->Circulate" + ex.Message, ex);
            }
            return dt;
        }///通過循環得出大於當前時間的觸發事件

        private int GetExecuteTime(int num)
        {
            string[] temp = executeDay.Split(',');
            if (num <= temp.Length)
            {
                return Convert.ToInt32(temp[num]);
            }
            return -1;
        }

        private int IsTriggerByWeek(string[] executeInfo, DateTime dt)
        {
            int tempValue = 0;
            try
            {
                int weekDay = (int)dt.DayOfWeek;///得到當前時間是星期幾
                int dtNowHour = (weekDay - 1) * 24 + dt.Hour;///將當前時間轉換為小時;
                for (int i = 0; i < executeInfo.Length-1;i++ )
                {
                    var temp = i / 2;
                    if (i % 2 == 0)
                    {
                        tempValue++;
                        if (dtNowHour >= Convert.ToInt32(executeInfo[i]) && dtNowHour < Convert.ToInt32(executeInfo[i + 1]))
                        {
                            excuteStart = Convert.ToInt32(executeInfo[i]);
                            return tempValue;
                        }
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                throw new Exception("IsTriggerByWeek-->" + ex.Message,ex);
            }        
        }

        private DateTime GetRealTime(DateTime dt)
        {
            bool flag = false;
            int index = ((int)dt.DayOfWeek - 1) * 24 + dt.Hour;
            while (!flag)
            {
                dt = dt.AddHours(1);
                if (dt > DateTime.Now)
                {
                    return dt;
                }
                if (index==168)
                {
                    flag = true;
                }
                index++;
            }
            return DateTime.MinValue;
        }
    }
}
