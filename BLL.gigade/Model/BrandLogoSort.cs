using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
  public  class BrandLogoSort:PageBase
    {
      public int blo_id { get; set; }
      public uint category_id { get; set; }
      public int brand_id { get; set; }
      public int blo_sort { get; set; }
      public int blo_kuser { get; set; }
      public DateTime blo_kdate { get; set; }
      public int blo_muser { get; set; }
      public DateTime blo_mdate { get; set; }

      public string category_name { get; set; }
      public string brand_name { get; set; }
      public string user_username { get; set; }

      public int old_brand_id { get; set; }
      public BrandLogoSort()
      {
          blo_id = 0;
          category_id = 0;
          brand_id = 0;
          blo_sort = 0;
          blo_kuser = 0;
          blo_kdate = DateTime.Now;
          blo_muser = 0;
          blo_mdate = DateTime.Now;
          category_name = string.Empty;
          brand_name = string.Empty;
          old_brand_id = 0;
      }
    }
}
