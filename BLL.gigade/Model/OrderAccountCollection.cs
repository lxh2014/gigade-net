using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class OrderAccountCollection : PageBase
    {
        public int row_id { get; set; }
        public uint order_id { get; set; }
        public DateTime account_collection_time { get; set; }
        public int account_collection_money { get; set; }
        public int poundage { get; set; }
        public DateTime return_collection_time { get; set; }
        public int return_collection_money { get; set; }
        public int return_poundage { get; set; }
        public string remark { get; set; }
        public DateTime invoice_date_manual { get; set; }
        public int invoice_sale_manual { get; set; }
        public int invoice_tax_manual { get; set; }

        public OrderAccountCollection()
        {
            row_id = 0;
            order_id = 0;
            account_collection_time = DateTime.MinValue;
            account_collection_money = 0;
            poundage = 0;
            return_collection_time = DateTime.MinValue;
            return_collection_money = 0;
            return_poundage = 0;
            remark = string.Empty;
            invoice_date_manual = DateTime.MinValue;
            invoice_sale_manual = 0;
            invoice_tax_manual = 0;
        }
    }
}
