using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class IpodQuery : Ipod
    {
        public string row_ids { set; get; }
        public string loc_id{set;get;}
        public int product_freight_set{set;get;}
        public string spec_title_1{set;get;}
        public string spec_title_2{set;get;}
        public string  spec{set;get;}
        public uint item_id { get; set; }
        public uint product_id { get; set; }
        public string product_name { set; get; }
        public string upc_id { set; get; }
        public string user_username { set; get; }
        public string parameterName { set; get; }
        public string ParameterCode { set; get; }
        public string Erp_Id { get; set; }
        
        public IpodQuery() 
        {
            row_ids = string.Empty;
            loc_id = string.Empty;
            product_freight_set = 0;
            spec_title_1 = string.Empty;
            spec_title_2 = string.Empty;
            spec = string.Empty;
            product_id = 0;
            product_name = string.Empty;
            item_id = 0;
            upc_id = string.Empty;
            user_username = string.Empty;
            ParameterCode=string.Empty;
            Erp_Id = string.Empty;
            
        }

    }
}
