using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class DisableKeywords : PageBase
    {
        public int dk_id { get; set; }
        public string dk_string { get; set; }
        public int user_id { get; set; }
        public DateTime dk_created { get; set; }
        public int dk_modified { get; set; }
        public int dk_active { get; set; }
        
        public DisableKeywords()
        {
            dk_id = 0;
            dk_string = string.Empty;
            user_id = 0;
            dk_created = DateTime.MinValue;
            dk_modified = 0;
            dk_active = 0;
        }
    }
}
