/* 
 * 武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ParticularsSrc 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作    者：zhuoqin0830w 
 * 完成日期：2015/05/19
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

        public ParticularsSrc()
        {
            particularsName = string.Empty;
            particularsValid = 0;
            particularsCollect = 0;
            particularsCome = 0;
            oldCollect = 0;
            oldCome = 0;
        }
    }
}