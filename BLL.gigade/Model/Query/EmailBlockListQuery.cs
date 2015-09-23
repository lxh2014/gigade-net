using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public  class EmailBlockListQuery:EmailBlockList
    {
       public string username { get; set; }
       public EmailBlockListQuery()
       {
           username = string.Empty;
       }
    }
}
