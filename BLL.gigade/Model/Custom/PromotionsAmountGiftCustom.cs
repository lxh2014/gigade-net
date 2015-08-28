using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model.Custom
{
    public class PromotionsAmountGiftCustom : PageBase
    {
        //product_spec
        public uint spec_id { get; set; }
        public uint product_id { get; set; }
        public uint spec_type { get; set; }
        public string spec_name { get; set; }
        public uint spec_status { get; set; }

        //product_item
        public uint Item_Id { get; set; }
        public uint Spec_Id_1 { get; set; }
        public uint Item_Stock { get; set; }
        public uint Item_Alarm { get; set; }
        public byte Item_Status { get; set; }

        public PromotionsAmountGiftCustom()
        {
            spec_id = 0;
            product_id = 0;
            spec_type = 0;
            spec_name = string.Empty;
            spec_status = 0;


            Item_Id = 0;
            Spec_Id_1 = 0;
            Item_Stock = 0;
            Item_Alarm = 0;
            Item_Status = 0;
        }

    }
}
