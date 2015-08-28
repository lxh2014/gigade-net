/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：LogMessage 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/9/26 10:01:01 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.gigade.Log4NetCustom
{
    public class LogMessage
    {
        private string m_MethodName;
        private string m_Content;
        public LogMessage()
        {
        }
        public LogMessage(string methodName, string content)
        {
            m_MethodName = methodName;
            m_Content = content;
        }
        public string MethodName
        {
            get
            {
                return m_MethodName;
            }
            set
            {
                m_MethodName = value;
            }
        }
        public string Content
        {
            get
            {
                return m_Content;
            }
            set
            {
                m_Content = value;
            }
        }
    }
}