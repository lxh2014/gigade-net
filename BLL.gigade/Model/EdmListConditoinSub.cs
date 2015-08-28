using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EdmListConditoinSub:PageBase
    {
        public int elcs_id { get; set; }//主鍵
        public int elcm_id { get; set; }//篩選條件表id
        //gender, age, register_time, last_order, last_login, buy_times, cancel_times, return_times, replenishment_info, total_consumption
        public string elcs_key { get; set; }//鍵值 gender...
        public string elcs_value1 { get; set; }//值1
        public string elcs_value2 { get; set; }//值2
        public string elcs_value3 { get; set; }//值3
        public string elcs_value4 { get; set; }//值4
        public EdmListConditoinSub()
        {
            elcs_id = 0;
            elcm_id = 0;
            elcs_key = string.Empty;
            elcs_value1 = string.Empty;
            elcs_value2 = string.Empty;
            elcs_value3 = string.Empty;
            elcs_value4 = string.Empty;
        }      
    }
}
