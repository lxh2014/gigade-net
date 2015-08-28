using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public class NewsContentQuery : NewsContent
    {
       public string user_username { get; set; }
       public DateTime s_news_show_start{get;set;}
       public DateTime s_news_show_end { get; set; }
       public DateTime s_news_createdate { get; set; }
       public DateTime s_news_updatedate { get; set; }
       public int serialvalue { get; set; }
       public int log_id { get; set; }
       public int log_value { get; set; }
       public string log_description { get; set; }
       public string searchCon { get; set; }
       public string search_con { get; set; }
       public string date { get; set; }
       public DateTime start_time { get; set; }
       public DateTime end_time { get; set; }
       public NewsContentQuery()
       {
           user_username = string.Empty;
           s_news_show_start = DateTime.MinValue;
           s_news_show_end = DateTime.MinValue;
           s_news_createdate = DateTime.MinValue;
           s_news_updatedate = DateTime.MinValue;
           serialvalue = 0;
           log_value = 0;
           searchCon = string.Empty;
           search_con = string.Empty;
           date = string.Empty;
           start_time = DateTime.MinValue;
           end_time = DateTime.MinValue;
       }
    }
}
