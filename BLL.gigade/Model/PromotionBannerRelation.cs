using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class PromotionBannerRelation : PageBase
    {
        public int pb_id { get; set; }
        public uint brand_id { get; set; }
        public PromotionBannerRelation()
        {
            pb_id = 0;
            brand_id = 0;
        }
    }
}
