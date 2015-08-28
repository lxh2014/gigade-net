using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ShippingVoucherQuery : ShippingVoucher
    {
        public string user_name { get; set; }
        /// <summary>
        /// 會員等級
        /// </summary>
        public string ml_name { get; set; }

        public string state_name { get; set; }
        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime created_start { get; set; }
        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime created_end { get; set; }
        public ShippingVoucherQuery()
        {
            user_name = string.Empty;
            ml_name = string.Empty;
            state_name = string.Empty;
            created_end = DateTime.MinValue;
            created_start = DateTime.MinValue;
        }
    }
}
