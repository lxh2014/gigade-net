using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class PromotionsBonusSerialHistory : PageBase
    {

        public int id { get; set; }
        public int promotion_id { get; set; }
        public string serial { get; set; }
        public int user_id { get; set; }
        public DateTime created { get; set; }

        public PromotionsBonusSerialHistory()
        {
            id = 0;
            promotion_id = 0;
            serial = string.Empty;
            user_id = 0;
            created = DateTime.MinValue;
        }
    }
}
