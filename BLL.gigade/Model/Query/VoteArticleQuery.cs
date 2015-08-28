using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class VoteArticleQuery : VoteArticle
    {
        public string time_start { get; set; }
        public string time_end { get; set; }
        public string kendo_editor { get; set; }
        public string creat_name { get; set; }
        public string upd_name { get; set; }
        public string event_name { get; set; }
        public string product_name { get; set; }
        public string name { get; set; }
        public int article_sort { get; set; }
        public Int64 reception_count { set; get; }
        public string prod_link { get; set; }
        
    }
}