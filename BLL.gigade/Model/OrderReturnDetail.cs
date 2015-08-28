using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    [DBTableInfo("order_return_detail")]
    public class OrderReturnDetail : PageBase
    {

        public uint return_id { get; set; }
        public uint detail_id { get; set; }

        public OrderReturnDetail()
        {
            return_id = 0;
            detail_id = 0;
        }
    }
}