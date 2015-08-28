/*
* 文件名稱 :Parametersrc.cs
* 文件功能描述 :编辑参数表数据
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/27
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Parametersrc : PageBase
    {
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
        public string parameterName { get; set; }
        /// <summary>
        /// 上一級的代碼
        /// </summary>
        public string TopValue { get; set; }
        /// <summary>
        /// 錄入時間
        /// </summary>
        public DateTime Kdate { get; set; }
        /// <summary>
        /// 錄入人員
        /// </summary>
        public string Kuser { get; set; }
        /// <summary>
        /// 是否使用(0:否,1:是)
        /// </summary>
        public int Used { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        public string remark { get; set; }

        public Parametersrc()
        {
            Rowid = 0;
            ParameterType = string.Empty;
            ParameterProperty = string.Empty;
            ParameterCode = string.Empty;
            parameterName = string.Empty;
            TopValue = string.Empty;
            Kdate = DateTime.MinValue;
            Kuser = string.Empty;
            Used = 1;
            Sort = 0;
            remark = string.Empty;
        }

        public string getUsed()
        {
            return Used == 1 ? "可用" : "不可用";
        }

    }
}
