
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class HgDeduct : PageBase
    {
        public int id { get; set; }
        public string merchant_pos { get; set; }
        public string terminal_pos { get; set; }
        public string enc_idno { get; set; }
        public string chk_sum { get; set; }
        public string token { get; set; }
        public string order_id { get; set; }
        public string prn { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public uint billing_checked { get; set; }



        public HgDeduct()
        {
            id = 0;
            merchant_pos = string.Empty;
            terminal_pos = string.Empty;
            enc_idno = string.Empty;
            chk_sum = string.Empty;
            token = string.Empty;
            order_id = string.Empty;
            prn = string.Empty;
            date = string.Empty;
            time = string.Empty;
            code = string.Empty;
            message = string.Empty;
            created = DateTime.MinValue;
            modified = DateTime.MinValue;
            billing_checked = 0;

        }

    }
}
