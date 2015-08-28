using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public class SitePageQuery:SitePage
    {
       public string page_shortHtml { get; set; }
       public SitePageQuery()
       {
           page_shortHtml = string.Empty;
       }
    }
}
