using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class NewPromoPresent : PageBase
    {
        public int row_id { get; set; }
        public string event_id { get; set; }
        public int gift_type { get; set; }
        //public DateTime valid_start { get; set; }//捨棄 edit by shuangshuang0420j 20150310 10：58
        //public DateTime valid_end { get; set; }
        public int group_id { get; set; }
        public string ticket_name { get; set; }
        public string ticket_serial { get; set; }
        public int gift_id { get; set; }
        public double deduct_welfare { get; set; }
        public int gift_amount { get; set; }
        public int gift_amount_over { get; set; }
        public int freight_price { get; set; }
        public int status { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public double welfare_mulriple { get; set; }//購物金倍數 add by shuangshuang0420j 20150319 11:16
        public int kuser { get; set; }
        public int muser { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public int bonus_expire_day { get; set; }
        public int use_span_day { get; set; }
        public NewPromoPresent()
        {
            row_id = 0;
            event_id = string.Empty;
            // valid_start = DateTime.MinValue;
            gift_type = 0;
            // valid_end = DateTime.MinValue;
            group_id = 0;
            ticket_name = string.Empty;
            ticket_serial = string.Empty;
            gift_id = 0;
            deduct_welfare = 0;
            gift_amount = 0;
            gift_amount_over = 0;
            freight_price = 0;
            status = 0;
            start = DateTime.MinValue;
            end = DateTime.MinValue;
            welfare_mulriple = 1;
            kuser = 0;
            muser = 0;
            created = DateTime.MinValue;
            modified = DateTime.MinValue;
            bonus_expire_day = 0;
            use_span_day = 0;
        }
    }
}
