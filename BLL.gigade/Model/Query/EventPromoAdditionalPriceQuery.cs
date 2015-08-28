using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class EventPromoAdditionalPriceQuery : EventPromoAdditionalPrice
    {
        public string user_username { get; set; }

        public EventPromoAdditionalPriceQuery()
        {
            user_username = string.Empty;
        }
    }
}
