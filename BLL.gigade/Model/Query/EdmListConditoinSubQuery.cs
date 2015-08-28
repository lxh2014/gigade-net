using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class EdmListConditoinSubQuery:EdmListConditionMain
    {
        //多選框
        public bool chkGender { get; set; }//性別
        public bool ChkBuy { get; set; }//購買次數
        public bool ChkAge { get; set; }//年齡
        public bool ChkCancel { get; set; }//取消次數
        public bool ChkRegisterTime { get; set; }//註冊時間
        public bool ChkReturn { get; set; }//退貨次數
        public bool ChkLastOrder { get; set; }//最後訂單
        public bool ChkNotice { get; set; }//貨到通知
        public bool ChkLastLogin { get; set; }//最後登入
        public bool ChkTotalConsumption { get; set; }//消費金額
        public bool ChkBlackList { get; set; }//黑名單
      
        public int genderCondition { get; set; }
        public int buyCondition { get; set; }
        public int buyTimes { get; set; }
        public DateTime buyTimeMin { get; set; }
        public DateTime buyTimeMax { get; set; }
        public int ageMin { get; set; }
        public int ageMax { get; set; }
        public int cancelCondition { get; set; }
        public int cancelTimes { get; set; }
        public DateTime cancelTimeMin { get; set; }
        public DateTime cancelTimeMax { get; set; }
        public DateTime registerTimeMin { get; set; }
        public DateTime registerTimeMax { get; set; }
        public int returnCondition { get; set; }
        public int returnTimes { get; set; }
        public DateTime returnTimeMin { get; set; }
        public DateTime returnTimeMax { get; set; }
        public DateTime lastOrderMin { get; set; }
        public DateTime lastOrderMax { get; set; }
        public int noticeCondition { get; set; }
        public int noticeTimes { get; set; }
        public DateTime lastLoginMin { get; set; }
        public DateTime lastLoginMax { get; set; }
        public int totalConsumptionMin { get; set; }
        public int totalConsumptionMax { get; set; }
        public bool excludeBlackList { get; set; }

        public EdmListConditoinSubQuery()
        {
            chkGender = false;
            ChkBuy = false;
            ChkAge = false;
            ChkCancel = false;
            ChkRegisterTime = false;
            ChkReturn = false;
            ChkLastOrder = false;
            ChkNotice = false;
            ChkLastLogin = false;
            ChkTotalConsumption = false;
            ChkBlackList = false;

            genderCondition = 0;
            buyCondition = 0;
            buyTimes = 0;
            buyTimeMin = DateTime.MinValue;
            buyTimeMax = DateTime.MinValue;
            ageMin = 0;
            ageMax = 0;
            cancelCondition = 0;
            cancelTimes = 0;
            cancelTimeMin = DateTime.MinValue;
            cancelTimeMax = DateTime.MinValue;
            registerTimeMin = DateTime.MinValue;
            registerTimeMax = DateTime.MinValue;
            returnCondition = 0;
            returnTimes = 0;
            returnTimeMin = DateTime.MinValue;
            returnTimeMax = DateTime.MinValue;
            lastOrderMin = DateTime.MinValue;
            lastOrderMax = DateTime.MinValue;
            noticeCondition = 0;
            noticeTimes = 0;
            lastLoginMin = DateTime.MinValue;
            lastLoginMax = DateTime.MinValue;
            totalConsumptionMin = 0;
            totalConsumptionMax = 0;
           // elcm_name = string.Empty;
        }
    }
}
