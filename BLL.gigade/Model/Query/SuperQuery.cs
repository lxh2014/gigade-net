using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class SuperQuery : Super
    {
        public string superSql { get; set; }
        public SuperQuery()
        {
            superSql = string.Empty;
        }
    }
}
