using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class ProductMapCustom : Model.ProductItemMap
    {
        public uint combination { get; set; }
        public int child_id { get; set; }
        public int g_must_buy { get; set; }
        public int s_must_buy { get; set; }
        public int pile_id { get; set; }
        public int buy_limit { get; set; }
        public uint product_spec { get; set; }
        public int cost { get; set; }
        public int price { get; set; }
        public int price_type { get; set; }

        public ProductMapCustom()
        {
            combination = 0;
            child_id = 0;
            g_must_buy = 0;
            s_must_buy = 0;
            pile_id = 0;
            buy_limit = 0;
            product_spec = 0;
            cost = 0;
            price = 0;
            price_type = 0;
        }
    }
}
