using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class PageErrorLog:PageBase
    {

        #region 字段
        /// <summary>
        /// 流水編號
        /// </summary>
        public int rowID { get; set; }

        /// <summary>
        /// 錯誤頁面地址
        /// </summary>
        public string error_page_url { get; set; }
        /// <summary>
        /// 錯誤類型
        /// </summary>
        public int error_type { get; set; }
        /// <summary>
        /// 訪問時間
        /// </summary>
        public DateTime create_date { get; set; }

        /// <summary>
        /// 訪問IP
        /// </summary>
        public string create_ip { get; set; }
        #endregion

        public PageErrorLog()
        {
            this.rowID = -1;
            this.error_page_url = string.Empty;
            this.error_type = -1;
            this.create_date = DateTime.Now;
            this.create_ip = string.Empty;

        }
    }
}
