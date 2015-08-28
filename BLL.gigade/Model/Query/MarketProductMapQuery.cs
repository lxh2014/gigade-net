using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public  class MarketProductMapQuery : MarketProductMap
    {
       public string map_id_in { set; get; }
       public string product_number { set; get; }//數字編號，(商品/美安)類別數字編號
       public string product_name { set; get; }//類別名稱，(商品/美安)類別名稱

       public MarketProductMapQuery()
       {
           map_id_in = string.Empty;
           product_number = string.Empty;
           product_name = string.Empty;
       }
    }
}
