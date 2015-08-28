using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class TicketDetail:PageBase
    {
        public int ticket_detail_id { set; get; }
        public int ticket_master_id { set; get; }
        public int vendor_id { set; get; }
        public int product_mode { set; get; }
        public string product_name { set; get; }
        public string product_spec_name { set; get; }
        public int single_cost { set; get; }
        public int event_cost { set; get; }
        public int single_price { set; get; }
        public int single_money { set; get; }
        public int deduct_bonus { set; get; }
        public int deduct_welfare { set; get; }
        public int deduct_account { set; get; }
        public string deduct_account_note { set; get; }
        public int detail_status { set; get; }
        public string detail_note { set; get; }
        public int ticket_type { set; get; }
        public string site_id { set; get; }
        
        public TicketDetail()
        {
            ticket_detail_id = 0;
            ticket_master_id = 0;
            vendor_id = 0;
            product_mode = 0;
            product_name = string.Empty;
            product_spec_name = string.Empty;
            single_cost = 0;
            event_cost = 0;
            single_price = 0;
            single_money = 0;
            deduct_bonus = 0;
            deduct_welfare = 0;
            deduct_account = 0;
            deduct_account_note = string.Empty;
            detail_status = 0;
            detail_note = string.Empty;
            ticket_type = 0;
            site_id = string.Empty;

        }

    }
}
