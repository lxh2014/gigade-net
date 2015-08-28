using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class MakePriceCustom : ItemPrice
    {
        public int Parent_Id { get; set; }
        public uint Child_Id { get; set; }
        public string Product_Name { get; set; }
        public string Prod_sz { get; set; }
        public string spec_1 { get; set; }
        public string spec_2 { get; set; }
        public int Pile_Id { get; set; }
        public int S_Must_Buy { get; set; }
        public int G_Must_Buy { get; set; }
        public int Buy_Limit { get; set; }
        public MakePriceCustom()
        {
            Parent_Id = 0;
            Child_Id = 0;
            Product_Name = string.Empty;
            spec_1 = string.Empty;
            spec_2 = string.Empty;
            Pile_Id = 0;
            S_Must_Buy = 0;
            G_Must_Buy = 0;
            Buy_Limit = 0;
        }
    }
}
