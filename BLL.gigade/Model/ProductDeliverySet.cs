using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    [Serializable]
    public class ProductDeliverySet:PageBase
    {
        //public int Rid { get; set; } //序號
        public int Product_id { get; set; } //商品編號
        public int Freight_big_area { get; set; } //配送區域
        public int Freight_type { get; set; } //配送模式

         public ProductDeliverySet()
         {
             //Rid = 0;
             Product_id = 0;
             Freight_big_area = 0;
             Freight_type = 0;
         }
    }
}
