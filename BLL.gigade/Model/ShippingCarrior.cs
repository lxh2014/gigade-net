using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ShippingCarrior : PageBase
    {
        public int Rid { get; set; }
        public int Delivery_store_id { get; set; }
        public int Freight_big_area { get; set; }
        public int Freight_type { get; set; }
        public int Delivery_freight_set { get; set; }
        public int Active { get; set; }
        public int Charge_type { get; set; }
        public int Shipping_fee { get; set; }
        public int Return_fee { get; set; }
        public int Size_limitation { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public int Pod { get; set; }
        public string Note { get; set; }

        public ShippingCarrior()
        {
            Rid = 0;
            Delivery_store_id = 0;
            Freight_big_area = 0;
            Freight_type = 0;
            Delivery_freight_set = 0;
            Active = 0;
            Charge_type = 0;
            Shipping_fee = 0;
            Return_fee = 0;
            Size_limitation = 0;
            Length = 0;
            Width = 0;
            Height = 0;
            Weight = 0;
            Pod = 0;
            Note = string.Empty;
        }
    }
}
