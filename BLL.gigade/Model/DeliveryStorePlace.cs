using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class DeliveryStorePlace : PageBase
    {
        public int dsp_id { get; set; }
        public string dsp_name { get; set; }
        public string dsp_address { get; set; }
        public string dsp_telephone { get; set; }
        public string dsp_big_code { get; set; }
        public string dsp_deliver_store { get; set; }
        public int dsp_status { get; set; }
        public string dsp_note { get; set; }
        public int create_user { get; set; }
        public DateTime create_time { get; set; }
        public int modify_user { get; set; }
        public DateTime modify_time { get; set; }

        public DeliveryStorePlace()
        {
            dsp_id = 0;
            dsp_name = string.Empty;
            dsp_address = string.Empty;
            dsp_telephone = string.Empty;
            dsp_big_code = string.Empty;
            dsp_deliver_store = string.Empty;
            dsp_status = 0;
            dsp_note = string.Empty;
            create_user = 0;
            create_time = DateTime.MinValue;
            modify_user = 0;
            modify_time = DateTime.MinValue;
        }
    }
}
