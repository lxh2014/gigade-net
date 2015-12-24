/*******************************************************************
 * 版权所有： 
 * 类 名 称：MailHelper
 * 作    者：shuangshuang
 * 电子邮箱：shuangshuang0420j@hz-mail.eamc.com.tw
 * 创建日期：2015/2/4 10:32:21 
 * 描述：支持群組代號、編號郵件發送；單用戶郵件發送；多用戶郵件發送
 * 修改描述：
 * 
 * *******************************************************************/

using System;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml.Linq;
using MySql.Data.MySqlClient;


namespace BLL.gigade.Common
{
    public class MailModel
    {
        /// <summary>
        /// 發件人地址
        /// </summary>
        public string MailFromAddress { get; set; }
        /// <summary>
        /// 發件人密碼
        /// </summary>
        public string MailFormPwd { get; set; }
        /// <summary>
        /// 發件人服務器名
        /// </summary>
        public string MailHost { get; set; }
        /// <summary>
        /// 發件人服務器端口號
        /// </summary>
        public string MailPort { get; set; }
        /// <summary>
        /// 發件人名稱
        /// </summary>
        public string MailFromUser { get; set; }
        /// <summary>
        /// 數據庫連接字符串
        /// </summary>
        public string MysqlConnectionString { get; set; }

        public MailModel()
        {
            MailFromAddress = String.Empty;
            MailFormPwd = String.Empty;
            MailHost = String.Empty;
            MailPort = String.Empty;
            MailFromUser = string.Empty;
            MysqlConnectionString = string.Empty;
        }
    }

    public class MailHelper
    {
        private int i = 0;
        #region 屬性

        /// <summary>
        /// 發件人地址
        /// </summary>
        private string MailFromAddress = String.Empty;
        /// <summary>
        /// 發件人密碼
        /// </summary>
        private string MailFormPwd = String.Empty;
        /// <summary>
        /// 發件人服務器名
        /// </summary>
        private string MailHost = String.Empty;
        /// <summary>
        /// 發件人服務器端口號
        /// </summary>
        private string MailPort = String.Empty;
        /// <summary>
        /// 發件人名稱
        /// </summary>
        private string MailFromUser = string.Empty;

        //數據庫連接字符串
        private string MysqlConnectionString;
        //收件人email地址數據集合
        private ArrayList arryMailTo = new ArrayList();
        #endregion

        #region 構造函數
        /// <summary>
        /// 從配置文件中獲取發件人信息
        /// </summary>
        public MailHelper()
        {
            arryMailTo.Clear();
            MysqlConnectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
            string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//郵件服務器的設置
            string path = System.Web.HttpContext.Current.Server.MapPath(xmlPath);
            MailFromAddress = GetConfigByName(path, "Mail_From").ToString();
            MailHost = GetConfigByName(path, "Mail_Host").ToString();
            MailPort = GetConfigByName(path, "Mail_Port").ToString();
            MailFromUser = GetConfigByName(path, "Mail_UserName").ToString();
            MailFormPwd = GetConfigByName(path, "Mail_UserPasswd").ToString();
        }
        /// <summary>
        /// 根據用戶輸入發件人信息
        /// </summary>
        /// <param name="mysqlConnectionString">數據庫連接字符串</param>
        /// <param name="mailFromAddress">發件人地址</param>
        /// <param name="mailFromUser">發件人名稱</param>
        /// <param name="mailFormPwd">發件人郵箱密碼</param>
        /// <param name="mailHost">發件人服務器</param>
        /// <param name="mailPort">發件人端口號</param>
        public MailHelper(string mysqlConnectionString, string mailFromAddress, string mailFromUser, string mailFormPwd, string mailHost, string mailPort = null)
        {
            MysqlConnectionString = mysqlConnectionString;
            MailFromAddress = mailFromAddress;
            MailHost = mailHost;
            MailPort = mailPort;
            MailFromUser = mailFromUser;
            MailFormPwd = mailFromUser;
        }
        /// <summary>
        /// 從MailModel中獲取發件人信息
        /// </summary>
        /// <param name="mailModel">類MailModel定義發件人的信息</param>
        public MailHelper(MailModel mailModel)
        {
            MysqlConnectionString = mailModel.MysqlConnectionString;
            MailFromAddress = mailModel.MailFromAddress;
            MailHost = mailModel.MailHost;
            MailPort = mailModel.MailPort;
            MailFromUser = mailModel.MailFromUser;
            MailFormPwd = mailModel.MailFormPwd;
        }
        #endregion

        #region 發送郵件給gigade用戶群組
        /// <summary>
        /// 發送郵件給用戶群組
        /// </summary>
        /// <param name="GroupID">群組編號</param>
        /// <param name="MailTitle">郵件主題</param>
        /// <param name="MailBody">郵件主體內容</param>
        /// <param name="isSeparate">收件人是否分離發送,默認false</param>
        /// <param name="IsDisplyName">收件人是否顯示名稱,默認false</param>
        /// <returns></returns>
        public Boolean SendToGroup(int GroupID, string MailTitle, string MailBody, Boolean IsSeparate = false, Boolean IsDisplyName = false)
        {

            Boolean result = false;
            try
            {
                //根據群組編號獲取收件人信息
                StringBuilder sql = new StringBuilder();
                sql.Append(@" select mu.user_mail,mu.user_name");
                sql.Append(@" from mail_group mg");
                sql.Append(@" left join mail_group_map mgm on mgm.group_id=mg.row_id ");
                sql.Append(@" left join mail_user mu on mu.row_id=mgm.user_id ");
                sql.AppendFormat(@" where  mg.row_id='{0}'", GroupID);
                sql.Append(" and mg.`status`=1 and mgm.`status`=1  and mu.`status`=1");
                DataTable _dt = GetDataTable(sql.ToString());
                if (_dt.Rows.Count > 0)
                {
                    if (IsSeparate)
                    {
                        if (IsDisplyName)
                        {
                            foreach (DataRow item in _dt.Rows)
                            {
                                SendMailAction(item["user_mail"].ToString(), item["user_name"].ToString(), MailTitle, MailBody);
                            }
                        }
                        else
                        {
                            foreach (DataRow item in _dt.Rows)
                            {
                                SendMailAction(item["user_mail"].ToString(), MailTitle, MailBody);
                            }
                        }
                        result = true;
                    }
                    else
                    {
                        arryMailTo.Clear();
                        if (IsDisplyName)
                        {
                            ArrayList arryMailToUser = new ArrayList();
                            foreach (DataRow item in _dt.Rows)
                            {
                                arryMailTo.Add(item["user_mail"].ToString());
                                arryMailToUser.Add(item["user_name"].ToString());
                            }
                            if (SendMailAction(arryMailTo, arryMailToUser, MailTitle, MailBody))
                            {
                                result = true;
                            }
                        }
                        else
                        {
                            foreach (DataRow item in _dt.Rows)
                            {
                                arryMailTo.Add(item["user_mail"].ToString());
                            }
                            if (SendMailAction(arryMailTo, MailTitle, MailBody))
                            {
                                result = true;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(" Error:MailHelper-->SendToGroup-->" + ex.Message, ex);
            }

            return result;
        }
        /// <summary>
        /// 發送郵件給用戶群組
        /// </summary>
        /// <param name="GroupCode">群組代碼</param>
        /// <param name="MailTitle">郵件主題</param>
        /// <param name="MailBody">郵件主體內容</param>
        /// <param name="isSeparate">收件人是否分離發送,默認false</param>
        /// <param name="IsDisplyName">收件人是否顯示名稱,默認false</param>
        /// <returns></returns>
        public Boolean SendToGroup(string GroupCode, string MailTitle, string MailBody, Boolean IsSeparate = false, Boolean IsDisplyName = false)
        {
            Boolean result = false;
            try
            {
                MailBody = MailBody + " ";
                //根據群組代碼獲取收件人信息
                StringBuilder sql = new StringBuilder();
                sql.Append(@" select mu.user_mail,mu.user_name");
                sql.Append(@" from mail_group mg");
                sql.Append(@" left join mail_group_map mgm on mgm.group_id=mg.row_id ");
                sql.Append(@" left join mail_user mu on mu.row_id=mgm.user_id ");
                sql.AppendFormat(@" where mg.group_code='{0}'", GroupCode);
                sql.Append(" and mg.`status`=1 and mgm.`status`=1  and mu.`status`=1");
                DataTable _dt = GetDataTable(sql.ToString());
                if (_dt.Rows.Count > 0)
                {
                    if (IsSeparate)
                    {
                        if (IsDisplyName)
                        {
                            foreach (DataRow item in _dt.Rows)
                            {
                                SendMailAction(item["user_mail"].ToString(), item["user_name"].ToString(), MailTitle, MailBody);
                            }
                        }
                        else
                        {
                            foreach (DataRow item in _dt.Rows)
                            {
                                SendMailAction(item["user_mail"].ToString(), MailTitle, MailBody);
                            }
                        }
                        result = true;
                    }
                    else
                    {
                        arryMailTo.Clear();
                        if (IsDisplyName)
                        {
                            ArrayList arryMailToUser = new ArrayList();
                            foreach (DataRow item in _dt.Rows)
                            {
                                arryMailTo.Add(item["user_mail"].ToString());
                                arryMailToUser.Add(item["user_name"].ToString());
                            }
                            if (SendMailAction(arryMailTo, arryMailToUser, MailTitle, MailBody))
                            {
                                result = true;
                            }
                        }
                        else
                        {
                            foreach (DataRow item in _dt.Rows)
                            {
                                arryMailTo.Add(item["user_mail"].ToString());
                            }
                            if (SendMailAction(arryMailTo, MailTitle, MailBody))
                            {
                                result = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(" Error:MailHelper-->SendToGroup-->" + ex.Message, ex);
            }

            return result;
        }

        #endregion

        #region 發送郵件給單個用戶
        /// <summary>
        /// 發送郵件給單個用戶
        /// </summary>
        /// <param name="UserID">用戶編號</param>
        /// <param name="MailTitle">郵件主題</param>
        /// <param name="MailBody">郵件主體內容</param>
        /// <param name="IsDisplyName">是否顯示收件人名稱，默認false</param>
        /// <returns></returns>
        public Boolean SendToUser(int UserID, string MailTitle, string MailBody, Boolean IsDisplyName = false)
        {
            Boolean result = false;
            try
            {
                //根據群組代碼獲取收件人信息
                StringBuilder sql = new StringBuilder();
                sql.Append(@" select  row_id,user_mail,user_name,`status` ");
                sql.Append(" from mail_user ");
                sql.AppendFormat(" where row_id={0}", UserID);
                sql.Append(" and `status`=1");
                DataTable _dt = GetDataTable(sql.ToString());

                if (_dt.Rows.Count > 0)
                {
                    if (IsDisplyName)
                    {
                        if (SendMailAction(_dt.Rows[0]["user_mail"].ToString(), _dt.Rows[0]["user_name"].ToString(), MailTitle, MailBody))
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        if (SendMailAction(_dt.Rows[0]["user_mail"].ToString(), MailTitle, MailBody))
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(" Error:MailHelper-->SendToUser-->" + ex.Message, ex);
            }
            return result;
        }

        #endregion

        #region 執行發送動作，不支持群發
        /// <summary>
        /// 執行發送動作，不支持群發
        /// </summary>
        /// <param name="MailToAddress">收件人地址</param>
        /// <param name="MailTitle">郵件主題</param>
        /// <param name="MailBody">郵件主體內容</param>
        /// <returns></returns>
        public Boolean SendMailAction(string MailToAddress, string MailTitle, string MailBody)
        {
            Boolean result = false;
            try
            {
                arryMailTo.Clear();
                arryMailTo.Add(MailToAddress);
                if (SendMailAction(arryMailTo, MailTitle, MailBody))
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(" Error:MailHelper-->SendMailAction-->" + ex.Message, ex);
            }
            return result;
        }
        /// <summary>
        /// 執行發送動作，不支持群發,顯示收件者名稱
        /// </summary>
        /// <param name="MailToAddress">收件人地址</param>
        /// <param name="MailToUserName">收件人名稱</param>
        /// <param name="MailTitle">郵件主題</param>
        /// <param name="MailBody">郵件主體內容</param>
        /// <returns></returns>
        public Boolean SendMailAction(string MailToAddress, string MailToUserName, string MailTitle, string MailBody)
        {
            Boolean result = false;
            try
            {
                arryMailTo.Clear();
                arryMailTo.Add(MailToAddress);
                ArrayList arryMailToUser = new ArrayList();
                arryMailToUser.Add(MailToUserName);
                if (SendMailAction(arryMailTo, arryMailToUser, MailTitle, MailBody))
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(" Error:MailHelper-->SendMailAction-->" + ex.Message, ex);
            }
            return result;
        }
        #endregion

        #region 執行發送動作，支持群發
        /// <summary>
        /// 執行發送動作，支持群發
        /// </summary>
        /// <param name="MailToAddress">收件人地址集</param>
        /// <param name="MailToUserName">和收件人地址一一對應的收件人名稱集</param>
        /// <param name="MailTitle">郵件主題</param>
        /// <param name="MailBody">郵件主體內容</param>
        /// <returns></returns>
        public Boolean SendMailAction(ArrayList MailToAddress, ArrayList MailToUserName, string MailTitle, string MailBody)
        {
            Boolean result = false;
            try
            {
                MailMessage objMail = new MailMessage();
                //發送人地址（發送郵件時顯示發件人名稱）
                objMail.From = new MailAddress(MailFromAddress, MailFromUser);
                //郵件主題
                objMail.Subject = MailTitle;
                //郵件主題編碼方式
                objMail.SubjectEncoding = System.Text.Encoding.UTF8;
                //郵件主體內容
                objMail.Body = MailBody;
                //郵件主體內容支持html編碼
                objMail.IsBodyHtml = true;
                //郵件主體編碼方式
                objMail.BodyEncoding = System.Text.Encoding.UTF8;
                //郵件收件人列表
                if (MailToAddress.Count != MailToUserName.Count)
                {
                    for (int i = 0; i < MailToAddress.Count; i++)
                    {
                        objMail.To.Add(MailToAddress[i].ToString());
                    }
                }
                else
                {
                    for (int i = 0; i < MailToAddress.Count; i++)
                    {
                        objMail.To.Add(new MailAddress(MailToAddress[i].ToString(), MailToUserName[i].ToString()));
                    }
                }

                //使用SMTP方式發送郵件
                SmtpClient client = new SmtpClient();
                //驗證發件人身份憑證
                client.Credentials = new System.Net.NetworkCredential(MailFromAddress, MailFormPwd);
                client.Host = MailHost;
                if (!string.IsNullOrEmpty(MailPort))
                {
                    client.Port = Convert.ToInt32(MailPort);
                }
                client.Send(objMail);
                result = true;
            }
            catch (Exception ex)
            {
                throw new Exception(" Error:MailHelper-->SendMailAction-->" + ex.Message, ex);
            }
            return result;
        }

        /// <summary>
        /// 執行發送動作，支持群發
        /// </summary>
        /// <param name="MailToAddress">收件人地址集</param>
        /// <param name="MailTitle">郵件主題</param>
        /// <param name="MailBody">郵件主體內容</param>
        /// <returns></returns>
        public Boolean SendMailAction(ArrayList MailToAddress, string MailTitle, string MailBody)
        {
            Boolean result = false;
            try
            {
                MailMessage objMail = new MailMessage();
                //發送人地址（發送郵件時顯示發件人名稱）
                objMail.From = new MailAddress(MailFromAddress, MailFromUser);
                //郵件主題
                objMail.Subject = MailTitle;
                //郵件主題編碼方式
                objMail.SubjectEncoding = System.Text.Encoding.UTF8;
                //郵件主體內容
                objMail.Body = MailBody;
                //郵件主體內容支持html編碼
                objMail.IsBodyHtml = true;
                //郵件主體編碼方式
                objMail.BodyEncoding = System.Text.Encoding.UTF8;
                //郵件收件人列表

                foreach (string item in MailToAddress)
                {
                    objMail.To.Add(item);
                }
                //使用SMTP方式發送郵件
                SmtpClient client = new SmtpClient();
                //驗證發件人身份憑證
                client.Credentials = new System.Net.NetworkCredential(MailFromAddress, MailFormPwd);
                client.Host = MailHost;
                if (!string.IsNullOrEmpty(MailPort))
                {
                    client.Port = Convert.ToInt32(MailPort);
                }
                client.Send(objMail);
                result = true;
            }
            catch (Exception ex)
            {
                throw new Exception(" Error:MailHelper-->SendMailAction-->" + ex.Message, ex);
            }
            return result;
        }
        #endregion

        #region 执行查询操作返回datatable类型
        /// <summary>
        /// 执行查询操作返回datatable类型
        /// </summary>
        /// <param name="sql">sql語句</param>
        /// <returns></returns>
        private DataTable GetDataTable(string sql)
        {
            MySqlConnection conn = new MySqlConnection(MysqlConnectionString);
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }
            return ds.Tables[0];
        }
        #endregion

        #region 獲取config.xml中的設置的值
        /// <summary>
        /// 獲取config.xml中的設置的值
        /// </summary>
        /// <param name="XmpPath"></param>
        /// <param name="configName"></param>
        /// <returns></returns>
        private string GetConfigByName(string XmpPath, string configName)
        {
            XDocument xml = XDocument.Load(XmpPath);
            var result = (from x in xml.Elements().Descendants(configName)
                          select new
                          {
                              Name = x.Name.LocalName,
                              Value = x.Attribute("Value").Value,
                          }).ToList();
            return result.FirstOrDefault().Value;
        }
        #endregion

        #region 郵件排成專用(添加寄件人方法)
        /// <summary>
        /// 執行發送動作，不支持群發(帶寄件者)
        /// </summary>
        /// <param name="MailToAddress">收件人地址</param>
        /// <param name="MailTitle">郵件主題</param>
        /// <param name="MailBody">郵件主體內容</param>
        /// <param name="SendAddress">寄件人地址</param>
        /// <param name="SendName">寄件人名稱</param>
        /// <returns></returns>
        public Boolean SendMailAction(string MailToAddress, string MailTitle, string MailBody, string SendAddress, string SendName)
        {
            Boolean result = false;
            try
            {
                arryMailTo.Clear();
                arryMailTo.Add(MailToAddress);
                if (SendMailAction(arryMailTo, MailTitle, MailBody, SendAddress, SendName))
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(" Error:MailHelper-->SendMailAction-->" + ex.Message, ex);
            }
            return result;
        }
        /// <summary>
        /// 執行發送動作，不支持群發(帶寄件者)
        /// </summary>
        /// <param name="MailToAddress">收件人地址</param>
        /// <param name="MailTitle">郵件主題</param>
        /// <param name="MailBody">郵件主體內容</param>
        /// <param name="SendAddress">寄件人地址</param>
        /// <param name="SendName">寄件人名稱</param>
        /// <returns></returns>
        public Boolean SendMailAction(ArrayList MailToAddress, string MailTitle, string MailBody, string SendAddress, string SendName)
        {
            Boolean result = false;
            try
            {
                MailMessage objMail = new MailMessage();
                //發送人地址（發送郵件時顯示發件人名稱）
                objMail.From = new MailAddress(SendAddress, SendName);
                //郵件主題
                objMail.Subject = MailTitle;
                //郵件主題編碼方式
                objMail.SubjectEncoding = System.Text.Encoding.UTF8;
                //郵件主體內容
                objMail.Body = MailBody;
                //郵件主體內容支持html編碼
                objMail.IsBodyHtml = true;
                //郵件主體編碼方式
                objMail.BodyEncoding = System.Text.Encoding.UTF8;
                //郵件收件人列表

                foreach (string item in MailToAddress)
                {
                    objMail.To.Add(item);
                }
                //使用SMTP方式發送郵件
                SmtpClient client = new SmtpClient();
                //驗證發件人身份憑證
                client.Credentials = new System.Net.NetworkCredential(MailFromAddress, MailFormPwd);
                if (MailHost.Split(',').Length > 0)
                {//MailHost
                    string[] host = MailHost.Split(',');
                    client.Host = host[i];
                    if (i + 1 >= MailHost.Split(',').Length)
                    {
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }
                }
                else
                {
                    client.Host = MailHost;
                }
                //client.Host = MailHost;
                if (!string.IsNullOrEmpty(MailPort))
                {
                    client.Port = Convert.ToInt32(MailPort);
                }
                client.Send(objMail);
                result = true;
            }
            catch (Exception ex)
            {
                throw new Exception(" Error:MailHelper-->SendMailAction-->" + ex.Message, ex);
            }
            return result;
        }
        #endregion
    }
}
