using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
  public  class OrderDetailManagerQuery:OrderDetailManager
    {

      public string user_username { get; set; }

      public OrderDetailManagerQuery()
      {
          user_username = string.Empty;
      }
    }
}
