using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class OrderMasterStatusQuery : OrderMasterStatus
    {
        public DateTime status_createdates { get; set; }
        public string states { get; set; }
        public uint slave_id { get; set; }

        public OrderMasterStatusQuery()
        {
            status_createdates = DateTime.MinValue;
            states = string.Empty;


        }
    }
}
