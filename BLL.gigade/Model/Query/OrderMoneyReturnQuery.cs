using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public class OrderMoneyReturnQuery:OrderMoneyReturn
    {
       public string moneytype { get; set; }
       public string states{get;set;}
       public DateTime createdate { get; set; }
       public DateTime updatedate { get; set; }
       public string parameterName { get; set; }
       public string SearchStore { get; set; }
       public string searchContents { get; set; }
       public DateTime start_date { get; set; }
       public DateTime end_date { get; set; }
       public string date { get; set; }

       public OrderMoneyReturnQuery()
       {
           moneytype = string.Empty;
           states = string.Empty;
           createdate = DateTime.MinValue;
           updatedate = DateTime.MinValue;
           parameterName = string.Empty;
           SearchStore = string.Empty;
           searchContents = string.Empty;
           date = string.Empty;
           start_date =DateTime.MinValue;
           end_date = DateTime.MinValue;
       }
    }
}
