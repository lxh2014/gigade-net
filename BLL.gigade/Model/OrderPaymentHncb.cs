/**
 * chaojie_zz添加於2014/10/31 10:30 am
 * 訂單管理>訂單內容>華南匯款資料
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class OrderPaymentHncb:PageBase
    {
        public string hncb_id { set; get; }
        public uint order_id { set; get; }
        public string bank { set; get; }//銀行帳號
        public uint entday { set; get; }//會計日期
        public int txtday { set; get; }//交易日期
        public string sn { set; get; }//交易序號
        public string specific_currency { set; get; }//幣別
        public uint paid { set; get; }//交易金額
        public uint type { set; get; }//借貸別
        public uint hncb_sn { set; get; }
        public uint outputbank { set; get; }//轉出銀行
        public string pay_type { set; get; }//作業別
        public uint e_date { set; get; }//票繳日期
        public string note { set; get; }//備註
        public uint vat_number { set; get; }//統一編號
        public uint error { set; get; }
        public string msg { set; get; }
        public int createdate { set; get; }
        public int updatedate { set; get; }

        public OrderPaymentHncb()
        {
            bank = string.Empty;
            entday = 0;
            txtday = 0;
            sn = string.Empty;
            specific_currency = string.Empty;
            paid = 0;
            type = 0;
            e_date = 0;
            note = string.Empty;
            vat_number = 0;
        }
    }
}
