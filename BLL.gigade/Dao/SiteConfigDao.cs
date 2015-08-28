/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：SiteConfigDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/9/12 13:43:57 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using System.Xml.Linq;
using BLL.gigade.Dao.Impl;

namespace BLL.gigade.Dao
{
    public class SiteConfigDao:ISiteConfigImplDao
    {
        public string XmpPath { get; set; }
        public SiteConfigDao(string xmlPath)
        {
            XmpPath = xmlPath;
        }

        public List<SiteConfig> Query()
        {
            XDocument xml = XDocument.Load(XmpPath);
            var result = (from x in xml.Elements().Elements()
                          select new
                          {
                              Name = x.Name.LocalName,
                              Remark = x.Attribute("Remark").Value,
                              Value = x.Attribute("Value").Value,
                              DefaultValue = x.Attribute("DefaultValue").Value
                          }).ToList().ConvertAll<SiteConfig>(m => new SiteConfig
                          {
                              Name = m.Name,
                              Remark = m.Remark,
                              Value = m.Value,
                              DefaultValue = m.DefaultValue
                          });

            return result;
        }

        public SiteConfig GetConfigByName(string configName)
        {
            XDocument xml = XDocument.Load(XmpPath);
            var result = (from x in xml.Elements().Descendants(configName)
                          select new
                          {
                              Name = x.Name.LocalName,
                              Remark = x.Attribute("Remark").Value,
                              Value = x.Attribute("Value").Value,
                              DefaultValue = x.Attribute("DefaultValue").Value
                          }).ToList().ConvertAll<SiteConfig>(m => new SiteConfig
                          {
                              Name = m.Name,
                              Remark = m.Remark,
                              Value = m.Value,
                              DefaultValue = m.DefaultValue
                          });
            return result.FirstOrDefault();
        }

        public bool UpdateNode(SiteConfig newConfig)
        {
            XDocument xml = XDocument.Load(XmpPath);
            XElement el = xml.Elements().Descendants(newConfig.Name).FirstOrDefault();
            if (el != null)
            {
                el.SetAttributeValue("Value", newConfig.Value);
                xml.Save(XmpPath);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
