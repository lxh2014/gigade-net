using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
  public  class NewsLogQuery : NewsLog
    {
        public string user_name { get; set; }
        public DateTime LogCreateDate { get; set; }

        public NewsLogQuery()
       {
           user_name = string.Empty;
           LogCreateDate = DateTime.MaxValue;
       }
    }
}
