using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class VoteEventQuery : VoteEvent
    {
        public string cuser { get; set; }
        public string uuser { get; set; }
        public VoteEventQuery()
        {
            cuser = string.Empty;
            uuser = string.Empty;
       }
    }
}