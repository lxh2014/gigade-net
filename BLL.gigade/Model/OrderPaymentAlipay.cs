/**
 * 訂單管理=>訂單內容=>支付寶
 * chaojie_zz添加於2014/10/29  02:12 PM
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public  class OrderPaymentAlipay : PageBase
    {


        public uint alipay_id { set; get; }//流水號
        public string merchantnumber { set; get; }//商店代號
        public string ordernumber { set; get; }
        public string serialnumber { set; get; }//交易序號
        public string writeoffnumber { set; get; }//銷帳編號
        public string timepaid { set; get; }//付款日期
        public string paymenttype { set; get; }
        public string amount { set; get; }//繳款金額
        public string tel { set; get; }//電話
        public string hash { set; get; }//加密
        public string hash2 { set; get; }
        public uint error { set; get; }
        public OrderPaymentAlipay()
        {
            alipay_id = 0;
            merchantnumber = string.Empty;
            timepaid = string.Empty;
            writeoffnumber = string.Empty;
            serialnumber = string.Empty;
            tel = string.Empty;
            hash = string.Empty;
        }
    }
}
