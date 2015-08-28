using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
  public  class PromoShareMaster:PageBase
    {
      public int promo_id { get; set; }
      public string promo_name { get; set; }
      public string promo_desc { get; set; }
      public DateTime promo_start { get; set; }
      public DateTime promo_end { get; set; }
      public int promo_active { get; set; }
      public string promo_event_id { get; set; }

      public bool eventId { get; set; }
      public PromoShareMaster()
      {
          promo_id = 0;
          promo_name = string.Empty;
          promo_desc = string.Empty;
          promo_start = DateTime.MinValue;
          promo_end = DateTime.MinValue;
          promo_event_id = string.Empty;
          eventId = false;
      }
    }
}
