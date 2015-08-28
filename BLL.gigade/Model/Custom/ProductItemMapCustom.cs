using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class ProductItemMapCustom : ProductItemMap
    {
        public string channel_name_full { get; set; }
        public uint set_num { get; set; }
        /// <summary>
        /// 對照中組合item的數量集
        /// </summary>
        public string group_item_num { get; set; }
        public string strChild { get; set; }
        public List<ProductMapSet> MapChild { get; set; }
        public uint groupcombo_product_id { get; set; }

        public uint site_id { get; set; }
        public uint user_level { get; set; }
        public uint user_id { get; set; }

        // edit by xiangwang0413w 2014/07/14 增加站台、會員等級、會員email
        public string site_name { get; set; }
        public string user_email { get; set; }
        public string user_level_name { get; set; }
        public ProductItemMapCustom()
        {
            channel_name_full = string.Empty;
            group_item_num = string.Empty;
            set_num = 0;
            strChild = string.Empty;
            MapChild = new List<ProductMapSet>();
            groupcombo_product_id = 0;

            site_name = string.Empty;
            user_email = string.Empty;
            user_level_name = string.Empty;

            site_id = 0;
            user_level = 0;
            user_id = 0;
        }
    }
}
