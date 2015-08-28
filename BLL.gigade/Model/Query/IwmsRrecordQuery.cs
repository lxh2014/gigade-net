using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class IwmsRrecordQuery : IwmsRrecord
    {
        public string product_name { get; set; }
        public string user_username { get; set; }
        public string datetype { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public uint item_id { get; set; }
        public string product_spec_name { get; set; }
        public uint product_id { get; set; }
        public IwmsRrecordQuery()
        {
            product_name = string.Empty;
            user_username = string.Empty;
            datetype = string.Empty;
            starttime = DateTime.MinValue;
            endtime = DateTime.MinValue;
            item_id = 0;
            product_spec_name = string.Empty;
            product_id = 0;
        }
    }
}
