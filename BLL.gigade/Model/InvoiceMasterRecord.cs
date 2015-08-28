/***
 * 訂單內容=>發票記錄
 * chaojie_zz 添加於2014/10/30  10:11 AM
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
     public  class InvoiceMasterRecord:PageBase
    {
        public uint invoice_id { set; get; }//發票編號
         public uint order_id { set; get; }
         public uint invoice_status { set; get; }//狀態
         public uint invoice_attribute { set; get; }//屬性
         public uint invoice_modify_count { set; get; }
         public string invoice_number { set; get; }//發票號碼
         public uint invoice_date { set; get; }//開立時間
         public string free_tax { set; get; }//免稅金額
         public string sales_amount { set; get; }
         public string tax_amount { set; get; }//營業稅
         public string total_amount { set; get; }//應稅金額
         public uint deduct_bonus { set; get; }
         public uint deduct_welfare { set; get; }
         public uint order_freight_normal { set; get; }
         public string order_freight_normal_notax { set; get; }
         public uint order_freight_low { set; get; }
         public string order_freight_low_notax { set; get; }
         public uint buyer_type { set; get; }
         public string buyer_name { set; get; }
         public string company_invoice { set; get; }
         public string company_title { set; get; }
         public uint order_zip { set; get; }
         public string order_address { set; get; }
         public string invoice_note { set; get; }
         public uint print_post_createdate { set; get; }
         public string print_post_mailer { set; get; }
         public uint print_flag { set; get; }
         public uint status_createdate { set; get; }
         public string user_update { set; get; }
         public uint user_updatedate { set; get; }
         public uint invoice_win { set; get; }
         public uint invoice_mode { set; get; }
         public uint invoice_close { set; get; }
         public uint invoice_close_date { set; get; }
         public int tax_type { set; get; }//營業稅


         public InvoiceMasterRecord() 
         {
             invoice_id = 0;
             invoice_number = string.Empty;
             tax_type = 0;
             free_tax = string.Empty;
             tax_amount = string.Empty;
             total_amount = string.Empty;
             invoice_status = 0;
             invoice_attribute = 0;
             invoice_date = 0;
         }

    }
}
