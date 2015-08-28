/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：OrderImport 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/9/4 13:41:31 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    [Serializable]
    public class OrdersImport : PageBase
    {
        /// <summary>
        /// 賣場訂單編號
        /// </summary>
        public string dmtshxuid { get; set; }

        /// <summary>
        /// 賣場子訂單編號 //訂單編號
        /// </summary>
        public string subsn { get; set; }

        /// <summary>
        /// 賣場商品編號
        /// </summary>
        public string chlitpdno { get; set; }

        /// <summary>
        /// 購買當時商品名稱 //商品名稱
        /// </summary>
        public string dsr { get; set; }

        /// <summary>
        /// 購買備註
        /// </summary>
        public string xrem { get; set; }

        /// <summary>
        /// 購買當時之商品金額小計
        /// </summary>
        public string sumup { get; set; }

        /// <summary>
        /// 訂購數量//商品數量
        /// </summary>
        public string qty { get; set; }

        /// <summary>
        /// 物流方式
        /// </summary>
        public string shipco { get; set; }

        /// <summary>
        /// 訂購日/付款時間
        /// </summary>
        public string orddat { get; set; }

        /// <summary>
        /// 訂購人姓名
        /// </summary>
        public string ordpesnm { get; set; }

        /// <summary>
        /// 訂購人手機
        /// </summary>
        public string ordpesnacttel { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string agpesnm { get; set; }

        /// <summary>
        /// 收件人地址
        /// </summary>
        public string agpesadr { get; set; }

        /// <summary>
        /// 收件人郵遞區號
        /// </summary>
        public string agpesadrzip { get; set; }

        /// <summary>
        /// 收件人電話
        /// </summary>
        public string agpestel1 { get; set; }

        /// <summary>
        /// 收件人手機
        /// </summary>
        public string agpesacttel { get; set; }

        /// <summary>
        /// 最晚出貨日///出貨日
        /// </summary>
        public string prndldat { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string Payment { get; set; }

        /// <summary>
        /// 超商提貨單號
        /// </summary>
        public string shipno { get; set; }

        /// <summary>
        /// 配送備註
        /// </summary>
        public string shipxrem { get; set; }
        public string Msg { get; set; }


        //add by wangwei0216w 2014/8/14
        #region MyRegion + PayEasy訂單特有屬性
        /// <summary>
        /// 項次
        /// </summary>
        public string ItemCount { get; set; }

        /// <summary>
        /// 訂單狀態
        /// </summary>
        public string OrderState { get; set; }

        /// <summary>
        /// 物流公司
        /// </summary>
        public string ProductCompany { get; set; }

        /// <summary>
        /// 配送單號
        /// </summary>
        public string DispatchingID { get; set; }

        /// <summary>
        /// 組檔日期
        /// </summary>
        public string ConstitutorTime { get; set; }

        /// <summary>
        /// 活動訂單
        /// </summary>
        public string ActivityOrder { get; set; }

        /// <summary>
        /// 商品流水號
        /// </summary>
        public string ProductStreamOrder { get; set; }

        /// <summary>
        ///場上商品原始碼
        /// </summary>
        public string ProductPrimitiveID { get; set; }
 
        /// <summary>
        ///訂單貨款
        /// </summary
        public string OrderRich { get; set; }

        /// <summary>
        ///電話(夜)
        /// </summary
        public string NightPhone { get; set; }

        /// <summary>
        /// 貨到日期
        /// </summary>
        public string ArriveTime { get; set; }

        /// <summary>
        /// 訂單類型
        /// </summary>
        public string OrderType { get; set; }

        #endregion



        public string CanSel
        {
            get
            {
                return string.IsNullOrEmpty(Msg) || Msg == BLL.gigade.Mgr.Resource.CoreMessage.GetResource("SAVE_TO_DB_FAILURE") ? "0" : "1";
            }
        }

        public bool IsSel { get; set; }




        public OrdersImport()
        {
            dmtshxuid = string.Empty;
            subsn = string.Empty;
            chlitpdno = string.Empty;
            dsr = string.Empty;
            xrem = string.Empty;
            sumup = string.Empty;
            qty = string.Empty;
            shipco = string.Empty;
            orddat = string.Empty;
            ordpesnm = string.Empty;
            ordpesnacttel = string.Empty;
            agpesnm = string.Empty;
            agpesadr = string.Empty;
            agpesacttel = string.Empty;
            agpesadrzip = string.Empty;
            agpestel1 = string.Empty;
            prndldat = string.Empty;
            Payment = string.Empty;
            shipno = string.Empty;
            shipxrem = string.Empty;
            Msg = string.Empty;

            IsSel = false;
        }
    }
}
