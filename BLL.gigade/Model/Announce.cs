using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Announce : PageBase
    {
        public uint announce_id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public uint sort { get; set; }
        public uint status { get; set; }
        public uint type { get; set; }
        public uint creator { get; set; }
        public uint create_time { get; set; }
        public uint modifier { get; set; }
        public uint modify_time { get; set; }
        public Announce()
        {
            announce_id = 0;
            title = string.Empty;
            content = string.Empty;
            sort = 0;
            status = 0;
            type = 0;
            creator = 0;
            create_time = 0;
            modifier = 0;
            modify_time = 0;

        }
    }
}