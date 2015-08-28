using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    [DBTableInfo("order_money_return")]
    public class OrderMoneyReturn : PageBase
    {

        public uint money_id { get; set; }
        public uint order_id { get; set; }
        public uint money_type { get; set; }
        public uint money_total { get; set; }
        public uint money_status { get; set; }
        public string money_note { get; set; }
        public string money_source { get; set; }
        public string bank_name { get; set; }
        public string bank_branch { get; set; }
        public string bank_account { get; set; }
        public string bank_note { get; set; }
        public string account_name { get; set; }
        public uint money_createdate { get; set; }
        public uint money_updatedate { get; set; }
        public string money_ipfrom { get; set; }
        public string cs_note { get; set; }

        public OrderMoneyReturn()
        {
            money_id = 0;
            order_id = 0;
            money_type = 0;
            money_total = 0;
            money_status = 0;
            money_note = string.Empty;
            money_source = string.Empty;
            bank_name = string.Empty;
            bank_branch = string.Empty;
            bank_account = string.Empty;
            bank_note = string.Empty;
            account_name = string.Empty;
            money_createdate = 0;
            money_updatedate = 0;
            money_ipfrom = string.Empty;
        }
    }
}