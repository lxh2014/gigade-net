using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class RedirectGroupQuery : RedirectGroup
    {
        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }
        public string parameterName { get; set; }
        public RedirectGroupQuery()
        {
            createdate = DateTime.MinValue;
            updatedate = DateTime.MinValue;
        }
    }
}
