using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.APIModels
{
    public class AesResult
    {
        public string text { get; set; }
        /// <summary>
        /// 計算后文本
        /// </summary>
        public string computed_text { get; set; }
    }
}
