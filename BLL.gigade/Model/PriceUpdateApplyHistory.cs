using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class PriceUpdateApplyHistory : PageBase
    {
        public int apply_id { get; set; }
        public int user_id { get; set; }
        public int price_status { get; set; }
        public int type { get; set; }
        public string remark { get; set; }

        public PriceUpdateApplyHistory()
        {
            apply_id = 0;
            user_id = 0;
            price_status = 0;
            type = 0;
            remark = string.Empty;
        }
    }
}
