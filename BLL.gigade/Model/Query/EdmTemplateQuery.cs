using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public  class EdmTemplateQuery:EdmTemplate
    {
       public string template_create_user { get; set; }
       public string template_update_user { get; set; }

       public EdmTemplateQuery()
       {
           template_create_user = string.Empty;
           template_update_user = string.Empty;
       }
    }
}
