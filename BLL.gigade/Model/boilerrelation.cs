using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class boilerrelation:PageBase
    {
        public int boiler_id { get; set; }
        public string boiler_type { get; set; }//對應安康內鍋型號
        public string boiler_describe { get; set; }//對應安康內鍋型號詳細信息
        public string inner_boiler_number { get; set; }//內鍋型號
        public string out_boiler_number { get; set; }//外鍋型號
        public int add_user { get; set; }
        public DateTime add_time { get; set; }
        public string boiler_remark { get; set; } //鍋備註

        public boilerrelation()
        {
            boiler_id = 0;
            boiler_type = string.Empty;
            boiler_describe = string.Empty;
            inner_boiler_number = string.Empty;
            out_boiler_number = string.Empty;
            add_user = 0;
            add_time = DateTime.MinValue;
            boiler_remark = string.Empty;
        }
    }
}
