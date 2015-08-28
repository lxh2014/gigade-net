/*
* 文件名稱 :SphinxKeyword.cs
* 文件功能描述 :系統關鍵字Model
* 版權宣告 :鄭州分公司
* 開發人員 : 文博
* 版本資訊 : 1.0
* 日期 : 2015-7-31
* 修改人員 :無
* 版本資訊 : 1.0
* 日期 : 2015-7-31
* 修改備註 :無
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class SphinxKeyword : PageBase
    {

        /// <summary>
        /// 編號
        /// </summary>
        public int row_id { get; set; }
        /// <summary>
        /// 關鍵字
        /// </summary>
        public string key_word { get; set; }
        /// <summary>
        /// 是否為食安關鍵字的標誌
        /// </summary>
        public string flag { get; set; }
        /// <summary>
        /// 創建日期
        /// </summary>
        public DateTime kdate { get; set; }
        /// <summary>
        /// 創建人
        /// </summary>
        public string kuser { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime mddate { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string moduser { get; set; }

        public SphinxKeyword()
        {
            this.row_id = 0;
            this.key_word = string.Empty;
            this.flag = string.Empty;
            this.kdate = DateTime.MinValue;
            this.kuser = string.Empty;
            this.mddate = DateTime.MinValue;
            this.moduser = string.Empty;
        }
    }
}
