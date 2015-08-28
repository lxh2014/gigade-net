using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class OrderMasterPattern : PageBase
    {
        public int ID { get; set; }
        public int Order_ID { get; set; }
        public int Pattern { get; set; }
        public int Dep { get; set; }

        public OrderMasterPattern()
        {
            ID = 0;
            Order_ID = 0;
            Pattern = 0;
            Dep = 0;
        }
    }
}