using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductComboTemp : ProductCombo
    {
        public new string Parent_Id { get; set; }
        public new string Child_Id { get; set; }
        public int Writer_Id { get; set; }
        public int Combo_Type { get; set; }
        
        public ProductComboTemp()
        {
            Parent_Id = "0";
            Child_Id = "0";
            Writer_Id = 0;
        }
    }
}
