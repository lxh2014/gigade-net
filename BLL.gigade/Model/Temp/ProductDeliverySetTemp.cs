using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductDeliverySetTemp : ProductDeliverySet
    {
        public int Writer_Id { get; set; }
        public int Combo_Type { get; set; }
        public ProductDeliverySetTemp()
        {
            Writer_Id=0;
            Combo_Type = 0;
        }
    }
}
