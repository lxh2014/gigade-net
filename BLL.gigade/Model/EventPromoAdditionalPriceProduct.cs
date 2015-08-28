using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EventPromoAdditionalPriceProduct : PageBase
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int row_id { get; set; }
        /// <summary>
        /// 群組編號
        /// </summary>
        public int group_id { get; set; }
        /// <summary>
        /// 商品編號
        /// </summary>
        public int product_id { get; set; }
        /// <summary>
        /// 創建者
        /// </summary>
        public int create_user { get; set; }
        /// <summary>
        /// 創建時間
        /// </summary>
        public DateTime create_time { get; set; }
        /// <summary>
        /// 加購價格
        /// </summary>
        public int add_price { get; set; }
        /// <summary>
        /// 異動者
        /// </summary>
        public int modify_user { get; set; }
        /// <summary>
        /// 異動時間
        /// </summary>
        public DateTime modify_time { get; set; }
        /// <summary>
        /// 商品狀態
        /// </summary>
        public int product_status { get; set; }

        public EventPromoAdditionalPriceProduct()
        {
            row_id = 0;
            group_id = 0;
            product_id = 0;
            create_user = 0;
            create_time = DateTime.MinValue;
            add_price = 0;
            modify_user = 0;
            modify_time = DateTime.MinValue;
            product_status = 1;
        }

    }
}
