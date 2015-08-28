using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SendMailService
{
    public class XmlHelper
    {
        public XmlHelper()
        {

        }

        /// <summary>
        /// 寫XML元素
        /// </summary>
        /// <param name="write">XmlWriter</param>
        /// <param name="elementName">元素名稱</param>
        /// <param name="value">元素的值</param>
        public void WriteElement(XmlWriter writer, string elementName, string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length == 0)
            {
                writer.WriteStartElement(elementName);
                writer.WriteFullEndElement();
            }
            else
            {
                writer.WriteElementString(elementName, value);
            }
        }

        /// <summary>
        /// 獲取XML子節點
        /// </summary>
        /// <param name="parent">父節點</param>
        /// <param name="name">子節點名稱</param>
        /// <param name="ignoring">是否忽略大小寫 true:忽略;fasle:不忽略</param>
        /// <returns></returns>
        public XmlNode GetXmlElemenetByName(XmlNode parent, string name, bool ignoring)
        {
            if (ignoring)
            {
                foreach (XmlNode element in parent.ChildNodes)
                {
                    if (element.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        return element;
                    }
                }
            }
            return parent[name];
        }

        /// <summary>
        /// 獲取XML子節點
        /// </summary>
        /// <param name="document">XML</param>
        /// <param name="name">子節點名稱</param>
        /// <param name="ignoring">是否忽略大小寫 true:忽略;fasle:不忽略</param>
        /// <returns></returns>
        public XmlNode GetXmlElemenetByName(XmlDocument document, string name, bool ignoring)
        {
            if (ignoring)
            {
                foreach (XmlNode element in document.ChildNodes)
                {
                    if (element.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        return (XmlNode)element;
                    }
                }
            }
            return document[name];
        }
    }
}
