/*
* 文件名稱 :SphinxKeywordQuery.cs
* 文件功能描述 :擴展系統關鍵字Model
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

namespace BLL.gigade.Model.Query
{
    public class SphinxKeywordQuery : SphinxKeyword
    {
        /// <summary>
        /// 操作類型 默認0; 1增,2改
        /// </summary>
        public int operateType { get; set; }
        public string searchKey { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string[] ArrId { get; set; }
        /// <summary>
        /// 當前用戶名稱
        /// </summary>
        public string user_name { get; set; }
        public SphinxKeywordQuery()
        {
            this.operateType = 0;
            searchKey = string.Empty;
            this.startTime = DateTime.MinValue;
            this.endTime = DateTime.MinValue;
            this.user_name = string.Empty;
        }
    }
}
