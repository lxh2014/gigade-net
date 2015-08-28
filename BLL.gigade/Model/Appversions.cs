using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    /// <summary>
    /// 上架版本
    /// </summary>
    public class Appversions:PageBase
    {
        public int id { get; set; }
        public int versions_id { get; set; }
        public int versions_code { get; set; }
        public string versions_name { get; set; }
        public string versions_desc { get; set; }
        public int drive { get; set; }
        public int release_type { get; set; }
        public int release_date { get; set; }

        public Appversions()
        {
            id = 0;
            versions_id = 0;
            versions_code = 0;
            versions_name = string.Empty;
            versions_desc = string.Empty;
            drive = -1;
            release_type = 0;
            release_date = 0;
        }
    }
}
