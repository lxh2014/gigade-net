/**
 *訂單管理=>訂單內容=>銀聯
 * chaojie1124j_zz添加於2014/10/29 03:39 PM
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class OrderPaymentUnionPay:PageBase
    {
        public uint union_id { set; get; }//流水號
        public string transtype { set; get; }//交易類型
        public string respcode { set; get; }//回應碼
        public uint order_id { set; get; }
        public string respmsg { set; get; }//回應訊息
        public string merabbr { set; get; }//特店名稱
        public string merid { set; get; }//特店代碼
        public string orderamount { set; get; }//繳款金額
        public string ordercurrency { set; get; }//交易幣種
        public string resptime { set; get; }//交易完成時間
        public string cupReserved { set; get; } //備註
        public uint union_createdate{set;get;}
        public string union_ipfrom { set; get; }
        public OrderPaymentUnionPay()
        {
            union_id=0;
            respcode=string.Empty;
            transtype=string.Empty;
            respmsg=string.Empty;
            merabbr=string.Empty;
            merid=string.Empty;
            orderamount=string.Empty;
            ordercurrency=string.Empty;
            resptime=string.Empty;
            cupReserved=string.Empty;

        }
       

    }
}
