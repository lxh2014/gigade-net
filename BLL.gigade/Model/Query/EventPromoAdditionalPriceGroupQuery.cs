using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class EventPromoAdditionalPriceGroupQuery : EventPromoAdditionalPriceGroup
    {
        public string group_ids { get; set; }
        public string user_username { get; set; }
        public EventPromoAdditionalPriceGroupQuery()
        {
            group_ids = string.Empty;
            user_username = string.Empty;
        }
    }
}
