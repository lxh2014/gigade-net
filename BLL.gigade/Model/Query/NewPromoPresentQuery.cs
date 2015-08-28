using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class NewPromoPresentQuery : NewPromoPresent
    {
        public string product_name { get; set; }
        public string user_username { get; set; }
        public NewPromoPresentQuery()
        {
            product_name = string.Empty;
            user_username = string.Empty;
        }
    }
}
