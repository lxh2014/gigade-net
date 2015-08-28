using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
  public  class InspectionReport : PageBase
    {
      public int rowID { get; set; }
      public uint product_id { get; set; }
      public string certificate_type1 { get; set; }
      public string certificate_type2 { get; set; }
      public DateTime certificate_expdate { get; set; }
      public string certificate_desc { get; set; }
      public string certificate_filename { get; set; }
      public int k_user { get; set; }
      public DateTime k_date { get; set; }
      public int m_user { get; set; }
      public DateTime m_date { get; set; }

      public InspectionReport()
      {
          rowID = 0;
          product_id = 0;
          certificate_type1 = string.Empty;
          certificate_type2 = string.Empty;
          certificate_expdate = DateTime.MinValue;
          certificate_desc = string.Empty;
          certificate_filename = string.Empty;
          k_user = 0;
          k_date = DateTime.MinValue;
          m_user = 0;
          m_date = DateTime.MinValue;

      }
    }
}
