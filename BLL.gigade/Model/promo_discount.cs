using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    //promo_discount
    [DBTableInfo("promo_discount")]
    public class PromoDiscount : PageBase
    {

        public int rid { get; set; }
        public string event_id { get; set; }
        public int quantity { get; set; }
        public int discount { get; set; }
        public int special_price { get; set; }
        public string kuser { get; set; }
        public DateTime kdate { get; set; }
        public string muser { get; set; }
        public DateTime mdate { get; set; }
        public int status { get; set; }
      

        public PromoDiscount()
        {
            rid = 0;
            event_id = string.Empty;
            quantity = 0;
            discount = 0;
            special_price = 0;
            kuser = string.Empty;
            kdate = DateTime.MinValue;
            muser = string.Empty;
            mdate = DateTime.MinValue;
            status = 1;

        }
    }
}