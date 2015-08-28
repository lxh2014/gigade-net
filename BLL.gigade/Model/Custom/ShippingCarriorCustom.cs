using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Model.Custom
{
    public class ShippingCarriorCustom : ShippingCarrior
    {
        public string Store_name { get; set; }
        public string Area_name { get; set; }
        public string Freight_type_Name { get; set; }

        public ShippingCarriorCustom()
        {
            Store_name = string.Empty;
            Area_name = string.Empty;
            Freight_type_Name = string.Empty;
        }
    }
}
