using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EdmList : PageBase
    {
        //電子統計報表
        public uint statistics_id { get; set; }
        public uint content_id { get; set; }
        public uint total_click { get; set; }
        public uint total_person { get; set; }
        
        public EdmList()
        {
            statistics_id = 0;
            content_id = 0;
            total_click = 0;
            
        }
    }
}
