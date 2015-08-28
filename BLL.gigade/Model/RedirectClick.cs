using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class RedirectClick : PageBase
    {
        public uint redirect_id { get; set; }
        public uint click_id { get; set; }
        public uint click_year { get; set; }
        public uint click_month { get; set; }
        public uint click_day { get; set; }
        public uint click_hour { get; set; }
        public uint click_week { get; set; }
        public uint click_total { get; set; }

        public RedirectClick()
        {
            redirect_id = 0;
            click_id = 0;
            click_year = 0;
            click_month = 0;
            click_day = 0;
            click_hour = 0;
            click_week = 0;
            click_total = 1;
        }
    }
}
