/*
* 文件名稱 :Channel.cs
* 文件功能描述 :外站資訊表
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/19
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Channel:PageBase
    {
        public int channel_id { get; set; }
        public int channel_status { get; set; }
        public string channel_status_name { get; set; }
        public string channel_name_full { get; set; }
        public string channel_name_simple { get; set; }
        public string channel_email { get; set; }
        public string company_phone { get; set; }
        public string company_fax { get; set; }
        public int company_zip { get; set; }
        public string company_address { get; set; }
        public string channel_invoice { get; set; }
        public string invoice_title { get; set; }
        public int invoice_zip { get; set; }
        public string invoice_address { get; set; }
        public int user_id { get; set; }
        public string user_email { get; set; }
        public DateTime contract_createdate { get; set; }
        public DateTime contract_start { get; set; }
        public DateTime contract_end { get; set; }
        public decimal annaul_fee { get; set; }
        public decimal renew_fee { get; set; }
        public string channel_note { get; set; }
        public string channel_manager { get; set; }
        public int deal_method { get; set; }
        public float deal_percent { get; set; }
        public int deal_fee { get; set; }
        public float creditcard_1_percent { get; set; }
        public float creditcard_3_percent { get; set; }
        public float shopping_car_percent { get; set; }
        public float commission_percent { get; set; }
        public int cost_by_percent { get; set; }
        public float cost_low_percent { get; set; }
        public float cost_normal_percent { get; set; }
        public int invoice_period { get; set; }
        public int invoice_checkout_day { get; set; }
        public int invoice_apply_start { get; set; }
        public int invoice_apply_end { get; set; }
        public string checkout_note { get; set; }
        public int receipt_to { get; set; }
        public int channel_type { get; set; }
        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }
        public string companyCity { get; set; }
        public string invoiceCity { get; set; }
        public string model_in { get; set; }
        public int notify_sms { get; set; }
        public string erp_id { get; set; }//add by xiangwang0413w 2014/06/26 增加 ERP客戶代號

        public Channel()
        { 
            channel_id = 0;
            channel_status = 0;
            channel_status_name = string.Empty;
            channel_name_full = string.Empty;
            channel_name_simple = string.Empty;
            channel_email = string.Empty;
            company_phone = string.Empty;
            company_fax = string.Empty;
            company_zip = 0;
            company_address = string.Empty;
            channel_invoice = string.Empty;
            invoice_title = string.Empty;
            invoice_zip = 0;
            invoice_address = string.Empty;
            user_id = 0;
            user_email = string.Empty;
            contract_createdate = DateTime.MinValue;
            contract_start = DateTime.MinValue;
            contract_end = DateTime.MinValue;
            annaul_fee = 0;
            renew_fee = 0;
            channel_note = string.Empty;
            channel_manager = string.Empty;
            deal_method = 0;
            deal_percent = 0;
            deal_fee = 0;
            creditcard_1_percent = 0;
            creditcard_3_percent = 0;
            shopping_car_percent = 0;
            commission_percent = 0;
            cost_by_percent = 0;
            cost_low_percent = 0;
            cost_normal_percent = 0;
            invoice_period = 0;
            invoice_checkout_day = 0;
            invoice_apply_start = 0;
            invoice_apply_end = 0;
            checkout_note = string.Empty;
            receipt_to = 0;
            channel_type = 0;
            createdate = DateTime.MinValue;
            updatedate = DateTime.MinValue;
            companyCity = string.Empty;
            invoiceCity = string.Empty;
            model_in = string.Empty;
            notify_sms = 0;
            erp_id = string.Empty;//add by xiangwang0413w 2014/06/26 增加 ERP客戶代號
        }
    }
}
