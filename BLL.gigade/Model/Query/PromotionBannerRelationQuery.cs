using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class PromotionBannerRelationQuery : PromotionBannerRelation
    {
        public string brand_name { get; set; }     
        public PromotionBannerRelationQuery()
        {
            brand_name = string.Empty;
        }
    }
}
