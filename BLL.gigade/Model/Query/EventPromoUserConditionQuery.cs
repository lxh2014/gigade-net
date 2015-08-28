using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class EventPromoUserConditionQuery : EventPromoUserCondition
    {
        public string condition_id_tostring { get; set; }
        public string group_name { get; set; }
        public string ml_name { get; set; }
        public EventPromoUserConditionQuery()
        {
            condition_id_tostring = string.Empty;
            group_name = string.Empty;
            ml_name = string.Empty;
        }
    }
}
