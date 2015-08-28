using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class PromotionsAmountReduceMember : PageBase
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int group_id { get; set; }
        public int reduce_id { get; set; }
        public int order_id { get; set; }
        public int order_type { get; set; }
        public int order_status { get; set; }
        public DateTime created { get; set; }
        public PromotionsAmountReduceMember()
        {
            id = 0;
            user_id = 0;
            group_id = 0;
            reduce_id = 0;
            order_id = 0;
            order_type = 0;
            order_status = 0;
            created = DateTime.MinValue;
        }

    }
}
