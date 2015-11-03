using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class XmlModelCustom
    {
        public int rowId { get; set; }///Id索引
        public string xmlStr { get; set; }
        public string baseUrl { get; set; }///文件所在位置
        public string fileName { get; set; }///所屬的xml文件名稱
        public string name { get; set; }///節點名稱
        public string brotherName { get; set; }///兄弟節點名稱
        public string childName { get; set; }///子節點名稱
        public int parentId { get; set; }
        public string parentName { get; set; }///父節點名稱
        public string code { get; set; }///子節點的內容
        public bool isLastNode { get; set; }///是否是最里面一層節點
        public bool isTopNode { get; set; }///是否是最頂層節點
        public bool hasAttributes { get; set; }///是否存在屬性
        public bool expanded { get; set; }///是否展開
        public string attributes { get; set; }///包含Attributes信息
        public string attributesXml { get; set; }///屬性信息xml版本
        public List<XmlModelCustom> children { get; set; }///包含的子節點
                 
        ///節點屬性                       
        public string text { get; set; }
        public bool Checked { get; set; }

        public bool leaf
        {
            get { return this.isLastNode; }
        }

        public XmlModelCustom() 
        {
            rowId = 0;
            xmlStr = string.Empty;
            fileName = string.Empty;
            name = string.Empty;
            parentId = 0;
            parentName = string.Empty;
            code = string.Empty;
            expanded = true;
            isLastNode = false;
            isTopNode = false;
            hasAttributes = false;
            children = new List<XmlModelCustom>();
            attributes = string.Empty;
            text = string.Empty;
            Checked = false;
            baseUrl = string.Empty;
            attributesXml = string.Empty;
        }
    }
}
