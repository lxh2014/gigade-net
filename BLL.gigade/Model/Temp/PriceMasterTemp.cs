using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class PriceMasterTemp:PriceMaster
    {
        public new string product_id { get; set; }
        public new string child_id { get; set; }
        public int writer_Id { get; set; }
        public int combo_type { get; set; }
        public PriceMasterTemp()
        {
            writer_Id = 0;
            combo_type = 0;

            product_id = "0";
            child_id = "0";
        }
    }
}
