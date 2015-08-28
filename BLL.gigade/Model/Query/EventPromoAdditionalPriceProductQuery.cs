using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class EventPromoAdditionalPriceProductQuery : EventPromoAdditionalPriceProduct
    {
        public string group_name { get; set; }
        public string product_name { get; set; }
        public EventPromoAdditionalPriceProductQuery()
        {
            group_name = string.Empty;
            product_name = string.Empty;
        }
    }
}
