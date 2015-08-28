using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public class PageAreaQuery:PageArea
    {
       public string serchcontent { get; set; }
       public PageAreaQuery()
       {
           serchcontent = string.Empty;
       }
    }
}
