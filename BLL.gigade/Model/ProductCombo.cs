using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductCombo
    {
        public int Id { get; set; }
        public int Parent_Id { get; set; }
        public int Child_Id { get; set; }
        public int S_Must_Buy { get; set; }
        public int G_Must_Buy { get; set; }
        public int Pile_Id { get; set; }
        public int Buy_Limit { get; set; }

        public ProductCombo()
        {
            Id = 0;
            Parent_Id = 0;
            Child_Id = 0;
            S_Must_Buy = 0;
            G_Must_Buy = 0;
            Pile_Id = 0;
            Buy_Limit = 0;
        }
    }
}
