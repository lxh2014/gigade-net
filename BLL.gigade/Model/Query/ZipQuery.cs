using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ZipQuery : Zip
    {
        public string zipcode { get; set; }
        public string zipname { get; set; }

        public ZipQuery()
        {
            zipcode = string.Empty;
            zipname = string.Empty;
        }
    }
}
