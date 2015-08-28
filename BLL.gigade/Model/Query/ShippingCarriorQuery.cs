using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ShippingCarriorQuery:ShippingCarrior
    {
        public string Delivery_store_name { get; set; }
        public ShippingCarriorQuery()
        {
            Delivery_store_name = string.Empty;
        }
    }
}
