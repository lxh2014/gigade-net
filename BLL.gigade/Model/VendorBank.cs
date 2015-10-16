using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class VendorBank:PageBase
    {
        public int id { get; set; }
        public string bank_code { get; set; }
        public string bank_name { get; set; }
        public uint muser { get; set; }
        public uint kuser { get; set; }
        public DateTime mdate { get; set; }
        public DateTime kdate { get; set; }
        public int status { get; set; }

        public string muser_name { get; set; }

        public VendorBank()
        {
            id = 0;
            bank_code = string.Empty;
            bank_name = string.Empty;
            muser = 0;
            kuser = 0;
            mdate = DateTime.MinValue;
            kdate = DateTime.MinValue;
            status = 1;
            muser_name = string.Empty;

        }
    }
}
