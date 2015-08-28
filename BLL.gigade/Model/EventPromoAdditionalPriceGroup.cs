using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EventPromoAdditionalPriceGroup:PageBase
    {
        /// <summary>
        /// 商品群組編號
        /// </summary>
        public int group_id { get; set; }
        /// <summary>
        /// 群組名稱
        /// </summary>
        public string group_name { get; set; }
        /// <summary>
        /// 創建者
        /// </summary>
        public int create_user { get; set; }
        /// <summary>
        /// 創建時間
        /// </summary>
        public DateTime create_date { get; set; }
        /// <summary>
        /// 更新者
        /// </summary>
        public int modify_user { get; set; }
        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime modify_time { get; set; }
        /// <summary>
        /// 群組狀態
        /// </summary>
        public int group_status { get; set; }

        public EventPromoAdditionalPriceGroup()
        {
            group_id = 0;
            group_name = string.Empty;
            create_user = 0;
            create_date = DateTime.MinValue;
            modify_user = 0;
            modify_time = DateTime.MinValue;
            group_status = 0;
        }
    }
}
