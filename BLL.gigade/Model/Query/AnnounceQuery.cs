using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class AnnounceQuery : Announce
    {
        public string c_name { get; set; }
        public string u_name { get; set; }
        public DateTime create_date { get; set; }
        public DateTime modify_date { get; set; }
        public string type_name { get; set; }
        public string key { get; set; }
        public int con_status { get; set; }
        public AnnounceQuery()
        {
            c_name = string.Empty;
            u_name = string.Empty;
            create_date = DateTime.MinValue;
            modify_date = DateTime.MinValue;
            con_status = -1;

        }
    }
}
