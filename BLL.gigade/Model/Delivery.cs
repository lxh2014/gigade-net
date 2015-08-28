using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Delivery : PageBase
    {
        public string De000 { get; set; }
        public string De001 { get; set; }
        public string De002 { get; set; }
        public int De003 { get; set; }
        public string De004 { get; set; }
        public string De005 { get; set; }
        public string De006 { get; set; }
        public DateTime De007 { get; set; }
        public string De008 { get; set; }
        public string Cust02 { get; set; }
        public string Cust03 { get; set; }
        public string Cust04 { get; set; }
        public string Cust05 { get; set; }
        public Delivery()
        {
            De000 = string.Empty;
            De001 = string.Empty;
            De002 = string.Empty;
            De003 = 0;
            De004 = string.Empty;
            De005 = string.Empty;
            De006 = string.Empty;
            De007 = DateTime.MinValue;
            De008 = string.Empty;
            Cust02 = string.Empty;
            Cust03 = string.Empty;
            Cust04 = string.Empty;
            Cust05 = string.Empty;
        }
    }
}
