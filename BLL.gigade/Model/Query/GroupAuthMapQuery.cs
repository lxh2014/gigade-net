using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{

    public class GroupAuthMapQuery : GroupAuthMap
    {
        public int user_id{get;set;}
        public string groupName { get; set; }
        public GroupAuthMapQuery()
        {
            user_id = 0;
            groupName = string.Empty;
        } 

    }
}
