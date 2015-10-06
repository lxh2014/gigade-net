/* 
 * 武漢聯綿信息技術有限公司
 *  
 * 文件名称：ParticularsSrcDao 
 * 摘    要： 
 *  
 * 当前版本：1.0 
 * 作    者：zhuoqin0830w 
 * 完成日期：2015/05/19
 * 
 * 修改日期：2015/09/29
 * 修改原因：將使用 xml 資料庫 改為使用 db 數據庫
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
using DBAccess;

namespace BLL.gigade.Dao
{
    public class ParticularsSrcDao : IParticularsSrcImplDao
    {
        private string connectionString;
        private int particularsSrcType;

        private IDBAccess _dbAccess;

        /// <summary>
        /// 方法
        /// </summary>
        /// <param name="connectionString">連接字符串</param>
        /// <param name="particularsSrcType">particularsSrcType = 1 表示使用 db數據庫  particularsSrcType = 2 表示 使用 xml 資料庫</param>
        public ParticularsSrcDao(string connectionString, int particularsSrcType)
        {
            this.connectionString = connectionString;
            this.particularsSrcType = particularsSrcType;

            if (particularsSrcType == 1)
            {
                _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionString);
            }
        }

        #region 獲取 ParticularsSrc  的信息  + GetParticularsSrc()
        /// <summary>
        /// 獲取 ParticularsSrc  的信息
        /// </summary>
        /// <returns></returns>
        public List<ParticularsSrc> GetParticularsSrc()
        {
            try
            {
                List<ParticularsSrc> result = new List<ParticularsSrc>();

                #region 判斷 是 使用 數據庫 還是 資料庫  2015/09/29
                switch (particularsSrcType)
                {
                    case 1://使用 db數據庫
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append(@"SELECT rowid,parameterType,parameterProperty,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType = 'ParticularsSrc';");
                        result = _dbAccess.getDataTableForObj<ParticularsSrc>(strSql.ToString());
                        break;
                    case 2://使用 xml 資料庫
                        XDocument xml = XDocument.Load(connectionString);
                        result = (from x in xml.Elements().Elements()
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
                        break;
                }
                #endregion

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
                #region 判斷 是 使用 數據庫 還是 資料庫  2015/09/29
                switch (particularsSrcType)
                {
                    case 1://使用 db數據庫
                        StringBuilder strSelect = new StringBuilder();
                        StringBuilder strSql = new StringBuilder();
                        if (particularsSrc.Count == 0)
                            return true;
                        strSql.Append(@"SET sql_safe_updates = 0;");
                        foreach (var item in particularsSrc)
                        {
                            string parameterProperty = "particulars" + item.particularsValid.ToString();
                            //修改
                            if (item.Rowid != 0)
                            {
                                strSelect.AppendFormat(@"SELECT rowid FROM t_parametersrc WHERE parameterType = 'ParticularsSrc' AND parameterProperty = '{0}'  AND rowid <> {1};", parameterProperty, item.Rowid);
                                if (_dbAccess.getDataTable(strSelect.ToString()).Rows.Count > 0) { break; }
                                else
                                {
                                    strSql.AppendFormat(@"UPDATE t_parametersrc SET parameterCode = '{0}' WHERE parameterProperty = '{1}' AND rowid = {2};", item.ParameterCode, item.ParameterProperty, item.Rowid);
                                }
                            }
                            //新增
                            else
                            {
                                strSelect.AppendFormat(@"SELECT rowid,parameterType,parameterProperty,parameterCode,parameterName,remark FROM t_parametersrc WHERE parameterType = 'ParticularsSrc' AND parameterProperty = '{0}';", parameterProperty);
                                if (_dbAccess.getDataTable(strSelect.ToString()).Rows.Count > 0) { break; }
                                else
                                {
                                    strSql.Append(@"INSERT INTO t_parametersrc(parameterType,parameterProperty,parameterCode,parameterName,remark) VALUES");
                                    strSql.AppendFormat(@"('{0}','{1}','{2}','{3}','{4}');", item.ParameterType, parameterProperty, item.ParameterCode, item.ParameterName, item.Remark);
                                }
                            }
                        }
                        strSql.Append(@"SET sql_safe_updates = 1;");
                        result = _dbAccess.execCommand(strSql.ToString()) >= 0;
                        break;
                    case 2://使用 xml 資料庫
                        if (System.IO.File.Exists(connectionString))
                        {
                            if (particularsSrc.Count == 0)
                                result = true;
                            foreach (var item in particularsSrc)
                            {
                                XDocument xml = XDocument.Load(connectionString);
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
                                        xml.Save(connectionString);
                                        result = true;
                                    }
                                }
                                else
                                {
                                    //加载xml文档
                                    XmlDocument doc = new XmlDocument();
                                    doc.Load(connectionString);
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
                                    doc.Save(connectionString);
                                    result = true;
                                }
                            }
                        }
                        else { result = false; }
                        break;
                }
                #endregion

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("ParticularsSrcDao-->SaveNode-->" + ex.Message, ex);
            }
        }
        #endregion

        #region  根據 保存期限 刪除 相關信息  + DeleteNode(string ParticularsName)
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
                #region 判斷 是 使用 數據庫 還是 資料庫
                switch (particularsSrcType)
                {
                    case 1://使用 db數據庫
                        StringBuilder strSql = new StringBuilder();
                        strSql.AppendFormat(@"SET sql_safe_updates = 0;DELETE FROM t_parametersrc WHERE rowid IN({0}) AND parameterType = 'ParticularsSrc';SET sql_safe_updates = 1;", ParticularsName);
                        result = _dbAccess.execCommand(strSql.ToString()) >= 0;
                        break;
                    case 2://使用 xml 資料庫
                        if (System.IO.File.Exists(connectionString))
                        {
                            char[] CH = { ',' };
                            string[] SS = ParticularsName.Split(CH);
                            for (int i = 0; i < SS.Length; i++)
                            {
                                //表示 XML 文檔
                                XDocument xml = XDocument.Load(connectionString);
                                XElement xl = xml.Elements().Descendants(SS[i]).FirstOrDefault();
                                if (xl != null)
                                {
                                    xl.Remove();
                                    xml.Save(connectionString);
                                    result = true;
                                }
                                else { result = false; }
                            }
                        }
                        else { result = false; }
                        break;
                }
                #endregion

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