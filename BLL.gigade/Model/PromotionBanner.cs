using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class PromotionBanner:PageBase
    {
        public int pb_id { get; set; }
        public string pb_image { get; set; }
        public string pb_image_link { get; set; }
        public DateTime pb_startdate { get; set; }
        public DateTime pb_enddate { get; set; }
        public int pb_status { get; set; }
        public DateTime pb_kdate { get; set; }
        public int pb_kuser { get; set; }
        public DateTime pb_mdate { get; set; }
        public int pb_muser { get; set; }
        public PromotionBanner()
        {
            pb_id = 0;          
            pb_image = string.Empty;
            pb_image_link = string.Empty;
            pb_startdate = DateTime.MinValue;
            pb_enddate = DateTime.MinValue;
            pb_status = 0;
            pb_kdate = DateTime.MinValue;
            pb_kuser = 0;
            pb_mdate = DateTime.MinValue;
            pb_muser = 0;

        }
    }
}
