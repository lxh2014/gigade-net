using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class InvoiceAllowanceRecord:PageBase
    {
        public int allowance_id { get; set; }
        public int invoice_id { get; set; }
        public int order_id { get; set; }
        public string invoice_number { get; set; }
        public int invoice_date { get; set; }
        public int allowance_date { get; set; }
        public uint buyer_type { get; set; }
        public string buyer_name { get; set; }
        public string company_title { get; set; }
        public string company_invoice { get; set; }
        public int status_createdate { get; set; }
        public uint invoice_status { get; set; }
        public int allownace_total { get; set; }
        public int allowance_amount { get; set; }
        public int allowance_tax { get; set; }
        public uint allowance_return { get; set; }
        public int allowance_return_date { get; set; }
        public string master_writer { get; set; }
        public string invoice_note { get; set; }
        public InvoiceAllowanceRecord()
        {
            allowance_id = 0;
            invoice_id = 0;
            order_id = 0;
            invoice_number = string.Empty;
            invoice_date = 0;
            allowance_date = 0;
            buyer_type = 0;
            buyer_name = string.Empty;
            company_title = string.Empty;
            company_invoice = string.Empty;
            status_createdate = 0;
            invoice_status = 0;
            allownace_total = 0;
            allowance_amount = 0;
            allowance_tax = 0;
            allowance_return = 0;
            allowance_return_date = 0;
            master_writer = string.Empty;
            invoice_note = string.Empty;
        }

    }
}
