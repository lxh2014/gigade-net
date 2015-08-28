using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductMigrationMap : PageBase
    {
        /// <summary>
        /// 商品編號
        /// </summary>
        public uint product_id { get; set; }
        /// <summary>
        /// 暫時編號
        /// </summary>
        public string temp_id { get; set; }

        public ProductMigrationMap()
        {
            product_id = 0;
            temp_id = string.Empty;
        }
    }
}
