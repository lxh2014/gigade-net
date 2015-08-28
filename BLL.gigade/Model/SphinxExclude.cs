using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class SphinxExclude : PageBase
    {
        public int product_id { get; set; }
        public DateTime kdate { get; set; }
        public string kuser { get; set; }
        public SphinxExclude()
        {
            product_id = 0;
            kdate = DateTime.MinValue;
            kuser = string.Empty;
        }
    }
}
