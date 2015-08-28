using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class DeliverStatus:PageBase
    {
        public int id { get; set; }
        public int deliver_id { get; set; }
        public int state { get; set; }//商品送達狀態
        public DateTime settime { get; set; }
        public DateTime endtime { get; set; }//商品最後處理時間
        public int freight_type { get; set; }//物流配送模式
        public int Logistics_providers { get; set; }//物流商

        public DeliverStatus()
        {
            id = 0;
            deliver_id = 0;
            state = 0;
            settime = DateTime.MinValue;
            endtime = DateTime.MinValue;
            freight_type = 0;
            Logistics_providers = 0;
        }
    }
}
