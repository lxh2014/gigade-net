/*
 jialei0706h 
 datatime:20140922
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model.Query;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using System.Data;
using System.IO;
using System.Web;
using System.Xml;
using System.Configuration;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
    public class UserRecommendDao : IUserRecommendIDao
    {
        private IDBAccess _access;
        private string connStr;
        #region IUserRecommendIDao 成员
        public UserRecommendDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }

        public List<UserRecommendQuery> QueryAll(UserRecommendQuery query, out int totalCount)
        {
            totalCount = 0;
            StringBuilder str = new StringBuilder();
            StringBuilder strall = new StringBuilder();
            StringBuilder strcounts = new StringBuilder();
            List<UserRecommendQuery> userRecommendQueryList = null;
            try
            {
                userRecommendQueryList = new List<UserRecommendQuery>();
                strcounts.AppendFormat("select count(ur.id) as totalcounts from user_recommend  ur ");
                strall.AppendFormat("SELECT  ur.id,ur.user_id,ur.`name`,ur.recommend_user_id,u.user_name as usname,u1.user_name,ur.mail,ur.createtime,u1.user_email,u1.user_password,u1.user_gender,concat(u1.user_birthday_year,'/',u1.user_birthday_month,'/',u1.user_birthday_day) as birthday,u1.user_zip,u1.user_address,u1.user_mobile,u1.user_phone,u1.user_id as suser_id,CASE u1.user_type  when '1' THEN '網路會員' else'電話會員' END as mytype,u1.user_gender,u1.user_gender,u1.adm_note,u1.send_sms_ad,u1.paper_invoice  FROM user_recommend ur ");
                str.AppendFormat(" left join users u ON u.user_id = ur.recommend_user_id ");
                str.AppendFormat(" left join users u1 ON u1.user_id = ur.user_id ");
                str.AppendFormat(" where 1=1");
                if (query.recommend_user_id != 0)
                    str.AppendFormat(" and ur.user_id <> 0 ");
                if (query.con != "" && query.con != null)
                {
                    switch (query.ddlstore)
                    {
                        case 1:
                            str.AppendFormat(" and ur.mail like '%" + query.con + "%' ");
                            break;
                        case 2:
                            str.AppendFormat(" and ur.`name` like '%" + query.con + "%' ");
                            break;
                        case 3:
                            str.AppendFormat(" and u.user_name like '%" + query.con + "%' ");
                            break;
                        case 4:
                            str.AppendFormat(" and u.user_email like '%" + query.con + "%' ");
                            break;
                        default:
                            break;
                    }
                }
                if (query.startdate != DateTime.Parse("2010/01/01"))
                    str.AppendFormat(" and createtime >= '" + query.startdate.ToString("yyyy-MM-dd 00:00:00") + "' ");
                else
                    str.AppendFormat(" and createtime >= '2010-01-01' ");
                if (query.enddate != DateTime.Parse("2010/01/01"))
                    str.AppendFormat(" and createtime <= '" + query.enddate.ToString("yyyy-MM-dd 23:59:59") + "' ");
                else
                    str.AppendFormat(" and createtime <= '" + DateTime.Now + "' ");
                totalCount = 0;
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(strcounts.ToString() + str.ToString());

                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalcounts"]);
                    }
                    //order by ur.id DESC
                    str.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                }
                userRecommendQueryList = _access.getDataTableForObj<UserRecommendQuery>(strall.ToString() + str.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UserRecommendDao-->QueryAll-->" + ex.Message + "sql:" + strall.ToString() + str.ToString(), ex);
            }
            return userRecommendQueryList;
        }

        /// <summary>
        /// 獲取推薦者的信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable getUserInfo(int id)
        {
            StringBuilder str = new StringBuilder();
            StringBuilder strall = new StringBuilder();

            try
            {

                strall.AppendFormat("SELECT  ul.user_name,ul.user_email,ur.`name` as name,ur.mail as mail");

                str.Append(" FROM user_recommend ur");
                str.AppendFormat(" left join users ul ON ul.user_id = ur.recommend_user_id ");//推薦人
                str.AppendFormat(" where 1=1");
                if (id != 0)
                {
                    str.AppendFormat(" and ur.id = '{0}' ", id);
                }

                strall.Append(str.ToString());
                return _access.getDataTable(strall.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UserRecommendDao-->getUserInfo-->" + ex.Message + "sql:" + strall.ToString(), ex);
            }

        }


        public List<UserRecommend> QueryByOrderId(uint order_id)
        {
            StringBuilder sql = new StringBuilder();
     
            try
            {
                sql.AppendFormat(@"select id from user_recommend where order_id = '{0}';", order_id);
                return _access.getDataTableForObj<UserRecommend>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UserRecommendDao.QueryByOrderId -->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string UpdateIsCommend(string idStr)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"update user_recommend set is_recommend = 0,updatetime='{0}' where id in ({1});" ,CommonFunction.DateTimeToString(DateTime.Now) ,idStr);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("UserRecommendDao.UpdateIsCommend -->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        private void WriterInfo(string functionName, string startRunInfo, string endRunInfo)
        {
            XmlDocument xmldoc;
            XmlElement xmlelem;
            try
            {
                string timeStr = DateTime.Now.ToString("yyyy-MM-dd");
                //string path = Server.MapPath("~/XML/Exceptional.xml");

                string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//郵件服務器的設置
                string path = System.Web.HttpContext.Current.Server.MapPath(xmlPath);
                xmldoc = new XmlDocument();
                if (System.IO.File.Exists(path))
                {

                    xmldoc.Load(path);
                    XmlNode root = xmldoc.SelectSingleNode("InformationRecord");//查找<InformationRecord>
                    XmlElement xe1 = xmldoc.CreateElement(functionName);//创建一个傳值進來的方法名稱节点
                    xe1.SetAttribute("date", timeStr);//设置该节点genre属性
                    XmlElement xesub1 = xmldoc.CreateElement("info");
                    xesub1.InnerText = startRunInfo;//设置文本节点
                    XmlElement xesub2 = xmldoc.CreateElement("info");
                    xesub2.InnerText = endRunInfo;
                    xe1.AppendChild(xesub1);//添加到<Node>节点中
                    xe1.AppendChild(xesub2);//添加到<Node>节点中
                    root.AppendChild(xe1);//添加到<Employees>节点中
                    if (root.ChildNodes.Count >= 2)
                    {
                        XmlElement xe;
                        XmlElement xe2;
                        xe = (XmlElement)root.ChildNodes[0];
                        DateTime dt = Convert.ToDateTime(xe.GetAttribute("date"));
                        xe2 = (XmlElement)root.ChildNodes[1];
                        DateTime dt2 = Convert.ToDateTime(xe2.GetAttribute("date"));
                        TimeSpan ts = DateTime.Now - dt;
                        TimeSpan ts2 = DateTime.Now - dt2;
                        if (ts.Days > 30)
                        {
                            root.RemoveChild(root.ChildNodes[0]);
                        }
                        if (ts2.Days > 30)
                        {
                            root.RemoveChild(root.ChildNodes[0]);
                        }
                    }
                    //xmldoc.Save(Server.MapPath("~/XML/Exceptional.xml"));//保存创建好的XML文档
                }
                else
                {
                    XmlDeclaration xmldecl;
                    xmldecl = xmldoc.CreateXmlDeclaration("1.0", "gb2312", null);
                    xmldoc.AppendChild(xmldecl);
                    xmlelem = xmldoc.CreateElement("", "InformationRecord", "");
                    xmldoc.AppendChild(xmlelem);//加入另外一个元素
                    xmldoc.Save(path);//保存创建好的XML文档
                    System.IO.File.SetAttributes(path, System.IO.FileAttributes.Normal);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("UserRecommendDao.UpdateIsCommend -->" + ex.Message, ex);
            }
        }

    }
}
