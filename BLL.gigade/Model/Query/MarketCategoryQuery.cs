using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Model.Query
{
    public class MarketCategoryQuery : MarketCategory
    {
        public string market_category_father_name { get; set; }
        public string muser_name { get; set; }
        public MarketCategoryQuery()
        {
            market_category_father_name = string.Empty;
            muser_name = string.Empty;
        }
    }
}
