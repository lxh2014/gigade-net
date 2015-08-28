using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Pickup : PageBase
    {
        public string Pick001 { get; set; }
        public int Pick002 { get; set; }
        public string Pick003 { get; set; }
        public string Pick004 { get; set; }
        public DateTime Pick005 { get; set; }
        public string Pick006 { get; set; }
        public string De000 { get; set; }

        public Pickup()
        {
            Pick001 = string.Empty;
            Pick002 = 0;
            Pick003 = string.Empty;
            Pick004 = string.Empty;
            Pick005 = DateTime.MinValue;
            Pick006 = string.Empty;
            De000 = string.Empty;
        }
    }
}
