using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class VoteArticle : PageBase
    {
        public int article_id { get; set; }
        public uint product_id { get; set; }
        public int event_id { get; set; }
        public int user_id { get; set; }
        public string article_content { get; set; }
        public int article_status { get; set; }
        public string article_title { get; set; }
        public string article_banner { get; set; }
        public int create_user { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
        public int update_user { get; set; }
        public int vote_count { get; set; }
        public DateTime article_start_time { get; set; }
        public DateTime article_end_time { get; set; }
        public DateTime article_show_start_time { get; set; }
        public DateTime article_show_end_time { get; set; }
        
    }
}