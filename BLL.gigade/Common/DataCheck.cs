/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：DataCheck 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/26 13:12:04 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BLL.gigade.Common
{
    public class DataCheck
    {
        public DataCheck()
        { 
            
        }

        /// <summary>
        /// 是否為數字
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <returns></returns>
        public static bool IsNumeric(string str)
        {
            return IsRegexEx("^[0-9]*$", str);
        }

        public static bool IsDate(string str)
        {
            DateTime tmp = DateTime.MinValue;
            return DateTime.TryParse(str, out tmp);
        }

        /// <summary>
        /// 根據傳入驗證格式判斷源字符串是否匹配
        /// </summary>
        /// <param name="regexStr">驗證格式</param>
        /// <param name="sourceStr">驗證字符串</param>
        /// <returns>bool</returns>
        private static bool IsRegexEx(string regexStr, string sourceStr)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceStr))
                {
                    return false;
                }
                Regex regex = new System.Text.RegularExpressions.Regex(regexStr);
                return regex.IsMatch(sourceStr);
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
