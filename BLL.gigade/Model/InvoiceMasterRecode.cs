using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class InvoiceMasterRecode : PageBase
    {
        public int invoice_id { get;set; }
        public int order_id { get;set; }
        public uint invoice_status { get;set; }
        public uint invoice_attribute { get;set; }
        public uint invoice_modify_count { get; set; }
        public string invoice_number { get;set; }
        public int invoice_date{ get;set; }
        public string free_tax { get; set; }
        public string sales_amount { get; set; }
        public string tax_amount { get; set; }
        public string total_amount { get; set; }
        public int deduct_bonus { get; set; }
        public int deduct_welfare { get; set; }
        public int order_freight_normal { get; set; }
        public string order_freight_normal_notax { get; set; }
        public int order_freight_low { get; set; }
        public string order_freight_low_notax { get; set; }
        public uint buyer_type { get; set; }
        public string buyer_name { get; set; }
        public string company_invoice { get; set; }
        public string company_title { get; set; }
        public int order_zip { get; set; }
        public string order_address { get; set; }
        public string invoice_note { get; set; }
        public int print_post_createdate { get; set; }
        public string print_post_mailer { get; set; }
        public int print_flag { get; set; }
        public int status_createdate { get; set; }
        public string user_update { get; set; }
        public int user_updatedate { get; set; }
        public uint invoice_win { get; set; }
        public uint invoice_mode { get; set; }
        public uint invoice_close { get; set; }
        public int invoice_close_date { get; set; }
        public int tax_type { get; set; }

        public InvoiceMasterRecode()
        {
            invoice_id = 0;
            order_id = 0;
            invoice_status = 0;
            invoice_attribute = 0;
            invoice_modify_count = 0;
            invoice_number = string.Empty;
            invoice_date = 0;
            free_tax = string.Empty;
            sales_amount = string.Empty;
            tax_amount = string.Empty;
            total_amount = string.Empty;
            deduct_bonus = 0;
            deduct_welfare = 0;
            order_freight_normal = 0;
            order_freight_normal_notax = string.Empty;
            order_freight_low = 0;
            order_freight_low_notax = string.Empty;
            buyer_type = 0;
            buyer_name = string.Empty;
            company_invoice = string.Empty;
            company_title = string.Empty;
            order_zip = 0;
            order_address = string.Empty;
            invoice_note = string.Empty;
            print_post_createdate = 0;
            print_post_mailer = string.Empty;
            print_flag = 0;
            status_createdate = 0;
            user_update = string.Empty;
            user_updatedate = 0;
            invoice_win = 0;
            invoice_mode = 0;
            invoice_close = 0;
            invoice_close_date = 0;
            tax_type = 0;
        }
    }
}
