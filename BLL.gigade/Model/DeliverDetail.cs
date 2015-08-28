using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class DeliverDetail : PageBase
    {
        public int id { get; set; }
        public uint deliver_id { get; set; }
        public uint detail_id { get; set; }
        public int delivery_status { get; set; }
        public DeliverDetail()
        {
            id = 0;
            deliver_id = 0;
            detail_id = 0;
            delivery_status = 0;
        }
    }
}
