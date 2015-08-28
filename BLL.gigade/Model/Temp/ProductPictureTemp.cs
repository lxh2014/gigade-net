using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductPictureTemp:ProductPicture
    {
        public new string product_id { get; set; }
        public int writer_Id { get; set; }
        public int combo_type { get; set; }

        public ProductPictureTemp()
        {
            product_id = "0";
            writer_Id = 0;
            combo_type = 0;
        }
    }
}
