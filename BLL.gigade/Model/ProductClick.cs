using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductClick : PageBase
    {
        public uint site_id { get; set; }
        public uint product_id { get; set; }
        public uint click_id { get; set; }
        public uint click_year { get; set; }
        public uint click_month { get; set; }
        public uint click_day { get; set; }
        public uint click_hour { get; set; }
        public uint click_week { get; set; }
        public uint click_total { get; set; }
        public ProductClick()
        {
            site_id = 0;
            product_id = 0; 
            click_id = 0;
            click_year = 0;
            click_month = 0;
            click_day = 0;
            click_hour = 0;
            click_week = 0;
            click_total = 0;
        }
    }
}
