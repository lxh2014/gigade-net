using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Net;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Net.Mail;
using System.Collections;

namespace SendMailService
{
    public partial class MailService : ServiceBase
    {
        private double SendTime = Convert.ToDouble(ConfigurationSettings.AppSettings["SendTime"].ToString());
        private System.ComponentModel.IContainer components = null;
        private bool servicePaused;//服務停止
        private System.Timers.Timer time;//計時器
        private XmlHelper _xmlHelper = new XmlHelper();
        private static string LogFile = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Template";

        public MailService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 進程入口處
        /// </summary>
        static void Main()
        {
            ServiceBase[] myservice = new ServiceBase[] { new MailService() };
            ServiceBase.Run(myservice);
        }

        /// <summary>
        /// 開始
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            if (servicePaused == false)
            {
                time = new System.Timers.Timer(SendTime);
                time.Enabled = true;
                time.Elapsed += this.timeout;
                time.Start();
            }
        }

        /// <summary>
        /// 你要加入定時器的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timeout(object sender, EventArgs e)
        {
            try
            {
                string dt = DateTime.Now.ToString("HH:mm");
                string UrlPath = ConfigurationSettings.AppSettings["UrlName"];// +funName[i]; //ConfigurationSettings.AppSettings["UrlName"] + funName[i];
                #region 獲取請求信息
                string xmlString = "";
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(UrlPath);
                httpRequest.Method = "GET";
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                Stream stream = httpResponse.GetResponseStream();
                using (StreamReader sr = new StreamReader(stream, Encoding.GetEncoding("UTF-8")))
                {
                    xmlString += sr.ReadToEnd();
                }

                //讀取xml文件
                XmlDocument document = new XmlDocument();
                document.LoadXml(xmlString);
                XmlNode node = _xmlHelper.GetXmlElemenetByName(document, "root", true);//可能有多個mail節點

                //日誌文件的狀態
                string logInfo = string.Empty;
                for (int i = 0, j = node.ChildNodes.Count; i < j; i++)
                {
                    XmlNode mailnode = node.ChildNodes[i];
                    int Status = int.Parse(_xmlHelper.GetXmlElemenetByName(mailnode, "status", true).InnerText);//郵件的狀態
                    if (Status == 1)
                    {
                        string Nowdt = dt;
                        string[] mailto = _xmlHelper.GetXmlElemenetByName(mailnode, "mailto", true).InnerText.Split(',');//發給誰
                        string[] sendTime = _xmlHelper.GetXmlElemenetByName(mailnode, "sendTime", true).InnerText.Split(',');//郵件定時發送的時間
                        string HttpPath = _xmlHelper.GetXmlElemenetByName(mailnode, "getHttp", true).InnerText + _xmlHelper.GetXmlElemenetByName(mailnode, "HttpPath", true).InnerText;//如果狀態ok就獲取內容
                        int logCount = 0;
                        for (int T = 0, L = sendTime.Length; T < L; T++)
                        {
                            if (Nowdt == Convert.ToDateTime(sendTime[T].ToString()).ToString("HH:mm"))
                            {
                                string MailFrom = string.Empty;//說發的
                                string MailSubject = string.Empty;//郵件的標題
                                string MailBody = string.Empty;//郵件的內容
                                string MailHost = string.Empty;//郵件的服務
                                string MailUserName = string.Empty;//登錄帳號
                                string MailUserpasswd = string.Empty;//登錄密碼
                                int MailPort = 0;//郵件端口
                                string xmlStr = "";
                                HttpWebRequest hpRequest = (HttpWebRequest)WebRequest.Create(HttpPath);
                                hpRequest.Method = "GET";
                                HttpWebResponse hpResponse = (HttpWebResponse)hpRequest.GetResponse();
                                Stream filestream = hpResponse.GetResponseStream();
                                using (StreamReader sr = new StreamReader(filestream, Encoding.GetEncoding("UTF-8")))
                                {
                                    xmlStr += sr.ReadToEnd();
                                }
                                //讀取xml文件
                                XmlDocument documentXml = new XmlDocument();
                                document.LoadXml(xmlStr);
                                XmlNode nodexml = _xmlHelper.GetXmlElemenetByName(document, "mail", true);
                                string style = _xmlHelper.GetXmlElemenetByName(nodexml, "status", true).InnerText;
                                if (style != "2")//等於2為無資料 ,為空說明是正常
                                {
                                    MailFrom = _xmlHelper.GetXmlElemenetByName(nodexml, "mailfrom", true).InnerText;
                                    MailSubject = _xmlHelper.GetXmlElemenetByName(nodexml, "mailsubject", true).InnerText;
                                    MailBody = _xmlHelper.GetXmlElemenetByName(nodexml, "mailbody", true).InnerText;
                                    MailHost = _xmlHelper.GetXmlElemenetByName(nodexml, "mailhost", true).InnerText;
                                    MailPort = Convert.ToInt32(_xmlHelper.GetXmlElemenetByName(nodexml, "mailport", true).InnerText);
                                    MailUserName = _xmlHelper.GetXmlElemenetByName(nodexml, "mailusername", true).InnerText;
                                    MailUserpasswd = _xmlHelper.GetXmlElemenetByName(nodexml, "mailuserpasswd", true).InnerText;
                                    MailMessage mail = new MailMessage();
                                    mail.Subject = MailSubject.ToString();
                                    mail.From = new MailAddress(MailFrom);
                                    for (int l = 0, k = mailto.Length; l < k; l++)
                                    {
                                        mail.To.Add(new MailAddress(mailto[l]));
                                    }

                                    //"商品狀態:" + Status + "</br>發給誰:" + MailTo + "誰發的郵件:" + MailFrom + "郵件內容為:" + 
                                    mail.Body = MailBody;
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;
                                    mail.IsBodyHtml = true;
                                    mail.Priority = MailPriority.High;
                                    logCount = 1;
                                    //SmtpClient sc = new SmtpClient("192.168.26.1");
                                    SmtpClient sc = new SmtpClient(MailHost, MailPort);
                                    sc.EnableSsl = false;
                                    sc.UseDefaultCredentials = false;
                                    sc.Credentials = new System.Net.NetworkCredential(MailUserName, MailUserpasswd);//通行
                                    sc.DeliveryMethod = SmtpDeliveryMethod.Network;//遞送方法為 網絡
                                    sc.Send(mail);
                                    logInfo = "SUCCESS";
                                }
                                else
                                {
                                    logInfo = "No Data";
                                }
                                break;//發過一次就不需要發了.
                            }
                        }
                        if (logCount == 0)
                        {
                            logInfo = "NO TIME";
                        }
                    }
                    else if (Status == 3)
                    {
                        logInfo = "Server ERROR";

                    }
                    //大於0就log信息
                    if (Status > 0)
                    {
                        Log(_xmlHelper.GetXmlElemenetByName(mailnode, "HttpPath", true).InnerText, logInfo);
                    }
                }
                #endregion 獲取請求信息
            }
            catch (Exception ex)
            {
                Log(ex.Message.ToString(), "ERROR");
            }
        }

        /// <summary>
        /// log服務信息
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="status">發送的狀態</param>
        protected void Log(string message, string status)
        {
            if (!Directory.Exists(LogFile))
            {
                Directory.CreateDirectory(LogFile);
            }
            string NewLogFile = LogFile + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";//用日期區分
            FileInfo fi = new FileInfo(NewLogFile);
            if (!File.Exists(NewLogFile))
            {
                fi.Create().Close();
            }
            FileStream fs = new FileStream(NewLogFile, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            try
            {
                sw.WriteLine("發送時間:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                sw.WriteLine("狀態:" + status);
                sw.WriteLine("提示信息:" + message);
                sw.WriteLine("");
            }
            catch
            {

            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        /// <summary>
        /// 結束
        /// </summary>
        protected override void OnStop()
        {
            servicePaused = true;
        }

        /// <summary>
        /// 暫停
        /// </summary>
        protected override void OnPause()
        {
            servicePaused = true;
        }

        /// <summary>
        /// 回覆服務
        /// </summary>
        protected override void OnContinue()
        {
            servicePaused = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.ServiceName = "SendMailService";
            this.CanPauseAndContinue = true;
            this.CanStop = true;
            servicePaused = false;
        }
    }
}
