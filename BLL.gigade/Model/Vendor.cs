/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：Vendor 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/1/14 13:21:14 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
namespace BLL.gigade.Model
{
    [Serializable]
    public class Vendor : PageBase
    {
        public uint vendor_id { get; set; }
        public string vendor_code { get; set; }
        public uint vendor_status { get; set; }
        public string vendor_email { get; set; }
        public string vendor_password { get; set; }
        public string vendor_name_full { get; set; }
        public string vendor_name_simple { get; set; }
        public string vendor_invoice { get; set; }
        public string company_phone { get; set; }
        public string company_fax { get; set; }
        public string company_person { get; set; }
        public uint company_zip { get; set; }
        public string company_address { get; set; }
        public uint invoice_zip { get; set; }//
        public string invoice_address { get; set; }
        public uint contact_type_1 { get; set; }
        public string contact_name_1 { get; set; }
        public string contact_phone_1_1 { get; set; }
        public string contact_phone_2_1 { get; set; }
        public string contact_mobile_1 { get; set; }
        public string contact_email_1 { get; set; }
        public uint contact_type_2 { get; set; }
        public string contact_name_2 { get; set; }
        public string contact_phone_1_2 { get; set; }
        public string contact_phone_2_2 { get; set; }
        public string contact_mobile_2 { get; set; }
        public string contact_email_2 { get; set; }
        public uint contact_type_3 { get; set; }
        public string contact_name_3 { get; set; }
        public string contact_phone_1_3 { get; set; }
        public string contact_phone_2_3 { get; set; }
        public string contact_mobile_3 { get; set; }
        public string contact_email_3 { get; set; }
        public uint contact_type_4 { get; set; }
        public string contact_name_4 { get; set; }
        public string contact_phone_1_4 { get; set; }
        public string contact_phone_2_4 { get; set; }
        public string contact_mobile_4 { get; set; }
        public string contact_email_4 { get; set; }
        public uint contact_type_5 { get; set; }
        public string contact_name_5 { get; set; }
        public string contact_phone_1_5 { get; set; }
        public string contact_phone_2_5 { get; set; }
        public string contact_mobile_5 { get; set; }
        public string contact_email_5 { get; set; }
        public uint cost_percent { get; set; }
        public uint creditcard_1_percent { get; set; }
        public string creditcard_3_percent { get; set; }
        public uint sales_limit { get; set; }
        public uint bonus_percent { get; set; }
        public uint agreement_createdate { get; set; }
        public uint agreement_start { get; set; }
        public uint agreement_end { get; set; }
        public uint checkout_type { get; set; }
        public string checkout_other { get; set; }
        public string bank_code { get; set; }
        public string bank_name { get; set; }
        public string bank_number { get; set; }
        public string bank_account { get; set; }
        public uint freight_low_limit { get; set; }
        public uint freight_low_money { get; set; }
        public uint freight_normal_limit { get; set; }
        public uint freight_normal_money { get; set; }
        public uint freight_return_low_money { get; set; }
        public uint freight_return_normal_money { get; set; }
        public string vendor_note { get; set; }
        public string vendor_confirm_code { get; set; }
        public uint vendor_login_attempts { get; set; }
        public uint assist { get; set; }
        public uint dispatch { get; set; }
        public uint product_mode { get; set; }
        public uint product_manage { get; set; }
        public uint gigade_bunus_percent { get; set; }
        public uint gigade_bunus_threshold { get; set; }
        public string erp_id { get; set; }
        public int procurement_days { get; set; }
        public int self_send_days { get; set; }
        public int stuff_ware_days { get; set; }
        public int dispatch_days { get; set; }
        public int export_flag { get; set; }
        public string vendor_type { get; set; }//供應商類型
        public int kuser { get; set; }//供應商建立人
        public DateTime kdate { get; set; }//供應商建立日期
        public Vendor()
        {
            vendor_id = 0;
            vendor_code = string.Empty;
            vendor_status = 0;
            vendor_email = string.Empty;
            vendor_password = string.Empty;
            vendor_name_full = string.Empty;
            vendor_name_simple = string.Empty;
            vendor_invoice = string.Empty;
            company_phone = string.Empty;
            company_fax = string.Empty;
            company_person = string.Empty;
            company_zip = 0;
            company_address = string.Empty;
            invoice_zip = 0;
            invoice_address = string.Empty;
            contact_type_1 = 0;
            contact_name_1 = string.Empty;
            contact_phone_1_1 = string.Empty;
            contact_phone_2_1 = string.Empty;
            contact_mobile_1 = string.Empty;
            contact_email_1 = string.Empty;
            contact_type_2 = 0;
            contact_name_2 = string.Empty;
            contact_phone_1_2 = string.Empty;
            contact_phone_2_2 = string.Empty;
            contact_mobile_2 = string.Empty;
            contact_email_2 = string.Empty;
            contact_type_3 = 0;
            contact_name_3 = string.Empty;
            contact_phone_1_3 = string.Empty;
            contact_phone_2_3 = string.Empty;
            contact_mobile_3 = string.Empty;
            contact_email_3 = string.Empty;
            contact_type_4 = 0;
            contact_name_4 = string.Empty;
            contact_phone_1_4 = string.Empty;
            contact_phone_2_4 = string.Empty;
            contact_mobile_4 = string.Empty;
            contact_email_4 = string.Empty;
            contact_type_5 = 0;
            contact_name_5 = string.Empty;
            contact_phone_1_5 = string.Empty;
            contact_phone_2_5 = string.Empty;
            contact_mobile_5 = string.Empty;
            contact_email_5 = string.Empty;
            cost_percent = 0;
            creditcard_1_percent = 0;
            creditcard_3_percent = string.Empty;
            sales_limit = 0;
            bonus_percent = 0;
            agreement_createdate = 0;
            agreement_start = 0;
            agreement_end = 0;
            checkout_type = 0;
            checkout_other = string.Empty;
            bank_code = string.Empty;
            bank_name = string.Empty;
            bank_number = string.Empty;
            bank_account = string.Empty;
            freight_low_limit = 0;
            freight_low_money = 0;
            freight_normal_limit = 0;
            freight_normal_money = 0;
            freight_return_low_money = 0;
            freight_return_normal_money = 0;
            vendor_note = string.Empty;
            vendor_confirm_code = string.Empty;
            vendor_login_attempts = 0;
            assist = 0;
            dispatch = 0;
            product_mode = 0;
            product_manage = 0;
            gigade_bunus_percent = 0;
            gigade_bunus_threshold = 0;
            erp_id = string.Empty;
            procurement_days = 0;
            self_send_days = 0;
            stuff_ware_days = 0;
            dispatch_days = 0;
            export_flag = 0;
            vendor_type = string.Empty;
            kuser = 0;
            kdate = DateTime.MinValue;
        }
    }
}
