using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class PromotionsBonusSerial:PageBase
    {
        public int id { get; set; }
        public int promotion_id { get; set; }
        public string serial { get; set; }
        public uint active { get; set; }
        public int myid { get; set; }
        public PromotionsBonusSerial()
        {
            id = 0;
            promotion_id = 0;
            serial = "";
            active = 0;
            myid = 0;
        }
    }
}
