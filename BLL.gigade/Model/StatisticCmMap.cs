using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    /// <summary>
    /// 產品品類管理表
    /// </summary>
    public class StatisticCmMap
    {
        public int Id { get; set; }
        public int product_id { get; set; }
        public int category_id { get; set; }

        public StatisticCmMap()
        {
            Id = 0;
            product_id = 0;
            category_id = 0;
        }
    }
}
