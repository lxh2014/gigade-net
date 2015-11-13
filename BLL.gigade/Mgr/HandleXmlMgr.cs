using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace BLL.gigade.Mgr
{
    public class HandleXmlMgr:IHandleXmlImplMgr
    {
        ///文件路徑
        private string baseUrl = "";

        ///文件名稱                    
        private string fileName = "";

        private string xmlStr = "";

        private int rowId = 1;

        public List<XmlModelCustom> GetXmlName(string fullPath)
        {
            try
            {
                this.baseUrl = fullPath;
                return OpenXmlFolder(fullPath);
            }
            catch (Exception ex)
            {
                throw new Exception("HandleXmlMgr-->GetXmlName" + ex.Message,ex);
            }
        }

        public string GetXmlInfo(string fullPath)
        {
            this.baseUrl = fullPath;

            string resultStr = string.Empty;

            List<XmlModelCustom> list = new List<XmlModelCustom>();
            try
            {
                ///加載xmlDoc
                XmlDocument xmlDoc = LoadXmlFolder(fullPath);
                ///定義一個空集合
                List<XmlModelCustom> xList = new List<XmlModelCustom>();
                ///獲得排序號的結果集合
                List<XmlModelCustom> sortResultList = ToSort(xmlDoc.ChildNodes, xList);

                ///得到xml的字符串格式????
                xmlStr = GetXmlStr(sortResultList, "#document", "");

                ///讓根節點的xmlStr屬性獲得預覽的xmlStr
                sortResultList.FindAll(m => m.parentName == "#document").FirstOrDefault<XmlModelCustom>().xmlStr = xmlStr;


                ///序列化結果集合
                resultStr = JsonConvert.SerializeObject(sortResultList);

                ///將大寫Checked換成小寫
                ///註釋掉后樹節點不會存在checkBox選擇
                //resultStr = resultStr.Replace("Checked", "checked");

                ///返回Json
                return resultStr;
            }
            catch (Exception ex)
            {
                throw new Exception("HandleXmlMgr-->GetXmlInfo" + ex.Message, ex);
            }
        }

        public bool CreateFile(string fullPath,string nodeName)
        {
            try
            {
                XmlDocument xmlDoc = LoadXmlFolder(fullPath);
                XmlElement xmlelem;
                XmlDeclaration xmldecl;
                xmldecl = xmlDoc.CreateXmlDeclaration("1.0", "gb2312", null);
                xmlDoc.AppendChild(xmldecl);
                xmlelem = xmlDoc.CreateElement("", nodeName, "");
                xmlDoc.AppendChild(xmlelem);//加入另外一个元素
                xmlDoc.Save(fullPath);//保存创建好的XML文档
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("HandleXmlMgr-->CreateFile" + ex.Message,ex);
            }
        }

        public bool DeleteFile(string fullPath)
        {
            try
            {
                if (File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("HandleXmlMgr-->DeleteFile"+ex.Message,ex);
            }
        }



        public bool SaveOrUpdate(string fullPath,XmlModelCustom xmc,int type)
        {
            try
            {
                XmlDocument xmlDoc = LoadXmlFolder(fullPath);
                return UpdateNode(xmlDoc,xmc,fullPath,type);
            }
            catch (Exception ex)
            {
                throw new Exception("HandleXmlMgr-->UpdateNode"+ ex.Message,ex);
            }
        }


        private bool UpdateNode(XmlDocument doc,XmlModelCustom xmc,string fullPath,int type)
        {
            try
            {

                
                int index_id = 0;
                ///獲得xmlDoc的所有節點
                XmlNodeList listXml = doc.GetElementsByTagName("*");
                ///定義一個handleXn變量
                XmlNode handleXn = null;
                ///循環遍歷節點,獲得選中節點
                foreach (XmlNode xn in listXml)
                {
                    ///獲得要修改的節點
                    if (index_id == xmc.rowId)
                    {
                        handleXn = xn;
                        break;
                    }
                    index_id++;
                }

                ///刪除本節點
                if(type==3)
                {
                    handleXn.ParentNode.RemoveChild(handleXn);

                    doc.Save(fullPath);

                    return true;
                }


                ///創建一個新節點
                 XmlElement newNode;

                 if (type == 1)
                 {
                     newNode = doc.CreateElement(xmc.brotherName);
                 }
                 else if (type == 2)
                 {
                     newNode = doc.CreateElement(xmc.childName);
                 }
                 else
                 {
                     newNode = doc.CreateElement(xmc.name);
                     
                 }
                ///分割屬性
                string [] attrArray = xmc.attributes.Split('|');

                ///設置屬性
                foreach(string attr in attrArray)
                {
                    if(attr!="")
                    {
                        string [] temp = attr.Split('=');
                        newNode.SetAttribute(temp[0],temp[1]);
                    }
                }

                ///設置內容
                //newNode.InnerText = xmc.code;

                if (type == 0)
                {
                    newNode = (XmlElement)LoopAddNode(newNode, handleXn);
                }

                ///添加新節點
                if (type == 1)
                {
                    handleXn.ParentNode.AppendChild(newNode);
                }
                if (type == 2)
                {
                    handleXn.AppendChild(newNode);
                }

                ///如果是修改節點
                if (type == 0)
                {
                    ///添加新節點
                    handleXn.ParentNode.AppendChild(newNode);
                    ///刪除掉舊節點
                    handleXn.ParentNode.RemoveChild(handleXn);
                }
                ///保存
                doc.Save(fullPath);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("HandleXmlMgr-->UpdateNode" + ex.Message,ex);
            }
            
        }


        private XmlNode LoopAddNode(XmlNode newNode,XmlNode oldNode)
        {
            try
            {
                if (oldNode.HasChildNodes)
                {
                    XmlNodeList childNode = oldNode.ChildNodes;
                    int index = 0;
                    int count = childNode.Count;///因為child.count在後面會改變所以先記錄下來
                    for (int i = 0; i < count; i++)
                    {
                        if (childNode[index] != null)
                        {
                            newNode.AppendChild(childNode[index]);
                        }
                    }
                    //newNode.AppendChild(childNode[0]);
                    //newNode.AppendChild(childNode[0]);
                    //foreach (XmlNode xn in childNode)
                    //{
                    //    XmlNode temp = xn;
                    //    newNode.AppendChild(temp);
                    //}
                }
                return newNode;
            }
            catch (Exception ex)
            {
                
                throw new Exception("HandleXmlMgr-->LoopAddNode" + ex.Message,ex);
            }
        }

        /// <summary>
        /// 打開指定文件夾
        /// </summary>
        /// <param name="fullPath">文件夾物理路徑</param>
        /// <returns>包含所有文件名稱的list<string>集合</returns>
        private List<XmlModelCustom> OpenXmlFolder(string fullPath)
        {
            List<XmlModelCustom> list = new List<XmlModelCustom>();
            try
            {
                if (fullPath != "" && !string.IsNullOrEmpty(fullPath))
                {
                    DirectoryInfo theFolder = new DirectoryInfo(fullPath);///打開指定文件夾
                    list = HandleXmlFolder(theFolder);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("HandleXmlMgr-->OpenXmlFolder" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 操作文件夾,讀取文件夾下的文件名稱
        /// </summary>
        /// <param name="theFolder">DirectoryInfo 對象</param>
        /// <returns>包含xml文件名的字符集</returns>
        private List<XmlModelCustom> HandleXmlFolder(DirectoryInfo theFolder)
        {
            try
            {
                if (theFolder == null) return new List<XmlModelCustom>(); ///如果為空直接返回list
                int indexTemp = 1;
                List<XmlModelCustom> list = new List<XmlModelCustom>();
                foreach (FileInfo NextFile in theFolder.GetFiles())///遍歷文件
                {
                    XmlModelCustom xmcTemp = new XmlModelCustom();///實例化XmlModelCustom
                    xmcTemp.rowId = indexTemp;
                    xmcTemp.fileName = NextFile.Name;///獲得xml文件名稱
                    xmcTemp.baseUrl = this.baseUrl;
                    indexTemp++;///通過+1,保證索引不會重複
                    list.Add(xmcTemp);///list集合添加
                }
                return list;///返回名字集合
            }
            catch (Exception ex)
            {
                throw new Exception("HandleXmlMgr-->HandleXmlFolder" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 加載指定路徑的xml文件
        /// </summary>
        /// <param name="fullPath">xml所在路徑</param>
        /// <returns>包含xml信息的XmlDocument實例</returns>
        private XmlDocument LoadXmlFolder(string fullPath="")
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (fullPath != "") 
            {
                if (System.IO.File.Exists(fullPath))
                {
                    xmlDoc.Load(fullPath);
                    System.IO.File.SetAttributes(fullPath, System.IO.FileAttributes.Normal);
                    fileName = baseUrl.Remove(0, baseUrl.LastIndexOf("\\") + 1);///文件名稱
                    //xmlStr = xmlDoc.InnerXml;
                    return xmlDoc;
                }
            }
            return xmlDoc;
        }


        private List<XmlModelCustom> ToSort(XmlNodeList xmlList,List<XmlModelCustom> sortList)
        {
            try
            {

                foreach (XmlNode xn in xmlList)
                {
                    if (xn.InnerText.IndexOf("version=\"1.0\"") >= 0 || xn.Name == "#text"||xn.Name=="#comment")
                    {
                        continue;
                    }
                    XmlModelCustom xTemp = new XmlModelCustom();
                    xTemp.isTopNode = xn.ParentNode.Name == "#document" ? true : false;///設置是否是根節點
                    if (xn.Attributes != null)
                    {
                        xTemp = GetAttributes((XmlElement)xn, xTemp);///設置和屬性相關內容
                    }
                    xTemp.fileName = this.fileName;
                    xTemp.name = xn.Name;///設置Node節點的名稱
                    xTemp.text = xn.Name;
                    xTemp.parentName = xn.ParentNode.Name;///設置父節點名稱
                    xTemp.isLastNode = xn.InnerText == xn.InnerXml ? true : false;///設置是否是最後節點(當InnerText屬性==InnerXml屬性時,可證明為最後節點)
                    xTemp.code = xTemp.isLastNode == true ? xn.InnerText : string.Empty;///設置節點中內容.只有最後節點才能有內容
                    if (xTemp.isLastNode == true && xTemp.code != "") xTemp.text = xTemp.code;
                    if (xTemp.isTopNode == true) xTemp.xmlStr = xmlStr;
                    sortList.Add(xTemp);
                    if (xn.HasChildNodes)
                    {
                        XmlNodeList xList = xn.ChildNodes;
                        xTemp.children = ToSort(xList, xTemp.children);
                    }
                }

                return sortList;
            }
            catch (Exception ex)
            {
                throw new Exception("HandleXmlMgr-->ToSortChildNode" + ex.Message, ex);
            }
        }



        /// <summary>
        /// 從xmlModelList中找出根節點
        /// </summary>
        /// <param name="xmlModelList">xmlModelList集合</param>
        /// <returns>根節點的集合</returns>
        private List<XmlModelCustom> GetRootNode(List<XmlModelCustom> xmlModelList)
        {
            try
            {
                if (xmlModelList.Count == 0) return new List<XmlModelCustom>();
                List<XmlModelCustom> tempList = xmlModelList.FindAll(m => m.isTopNode == true);
                if(tempList.Count==0)return xmlModelList;
                return tempList;
            }
            catch (Exception ex)
            {
                throw new Exception("HandleXmlMgr-->GetRootNode" + ex.Message,ex);
            }
        }





        //private List<XmlModelCustom> ToSortChildNodeaaa(List<XmlModelCustom> allXmlNodeList, List<XmlModelCustom> rootXmlList, XmlNodeList xmlList)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //         throw new Exception("HandleXmlMgr-->ToSortChildNode" + ex.Message, ex);
        //    }
        //}







    

        /// <summary>
        /// 獲取屬性相關信息
        /// </summary>
        /// <param name="xn">node節點</param>
        /// <returns>包含Attributes相關信息的XmlModeCustom對象</returns>
        private XmlModelCustom GetAttributes(XmlElement xn, XmlModelCustom xmlModelTemp)
        {
            
            string attris = string.Empty;
            string attrisXml = string.Empty;
            try
            {
                if (xn != null && xn.HasAttributes)
                {
                    ///是否有屬性
                    xmlModelTemp.hasAttributes = true;
                    ///獲得屬性集合
                    XmlAttributeCollection attributeList = xn.Attributes;
                    ///遍歷屬性
                    foreach (XmlAttribute attriTemp in attributeList)
                    {
                        ///如果有屬性 = index_id
                        if (attriTemp.Name == "index_id")
                        {
                            ///將該index_id 當做索引id
                            xmlModelTemp.rowId = Convert.ToInt32(attriTemp.Value);
                        }
                        ///累加attris
                        
                        attris += attriTemp.Name + "=" + attriTemp.Value + "|";
                        attrisXml += attriTemp.Name + "=\"" + attriTemp.Value + "\" "; 
                    }
                    ///將屬性賦予XmlModeCustom對象
                    xmlModelTemp.attributes = attris;
                    xmlModelTemp.attributesXml = attrisXml;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("HandleXmlMgr-->GetAttributes" + ex.Message,ex);
            }

            return xmlModelTemp;
        }

        /// <summary>
        /// 驗證節點是否存在索引屬性
        /// </summary>
        /// <param name="xmlList">驗證的節點集合</param>
        /// <returns>true :存在 false: 不存在</returns>
        private bool IsExistIndex(XmlNodeList xmlList)
        {
            int index = 0;
            try
            {
                ///遍歷所有節點
                foreach (XmlElement xn in xmlList)
                {
                    ///如果不存在屬性,則返回false
                    if (xn.HasAttributes == false)
                    {
                        return false;
                    }
                    ///獲得屬性的集合
                    XmlAttributeCollection attributeList = xn.Attributes;
                    ///遍歷屬性
                    foreach(XmlAttribute attr in attributeList)
                    {
                        ///index清0
                        index = 0;
                        ///如果有屬性為index_id
                        if (attr.Name == "index_id")
                        {
                            ///index ++
                            index++;
                        }
                    }
                    if (index != 1)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("HandleXmlMgr-->IsExistIndex" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 為每個節點添加屬性
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="xmlList"></param>
        /// <returns></returns>
        private XmlDocument AddIndexAttributes(XmlDocument doc)
        {
            try
            {
                ///為空返回null
                if (doc == null) return doc;
                ///節點的index_id唯一屬性
                int index_id = 0;
                ///獲得所有doc節點
                XmlNodeList xmlList = doc.GetElementsByTagName("*");

                ///循環遍歷xmlList
                foreach (XmlElement xn in xmlList)
                {
                    ///屬性+1保證唯一
                    index_id++;
                    ///創建索引屬性
                    XmlAttribute xmlAttribute = doc.CreateAttribute("index_id");
                    xmlAttribute.InnerXml = index_id.ToString();
                    ///添加索引屬性
                    xn.Attributes.Append(xmlAttribute);
                }
                ///保存
                doc.Save(baseUrl);
                /// 重新加載xml
                return LoadXmlFolder(baseUrl);
            }
            catch (Exception ex)
            {
                throw new Exception("HandleXmlMgr-->AddIndexAttributes" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 獲得用於預覽顯示的xmlStr
        /// </summary>
        /// <param name="xmlModelList">集合節點</param>
        /// <param name="parentNode">父節點</param>
        /// <param name="code">子節點最終顯示內容</param>
        /// <returns>預覽顯示的xmlStr</returns>
        private string GetXmlStr(List<XmlModelCustom> xmlModelList,string parentNode,string code)
        {
            List<XmlModelCustom> tempList = xmlModelList.FindAll(m => m.parentName == parentNode);
            if (tempList.Count == 0) return "";
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul style=\"list-style-type:none;color:red\">");
            try
            {
                foreach(XmlModelCustom x in tempList)
                {
                    sb.Append("<li>");
                    if (x.attributesXml != "")
                    {
                        sb.AppendFormat("〈{0} {1}〉", x.name, x.attributesXml);
                    }
                    else
                    {
                        sb.AppendFormat("〈{0}{1}〉", x.name, x.attributesXml);
                    }
                    if (x.children.Count == 0)
                    {
                        sb.AppendFormat("{0}", x.code);
                    }
                    else 
                    {
                        sb.AppendFormat("{0}", GetXmlStr(x.children, x.name, x.code));
                    }
                    sb.AppendFormat("〈/{0}〉", x.name);
                    sb.Append("</li>");
                }
                sb.Append("</ul>");
            }
            catch (Exception ex)
            {
                throw new Exception("HandleXmlMgr--->GetXmlStr" + ex.Message, ex);
            }
            return sb.ToString();
        }

        public string GetStrCopy(string fullPath)
        {
            string str = "";
            try
            {
                XmlDocument xmlDoc = LoadXmlFolder(fullPath);
                List<XmlModelCustom> TempList= new List<XmlModelCustom>();
                List<XmlModelCustom> resultXmlList = ToSort(xmlDoc.ChildNodes, TempList);
                ///得到xml格式字符串
                xmlStr = GetXmlStr(resultXmlList, "#document", "");
                return xmlStr;
            }
            catch (Exception ex)
            {
                throw new Exception("HandleXmlMgr-->GetStrCopy" + ex.Message,ex);
            }
        }
    }
}
