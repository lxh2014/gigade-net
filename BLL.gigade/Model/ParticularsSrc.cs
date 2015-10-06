/* 
 * 武漢聯綿信息技術有限公司
 *  
 * 文件名称：ParticularsSrc 
 * 摘    要： 
 *  
 * 当前版本：1.0 
 * 作    者：zhuoqin0830w 
 * 完成日期：2015/05/19
 * 
 * 修改日期：2015/09/29
 * 修改原因：將使用 xml 資料庫 改為使用 db 數據庫
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ParticularsSrc : PageBase
    {
        /// <summary>
        /// 節點名稱
        /// </summary>
        public string particularsName { get; set; }
        /// <summary>
        /// 有效天數
        /// </summary>
        public int particularsValid { get; set; }
        /// <summary>
        /// 允收天數
        /// </summary>
        public int particularsCollect { get; set; }
        /// <summary>
        /// 允出天數
        /// </summary>
        public int particularsCome { get; set; }
        /// <summary>
        /// 舊允收天數
        /// </summary>
        public int oldCollect { get; set; }
        /// <summary>
        /// 舊允出天數
        /// </summary>
        public int oldCome { get; set; }

        #region 借鑒 參數表的 參數  add by zhuoqin0830w 2015/09/29
        /// <summary>
        /// 編號
        /// </summary>
        public int Rowid { get; set; }
        /// <summary>
        /// 參數類型
        /// </summary>
        public string ParameterType { get; set; }
        /// <summary>
        /// 參數屬性
        /// </summary>
        public string ParameterProperty { get; set; }
        /// <summary>
        /// 參數代碼
        /// </summary>
        public string ParameterCode { get; set; }
        /// <summary>
        /// 參數內容
        /// </summary>
        public string ParameterName { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Remark { get; set; }
        #endregion

        public ParticularsSrc()
        {
            particularsName = string.Empty;
            particularsValid = 0;
            particularsCollect = 0;
            particularsCome = 0;
            oldCollect = 0;
            oldCome = 0;

            #region 借鑒 參數表的 參數
            Rowid = 0;
            ParameterType = string.Empty;
            ParameterProperty = string.Empty;
            ParameterCode = string.Empty;
            ParameterName = string.Empty;
            Remark = string.Empty;
            #endregion
        }
    }
}