using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public  class IplasQuery:Iplas
    {
       public string product_name { get; set; }
       public string create_users { get; set; }
       public string lcat_id { get; set; }
       public int prod_qtys { get; set; }
       public string prod_sz { get; set; }
       public string upc_id { get; set; }
       public DateTime cde_dt { get; set; }
       public string startloc { get; set; }
       public string endloc { get; set; }
       public string searchcontent { get; set; }
       public int serch_type { set; get; }
       public int prepaid { get; set; }
       public DateTime starttime { get; set; }
       public DateTime endtime { get; set; }

       public IplasQuery()
       {
           product_name = string.Empty;
           create_users = string.Empty;
           lcat_id = string.Empty;
           prod_qtys = 0;
           prod_sz = string.Empty;
           upc_id = string.Empty;
           startloc = string.Empty;
           endloc = string.Empty;
           searchcontent = string.Empty;
           prepaid = 0;
           serch_type = 0;
       }
    }
}
