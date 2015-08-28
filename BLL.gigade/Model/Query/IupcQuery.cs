using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class IupcQuery:Iupc
    {
        public string product_name { set; get; }

        public string create_users { set; get; }//创建人

        public string searchcontent { get; set; }
        public string searchcontentstring { get; set; }//當不是純數字時使用
        public int parametercode { set; get; }
        public string upc_type_flg_string { get; set; }
        public string create_time_start { set; get; }
        public string create_time_end { set; get; }
        public IupcQuery()
        {
            product_name = string.Empty;
            searchcontent = string.Empty;
            searchcontentstring = string.Empty;
            parametercode = 0;
            upc_type_flg_string = string.Empty;
            create_time_start = string.Empty;
            create_time_end = string.Empty;
        }
    }
}
 