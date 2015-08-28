/**
 * chaojie_zz添加於 2014/10/31 01:02 PM
 * 訂單管理>訂單內容>Hitrust-網際威信
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public  class OrderPaymentHitrust:PageBase
    {
        public uint id { set; get; }//流水號
        public string order_id { set; get; }
        public int retstatus { set; get; }
        public string retcode { set; get; }
        public string retcodename { set; get; }//刷卡狀態
        public string rettype { set; get; }//交易類別
        public int depositamount { set; get; }//請款金額
        public int approveamount { set; get; }//核准金額
        public int orderstatus { set; get; }//請款狀態 retcode==00&&orderstatus=3
        public string authCode { set; get; }
        public string eci { set; get; }
          public string pan { set; get; }
          public string bankname { set; get; }
        public int authRRN { set; get; }//銀行調單編號
        public int paybatchnumber { set; get; }
        public string capDate { set; get; }//請款日期
        public int credamount { set; get; }
        public int credbatchnumber { set; get; }
        public int credRRN { set; get; }
        public int credCode { set; get; }
        public string creddate { set; get; }
        public string E09 { set; get; }
        public string redem_discount_point { set; get; }//本次折抵點數
        public string redem_discount_amount { set; get; }//本次折抵金額
        public string redem_purchase_amount { set; get; }
        public string memo { set; get; }
        public string createtime { set; get; }//建立時間
        //updatetime字段，timestamp類型。未定義。
        public OrderPaymentHitrust()
        { 
            id=0;
            retcodename=string.Empty; 
            authRRN=0;
            rettype = string.Empty;
            capDate=string.Empty;
            retcode=string.Empty;
            orderstatus=0;
            depositamount=0;
            approveamount=0;
            redem_discount_amount = string.Empty;
            redem_discount_point = string.Empty; 
            createtime = string.Empty;
            pan = string.Empty;
            bankname=string.Empty;
        }
    }
}
