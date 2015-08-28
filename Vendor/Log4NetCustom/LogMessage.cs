using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vendor.Log4NetCustom
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