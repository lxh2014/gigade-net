using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class CertificateCategory:PageBase
    {
        public int rowID { get; set; }
        public string certificate_categoryname { get; set; }
        public string certificate_categorycode { get; set; }
        public uint certificate_categoryfid { get; set; }
        public int status { get; set; }
        public uint k_user { get; set; }
        public DateTime k_date { get; set; }
        public CertificateCategory()
        {
            rowID = 0;
            certificate_categoryname = string.Empty;
            certificate_categorycode = string.Empty;
            certificate_categoryfid = 0;
            status = 0;
            k_user = 0;
            k_date = DateTime.MinValue;
        }
    }
}
