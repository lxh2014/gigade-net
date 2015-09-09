using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
  public  class SiteAnalytics:PageBase
    {
      public int sa_id { get; set; }
      public DateTime sa_date { get; set; }
      public int sa_work_stage { get; set; }
      public int sa_user { get; set; }
      public DateTime sa_create_time { get; set; }
      public int sa_create_user { get; set; }

      //query
      public string s_sa_date { get; set; }
      public string s_sa_create_time { get; set; }
      public string s_sa_create_user { get; set; }
      public int search_con { get; set; }
      public string sa_ids { get; set; }
      public SiteAnalytics()
      {
          sa_id = 0;
          sa_date = DateTime.MinValue;
          sa_work_stage = 0;
          sa_user = 0;
          sa_create_time = DateTime.MinValue;
          sa_create_user = 0;

          //query
          s_sa_date = string.Empty;
          s_sa_create_time = string.Empty;
          s_sa_create_user = string.Empty;
          search_con = 0;
          sa_ids = string.Empty;
      }
    }
}
