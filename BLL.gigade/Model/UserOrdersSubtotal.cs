/*
* 文件名稱 :UserOrdersSubtotal.cs
* 文件功能描述 :會員消費統計表UserOrdersSubtotal對應Model
* 版權宣告 :鄭州分公司
* 開發人員 : 文博
* 版本資訊 : 1.0
* 日期 : 2015-7-28
* 修改人員 :無
* 版本資訊 : 1.0
* 日期 : 2015-7-28
* 修改備註 :無
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class UserOrdersSubtotal : PageBase
    {
        #region 屬性
        public int row_id { get; set; }
        /// <summary>
        /// 會員編號
        /// </summary>
        public uint user_id { get; set; }
        /// <summary>
        /// 當月累計消費金額
        /// </summary>
        public int order_product_subtotal { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public int year_month { get; set; }
        public int buy_count { get; set; }
        /// <summary>
        /// 最近消費時間
        /// </summary>
        public DateTime last_buy_time { get; set; }
        /// <summary>
        ///  當月平均購買價
        /// </summary>
        public int buy_avg { get; set; }
        /// <summary>
        /// 常溫商品總額
        /// </summary>
        public int normal_product_subtotal { get; set; }
        /// <summary>
        /// 低溫商品總額
        /// </summary>
        public int low_product_subtotal { get; set; }
        /// <summary>
        /// 創建時間
        /// </summary>
        public DateTime create_datetime { get; set; }
        /// <summary>
        /// 創建人
        /// </summary>
        public int create_user { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string note { get; set; }
        #endregion
        public UserOrdersSubtotal()
        {
            this.row_id = 0;
            this.user_id = 0;
            this.order_product_subtotal = 0;
            this.year = 0;
            this.month = 0;
            this.year_month = 0;
            this.buy_count = 0;
            this.last_buy_time = DateTime.MinValue;
            this.buy_avg = 0;
            this.normal_product_subtotal = 0;
            this.low_product_subtotal = 0;
            this.create_datetime = DateTime.MinValue;
            this.create_user = 0;
            this.note = "無";
        }
    }
}
