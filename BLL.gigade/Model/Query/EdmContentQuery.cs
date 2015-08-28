using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
  public  class EdmContentQuery : EdmContent
    {
      public uint content_send_count { get; set; }
      public DateTime s_content_start { get; set; }
      public int serialvalue { get; set; }
      public string group_name { get; set; }
      public string searchStatus { get; set; }
      public string search_text { get; set; }
      public DateTime s_content_end { get; set; }
      public EdmContentQuery()
      {
          content_send_count = 0;
          s_content_start = DateTime.MinValue;
          serialvalue = 0;
          group_name = string.Empty;
          searchStatus = string.Empty;
          search_text = string.Empty;
          s_content_end = DateTime.MinValue;
      }
    }
}
