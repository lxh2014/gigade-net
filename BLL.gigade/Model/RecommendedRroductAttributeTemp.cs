using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class RecommendedRroductAttributeTemp:PageBase
    {
        public uint product_id { get; set; }
        public int write_id { get; set; }
        public uint time_start { get; set; }
        public uint time_end { get; set; }
        public uint expend_day { get; set; }
        public string months { get; set; }
        public int combo_type { get; set; }
        public RecommendedRroductAttributeTemp()
        {
            product_id = 0;
            write_id = 0;
            time_start = 0;
            time_end = 0;
            expend_day = 0;
            months = string.Empty;
            combo_type = 0;
        }
    }
}
