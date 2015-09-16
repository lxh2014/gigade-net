using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class SiteAnalytics : PageBase
    {
        public int sa_id { get; set; }
        public DateTime sa_date { get; set; }
        public int sa_session { get; set; }
        public int sa_user { get; set; }
        public DateTime sa_create_time { get; set; }
        public int sa_create_user { get; set; }
        public int sa_pageviews { get; set; }//瀏覽量
        public float sa_pages_session { get; set; }//單次造訪頁數
        public float sa_bounce_rate { get; set; }//跳出率
        public float sa_avg_session_duration { get; set; }//平均停留時間
        public DateTime sa_modify_time { get; set; }
        public int sa_modify_user { get; set; }
        //query

        public string s_sa_date { get; set; }
        public string s_sa_create_time { get; set; }
        public string s_sa_create_user { get; set; }
        public int search_con { get; set; }
        public string sa_ids { get; set; }
        public string sa_modify_username { get; set; }
        public string sa_modify_time_query { get; set; }
        public SiteAnalytics()
        {
            sa_id = 0;
            sa_date = DateTime.MinValue;
            sa_session = 0;
            sa_user = 0;
            sa_create_time = DateTime.MinValue;
            sa_create_user = 0;
            sa_pages_session = 0;
            sa_bounce_rate = 0;
            sa_pageviews = 0;
            sa_avg_session_duration = 0;
            sa_modify_time = DateTime.MinValue;
            sa_modify_user = 0;

            //query
            s_sa_date = string.Empty;
            s_sa_create_time = string.Empty;
            s_sa_create_user = string.Empty;
            search_con = 0;
            sa_ids = string.Empty;
            sa_modify_username = string.Empty;
            sa_modify_time_query = string.Empty;
        }
    }
}
