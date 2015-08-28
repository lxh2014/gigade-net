using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class boilerrelationQuery:boilerrelation
    {
        public string Boiler_type_describe { get; set; }//安康內鍋型號和描述累加一起的值

        public string user_username { get; set; }

        public boilerrelationQuery()
        {
            Boiler_type_describe = string.Empty;
            user_username = string.Empty;
        }
    }
}
