using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    [DBTableInfo("promotions_amount_reduce")]
    public class PromotionsAmountReduce : PageBase
    {
        public int id { get; set; }
        public string name { get; set; }
        public int delivery_store { get; set; }
        public int group_id { get; set; }
        public int type { get; set; }
        public int amount { get; set; }
        public int quantity { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public DateTime created { get; set; }
        public DateTime updatetime { get; set; }
        public int active { get; set; }
        public int status { get; set; }

        public PromotionsAmountReduce()
        {

            id = 0;
            name = string.Empty;
            delivery_store = 0;
            group_id = 0;
            type = 0;
            amount = 0;
            quantity = 0;
            start = DateTime.MinValue;
            end = DateTime.MinValue;
            created = DateTime.MinValue;
            updatetime = DateTime.MinValue;
            active = 0;
            status = 1;
        }

    }
}
