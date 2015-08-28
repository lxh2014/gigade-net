using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class ProductComboMap
    {
        public uint item_id { get; set; }
        public int set_num { get; set; }
        public int g_must_buy { get; set; }
        public string product_name { get; set; }
        public string spec_name_1 { get; set; }
        public string spec_name_2 { get; set; }
        public int pile_id { get; set; }
        public uint product_spec { get; set; }
        public ProductComboMap()
        {
            item_id = 0;
            set_num = 0;
            product_name = string.Empty;
            spec_name_1 = string.Empty;
            spec_name_2 = string.Empty;
            pile_id = 0;
            product_spec = 0;
        }
    }
}
