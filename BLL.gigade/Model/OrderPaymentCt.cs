/**
 * chaojie_zz添加于2014/10/31  09:37 am
 * 订单管理=>订单内容=>中國信託
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class OrderPaymentCt:PageBase
    {
        public int id { set; get; }//編號
        public int status { set; get; }//狀態
        public int errcode { set; get; }//錯誤代碼
        public string authcode { set; get; }//授權 代碼
        public int authamt { set; get; }
        public int merid { set; get; }
        public int lidm { set; get; }
        public int originalamt { set; get; }//訂單金額
        public int offsetamt { set; get; }//紅利折抵金額
        public int utilizedpoint { set; get; }//紅利折抵點數
        public int last4digitpan{set;get;}
        public string errdesc { set; get; }//交易錯誤訊息
        public string xid { set; get; }//交易序號
        public int awardedpoint { set; get; }//獲取紅利點數
        public int pointbalance { set; get; }//紅利餘額
        public int numberofpay { set;get;}
        public int prodcode { set; get; }
        public uint check {set;get;}//数据库中是checked,在这里是关键字。查询的时候as 一下
        public DateTime created { set; get; }
        public DateTime modified { set; get; }
        public string ipfrom { set; get; }
        public OrderPaymentCt()
        { 
            id = 0;
            status = 0;
            errcode = 0;
            authcode = string.Empty;
            originalamt = 0;
            offsetamt = 0;
            utilizedpoint = 0;
            errdesc = string.Empty;
            xid = string.Empty;
            awardedpoint = 0;
            pointbalance = 0;
        }
    }
}
