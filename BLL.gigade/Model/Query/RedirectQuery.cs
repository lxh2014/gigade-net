using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public  class RedirectQuery:Redirect
    {
        public string group_name { set; get; }
        public int selsum { set; get; }
        public string user { set; get; }
        public string order { set; get; }
        public DateTime sredirect_createdate { get; set; }
        public DateTime sredirect_updatedate { get; set; }

       public RedirectQuery()
       {
           group_name = string.Empty;
           selsum = 0;
           sredirect_createdate = DateTime.MinValue;
           sredirect_updatedate = DateTime.MinValue;
       }
    }
}
