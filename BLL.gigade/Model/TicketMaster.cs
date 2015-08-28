using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class TicketMaster : PageBase
    {
        public int ticket_master_id { set; get; }
        public int user_id { set; get; }
        public int order_status { set; get; }
        public int order_payment { set; get; }
        public int order_amount { set; get; }
        public int money_cancel { set; get; }
        public int money_return { set; get; }
        public int money_collect_date { set; get; }
        public int order_date_pay { set; get; }
        public string order_name { set; get; }//
        public int order_gender { set; get; }
        public string order_mobile { set; get; }
        public string order_phone { set; get; }
        public int order_zip { set; get; }
        public string order_address { set; get; }
        public int delivery_same { set; get; }
        public string delivery_name { set; get; }
        public int delivery_gender { set; get; }
        public string delivery_mobile { set; get; }
        public string delivery_phone { set; get; }
        public int delivery_zip { set; get; }
        public string delivery_address { set; get; }
        public int delivery_store { set; get; }
        public int company_write { set; get; }
        public string company_invoice { set; get; }
        public string company_title { set; get; }
        public int invoice_status { set; get; }
        public string note_order { set; get; }
        public string note_admin { set; get; }
        public int billing_checked { set; get; }
        public int order_createdate { set; get; }
        public int order_updatedate { set; get; }
        public string order_ipfrom { set; get; }
        public int master_status { set; get; }
        public TicketMaster()
        {
            ticket_master_id = 0;
            user_id = 0;
            order_status = 0;
            order_payment = 0;
            order_amount = 0;
            money_cancel = 0;
            money_return = 0;
            money_collect_date = 0;
            order_date_pay = 0;
            order_name = string.Empty;
            order_gender = 0;
            order_mobile = string.Empty;
            order_phone = string.Empty;
            order_zip = 0;
            order_address = string.Empty;
            delivery_same = 0;
            delivery_name = string.Empty;
            delivery_gender = 0;
            delivery_mobile = string.Empty;
            delivery_phone = string.Empty;
            delivery_zip = 0;
            delivery_address = string.Empty;
            delivery_store = 0;
            company_write = 0;
            company_invoice = string.Empty;
            company_title = string.Empty;
            invoice_status = 0;
            note_order = string.Empty;
            note_admin = string.Empty;
            billing_checked = 0;
            order_createdate = 0;
            order_updatedate = 0;
            order_ipfrom = string.Empty;
            master_status = 0;
        }
    }
}
