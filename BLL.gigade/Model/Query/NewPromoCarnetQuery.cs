using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class NewPromoCarnetQuery : NewPromoCarnet
    {
        public int isEdit { get; set; }
        public string s_promo_image { get; set; }
        public Int64 present_num { get; set; }//該活動的有效贈品數
        public int condition { get; set; }
        public NewPromoCarnetQuery()
        {
            isEdit = 0;
            s_promo_image = string.Empty;
            present_num = 0;
            condition = 0;
        }
    }
}
