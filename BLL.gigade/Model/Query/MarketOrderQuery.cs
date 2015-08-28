using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class MarketOrderQuery:OrderMaster
    {
        public DateTime orderdate { get; set; }
        public uint item_id { get; set; }
        public string product_name { get; set; }
        public uint buy_num { get; set; }
        public uint parent_num { get; set; }
        public uint price { get; set; }
        public float Amount { get; set; }
        public string Market_Taiwan_RID_number { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public string rid { get; set; }
        public int deduct_happygo_money { get; set; }
        public uint deduct_welfare { get; set; }
        public uint deduct_bonus { get; set; }
        public int item_mode { get; set; }

    }
}
