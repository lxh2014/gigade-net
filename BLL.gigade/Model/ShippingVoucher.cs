using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ShippingVoucher:PageBase
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int sv_id { get; set; }
        /// <summary>
        /// 會員編號
        /// </summary>
        public int user_id { get; set; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public int order_id { get; set; }
        /// <summary>
        /// 免運劵狀態: 0)未使用 1)已使用 2)未使用已到期
        /// </summary>
        public int sv_state { get; set; }
        /// <summary>
        /// 免運劵發放年份
        /// </summary>
        public int sv_year { get; set; }
        /// <summary>
        /// 免運劵發放月份
        /// </summary>
        public int sv_month { get; set; }
        /// <summary>
        /// 創建時間
        /// </summary>
        public DateTime sv_created { get; set; }
        /// <summary>
        /// 異動時間
        /// </summary>
        public DateTime sv_modified { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string sv_note { get; set; }

        public ShippingVoucher()
        {
            sv_id = 0;
            user_id = 0;
            order_id = 0;
            sv_state = 0;
            sv_year = 0;
            sv_month = 0;
            sv_created = DateTime.MinValue;
            sv_modified = DateTime.MinValue;
            sv_note = string.Empty;

        }

    }
}
