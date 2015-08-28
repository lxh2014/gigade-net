using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    /// <summary>
    /// 訊息公告
    /// </summary>
    public class Appmessage:PageBase
    {
        public int message_id { get; set; }
        public int type { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public uint messagedate { get; set; }
        public string group { get; set; }
        public string linkurl { get; set; }
        public int display_type { get; set; }
        public uint msg_start { get; set; }
        public uint msg_end { get; set; }
        public string fit_os { get; set; }
        public string appellation { get; set; }
        public UInt64 need_login { get; set; }

        public Appmessage()
        {
            message_id = 0;
            type = 0;
            title = string.Empty;
            content = string.Empty;
            messagedate = 0;
            group = string.Empty;
            linkurl = string.Empty;
            display_type = 0;
            msg_start = 0;
            msg_end = 0;
            fit_os = string.Empty;
            appellation = string.Empty;
            need_login = 0;
        }

    }
}
