using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Zip : PageBase
    {
        public int RowID { get; set; }
        public string big { get; set; }
        public string bigcode { get; set; }
        public string middle { get; set; }
        public string middlecode { get; set; }
        public string zipcode { get; set; }
        public string small { get; set; }

        public Zip()
        {
            RowID = 0;
            big = string.Empty;
            bigcode = string.Empty;
            middle = string.Empty;
            middlecode = string.Empty;
            zipcode = string.Empty;
            small = string.Empty;
        }
    }
}
