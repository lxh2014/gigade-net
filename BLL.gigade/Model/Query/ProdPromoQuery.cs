using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ProdPromoQuery : ProdPromo
    {
        public string user_username { get; set; }
        public string rids { get; set; }
        public ProdPromoQuery()
        {
            user_username = string.Empty;
            rids = string.Empty;
        }
    }
}
