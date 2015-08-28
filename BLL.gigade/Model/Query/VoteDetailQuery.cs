using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class VoteDetailQuery : VoteDetail
    {
        /// <summary>
        /// 搜索條件中起始時間
        /// </summary>
        public DateTime start_time { get; set; }
        /// <summary>
        /// 搜索條件中結束時間
        /// </summary>
        public DateTime end_time { get; set; }
        /// <summary>
        /// 用於搜索的條件內容
        /// </summary>
        public string searchContent { get; set; }
        /// <summary>
        /// 文章標題
        /// </summary>
        public string article_title { get; set; }
        /// <summary>
        /// 會員名稱
        /// </summary>
        public string user_name { get; set; }
    }
}