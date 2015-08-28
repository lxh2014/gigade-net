using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
     public class MarketCategory : PageBase
    {
        public uint market_category_id { get; set; }
        public int market_category_father_id { get; set; }
        public string market_category_code { get; set; }
        public string market_category_name { get; set; }
        public int market_category_sort { get; set; }
        public int market_category_status { get; set; }
        public int kuser { get; set; }
        public int muser { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public string attribute { get; set; }

        public MarketCategory()
        {
            market_category_id = 0;
            market_category_father_id = 0;
            market_category_code = string.Empty;
            market_category_name = string.Empty;
            market_category_sort = 0;
            market_category_status = 1;
            kuser = 0;
            muser = 0;
            created = DateTime.MinValue;
            modified = DateTime.MinValue;
            attribute = string.Empty;
        }
    }
}
