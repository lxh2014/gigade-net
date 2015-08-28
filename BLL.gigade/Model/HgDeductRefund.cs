using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class HgDeductRefund : PageBase
    {
        public uint id { get; set; }
        public string head { get; set; }
        public string card_num { get; set; }
        public string chk_card { get; set; }
        public string enc_idno { get; set; }
        public string chk_sum { get; set; }
        public DateTime transaction_date { get; set; }
        public string merchant { get; set; }
        public string terminal { get; set; }
        public uint refund_point { get; set; }
        public string category { get; set; }
        public string note { get; set; }
        public string wallet { get; set; }
        public uint order_id { get; set; }
        public DateTime import_time { get; set; }
        public string error_type { get; set; }
        public uint status { get; set; }
        public DateTime modified { get; set; }
        public uint billing_checked { get; set; }
        public HgDeductRefund()
        {
            id = 0;
            head = string.Empty;
            card_num = string.Empty;
            chk_card = string.Empty;
            enc_idno = string.Empty;
            chk_sum = string.Empty;
            transaction_date = DateTime.MinValue;
            merchant = string.Empty;
            terminal = string.Empty;
            refund_point = 0;
            category = string.Empty;
            note = string.Empty;
            wallet = string.Empty;
            order_id = 0;
            import_time = DateTime.MinValue;
            error_type = string.Empty;
            status = 0;
            modified = DateTime.MinValue;
            billing_checked = 0;

        }

    }
}

