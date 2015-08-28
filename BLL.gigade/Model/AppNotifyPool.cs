using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    /// <summary>
    /// 推播設定
    /// </summary>
    public class AppNotifyPool:PageBase
    {
        public int id { get; set; }
        public string title { get; set; }
        public string alert { get; set; }
        public string url { get; set; }
        public string to { get; set; }
        public int valid_start { get; set; }
        public int valid_end { get; set; }
        public int notified { get; set; }
        public int notify_time { get; set; }

        public AppNotifyPool()
        {
            id = 0;
            title = string.Empty;
            alert = string.Empty;
            url = string.Empty;
            to = string.Empty;
            valid_start = 0;
            valid_end = 0;
            notified = 0;
            notify_time = 0;
        }
    }
}
