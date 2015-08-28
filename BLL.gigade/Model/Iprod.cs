using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
  public  class Iprod:PageBase
    {
      public int row_id { get; set; }
      public int dc_id { get; set; }
      public string prod_id { get; set; }
      public string ucn { get; set; }
      public string change_user { get; set; }
      public DateTime change_dtim { get; set; }
      public string create_user { get; set; }
      public DateTime create_dtim { get; set; }
      public string vend_prod_no { get; set; }
      public string description { get; set; }
      public string sdesc { get; set; }
      public string crush_factor { get; set; }
      public string psta_id { get; set; }
      public string pend_del { get; set; }
      public string commodity_type { get; set; }
      public string buyer_ref { get; set; }
      public string pwy_dte_ctl { get; set; }
      public int cde_dt_incr { get; set; }
      public string cde_dt_prod { get; set; }
      public int cde_dt_var { get; set; }
      public string hzd_ind { get; set; }
      public string hzd_class { get; set; }
      public int unit_ship_cse { get; set; }
      public string lot_no_cntl { get; set; }
      public double case_cost { get; set; }
      public Iprod()
      {
          row_id = 0;
          dc_id = 0;
          ucn = string.Empty;
          change_user = string.Empty;
          change_dtim = DateTime.MinValue;
          create_user = string.Empty;
          create_dtim = DateTime.MinValue;
          vend_prod_no = string.Empty;
          description = string.Empty;
          sdesc = string.Empty;
          crush_factor = string.Empty;
          psta_id = string.Empty;
          pend_del = string.Empty;
          commodity_type = string.Empty;
          buyer_ref = string.Empty;
          pwy_dte_ctl = string.Empty;
          cde_dt_incr = 0;
          cde_dt_prod = string.Empty;
          cde_dt_var = 0;
          hzd_ind = string.Empty;
          hzd_class = string.Empty;
          unit_ship_cse = 0;
          lot_no_cntl = string.Empty;
          case_cost = 0;
      }

    }
}
