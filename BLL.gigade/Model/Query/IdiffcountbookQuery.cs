using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class IdiffcountbookQuery : Idiffcountbook
    {
        public string user_name { get; set; }

        public IdiffcountbookQuery()
        {
            user_name = string.Empty;
        }
    }
}
