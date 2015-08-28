/* 
 * 武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ParticularsSrcDao 
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
using System.Xml;
using System.Xml.Linq;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    public class ParticularsSrcDao : IParticularsSrcImplDao
    {
        private string xmlPath;
        public ParticularsSrcDao(string xmlPath)
        {
            this.xmlPath = xmlPath;
        }

        #region 獲取 ParticularsSrc.xml 文檔的信息  + GetParticularsSrc()
        /// <summary>
        /// 獲取 ParticularsSrc.xml 文檔的信息
        /// </summary>
        /// <returns></returns>
        public List<ParticularsSrc> GetParticularsSrc()
        {
            try
            {
                XDocument xml = XDocument.Load(xmlPath);
                var result = (from x in xml.Elements().Elements()
                              select new
                              {
                                  ParticularsName = x.Name.LocalName,
                                  particularsValid = x.Attribute("particularsValid").Value,
                                  particularsCollect = x.Attribute("particularsCollect").Value,
                                  particularsCome = x.Attribute("particularsCome").Value,
                                  oldCollect = x.Attribute("oldCollect").Value,
                                  oldCome = x.Attribute("oldCome").Value
                              }).ToList().ConvertAll<ParticularsSrc>(m => new ParticularsSrc
                              {
                                  particularsName = m.ParticularsName,
                                  particularsValid = Convert.ToInt32(m.particularsValid),
                                  particularsCollect = Convert.ToInt32(m.particularsCollect),
                                  particularsCome = Convert.ToInt32(m.particularsCome),
                                  oldCollect = Convert.ToInt32(m.oldCollect),
                                  oldCome = Convert.ToInt32(m.oldCome)
                              });
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("ParticularsSrcDao-->GetParticularsSrc-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 根據 保存期限 添加 或 修改 相關信息  + SaveNode(List<ParticularsSrc> particularsSrc)
        /// <summary>
        /// 根據 保存期限 添加 或 修改 相關信息
        /// </summary>
        /// <param name="particularsSrc"></param>
        /// <returns></returns>
        public bool SaveNode(List<ParticularsSrc> particularsSrc)
        {
            bool result = false;
            try
            {
                if (System.IO.File.Exists(xmlPath))
                {
                    if (particularsSrc.Count == 0)
                        result = true;
                    foreach (var item in particularsSrc)
                    {
                        XDocument xml = XDocument.Load(xmlPath);
                        if (item.particularsName != "null")
                        {
                            XElement el = xml.Elements().Descendants(item.particularsName).FirstOrDefault();
                            if (el != null)
                            {
                                el.SetAttributeValue("particularsValid", item.particularsValid.ToString());
                                el.SetAttributeValue("particularsCollect", item.particularsCollect.ToString());
                                el.SetAttributeValue("particularsCome", item.particularsCome.ToString());
                                el.SetAttributeValue("oldCollect", item.oldCollect.ToString());
                                el.SetAttributeValue("oldCome", item.oldCome.ToString());
                                xml.Save(xmlPath);
                                result = true;
                            }
                        }
                        else
                        {
                            //加载xml文档
                            XmlDocument doc = new XmlDocument();
                            doc.Load(xmlPath);
                            //创建节点
                            string particularsName = "particulars" + item.particularsValid.ToString();
                            XmlElement xmlElement = doc.CreateElement("" + particularsName + "");
                            //添加属性
                            xmlElement.SetAttribute("particularsValid", "" + item.particularsValid.ToString() + "");
                            xmlElement.SetAttribute("particularsCollect", "" + item.particularsCollect.ToString() + "");
                            xmlElement.SetAttribute("particularsCome", "" + item.particularsCome.ToString() + "");
                            xmlElement.SetAttribute("oldCollect", "0");
                            xmlElement.SetAttribute("oldCome", "0");
                            //将节点加入到指定的节点下
                            XmlNode xmlNode = doc.DocumentElement.PrependChild(xmlElement);
                            doc.Save(xmlPath);
                            result = true;
                        }
                    }
                }
                else { result = false; }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("ParticularsSrcDao-->SaveNode-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 根據 保存期限 刪除 相關信息  + DeleteNode(string ParticularsName)
        /// <summary>
        /// 根據 保存期限 刪除 相關信息
        /// </summary>
        /// <param name="incr"></param>
        /// <returns></returns>
        public bool DeleteNode(string ParticularsName)
        {
            bool result = false;
            try
            {
                if (System.IO.File.Exists(xmlPath))
                {
                    char[] CH = { ',' };
                    string[] SS = ParticularsName.Split(CH);
                    for (int i = 0; i < SS.Length; i++)
                    {
                        //表示 XML 文檔
                        XDocument xml = XDocument.Load(xmlPath);
                        XElement xl = xml.Elements().Descendants(SS[i]).FirstOrDefault();
                        if (xl != null)
                        {
                            xl.Remove();
                            xml.Save(xmlPath);
                            result = true;
                        }
                        else { result = false; }
                    }
                }
                else { result = false; }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("ParticularsSrcDao-->DeleteNode-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}