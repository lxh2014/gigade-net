using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using System.Text.RegularExpressions;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using BLL.gigade.Common;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using System.IO;
using System.Text;
using System.Data;
namespace Admin.gigade.Controllers
{
    public class ProductConsultController : Controller
    {
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//郵件服務器的設置
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        string FromName = ConfigurationManager.AppSettings["FromName"];//發件人姓名
        string EmailTile = ConfigurationManager.AppSettings["EmailTile"];//郵件標題

        private IProductConsultImplMgr _productconsultMgr;
        //
        // GET: /ProductConsult/

        public ActionResult Index()
        {
            return View();
        }

        #region 商品諮詢列表頁+HttpResponseBase GetProductConsultList()
        /// <summary>
        /// 商品諮詢列表頁
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase GetProductConsultList()
        {
            List<ProductConsultQuery> store = new List<ProductConsultQuery>();
            string json = string.Empty;
            string str = string.Empty;
            try
            {
                ProductConsultQuery query = new ProductConsultQuery();
                _productconsultMgr = new ProductConsultMgr(mySqlConnectionString);
                query.Start = Convert.ToInt32(Request["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request["Limit"]))
                {
                    query.Limit = Convert.ToInt32(Request["Limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ddlSel"]))
                {
                    query.answer_status = Convert.ToInt32(Request.Params["ddlSel"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["huifu"]))
                {
                    query.huifu = Convert.ToInt32(Request.Params["huifu"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["productName"]))
                {
                    query.product_name = Request.Params["productName"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["productId"]))
                {
                    query.product_id = Convert.ToUInt32(Request.Params["productId"]);
                }
                if (Convert.ToBoolean(Request.Params["consultType1"]))
                {
                    query.consultType += "1,";
                }
                if (Convert.ToBoolean(Request.Params["consultType2"]))
                {
                    query.consultType += "2,";
                }
                if (Convert.ToBoolean(Request.Params["consultType3"]))
                {
                    query.consultType += "3,";
                }
                if (Convert.ToBoolean(Request.Params["consultType4"]))
                {
                    query.consultType += "4,";
                }
                if (Convert.ToBoolean(Request.Params["consultType5"]))
                {
                    query.consultType += "5,";
                }
                if (query.consultType != "")
                {
                    query.consultType = query.consultType.Remove(query.consultType.LastIndexOf(","));
                }
                if (!string.IsNullOrEmpty(Request.Params["userName"]))
                {
                    query.user_name = Request.Params["userName"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["userEmail"]))
                {
                    query.user_email = Request.Params["userEmail"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["timestart"]))
                {
                    query.beginTime = Convert.ToDateTime(Convert.ToDateTime(Request.Params["timestart"]).ToString("yyyy-MM-dd 00:00:00")) ;
                }
                if (!string.IsNullOrEmpty(Request.Params["timeend"]))
                {
                    query.endTime = Convert.ToDateTime(Convert.ToDateTime(Request.Params["timeend"]).ToString("yyyy-MM-dd 23:59:59"));
                }
                if (Convert.ToBoolean(Request.Params["shopClass1"]) && Convert.ToBoolean(Request.Params["shopClass2"]))
                {

                }
                else
                {
                    if (Convert.ToBoolean(Request.Params["shopClass1"]))
                    {

                        query.productIds = "10";//食品館
                    }
                    if (Convert.ToBoolean(Request.Params["shopClass2"]))
                    {

                        query.productIds = "20";//用品館
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["relation_id"]))//待回覆
                {
                    query.consult_id = Convert.ToInt32(Request.Params["relation_id"]);
                }
                int totalCount = 0;
                store = _productconsultMgr.Query(query, out totalCount);
                if (Convert.ToBoolean(Request.Params["isSecret"]))
                {
                    foreach (var item in store)
                    {
                        if (!string.IsNullOrEmpty(item.user_name))
                        {
                            item.user_name = item.user_name.Substring(0, 1) + "**";
                        }
                        item.user_email = item.user_email.Split('@')[0] + "@***";
                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {

                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region  發送群組郵件 + HttpResponseBase SendMailByGroup()
        /// <summary>
        ///發送群組郵件
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase SendMailByGroup()
        {
            string path = Server.MapPath(xmlPath);
            SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
            SiteConfig Food_Product_Consult = _siteConfigMgr.GetConfigByName("Food_Product_Consult");
            SiteConfig Use_Product_Consult = _siteConfigMgr.GetConfigByName("Use_Product_Consult");
            SiteConfig Other_Product_Consult = _siteConfigMgr.GetConfigByName("Other_Product_Consult");
            string jsonStr = String.Empty;
            ProductConsultQuery query = new ProductConsultQuery();
            _productconsultMgr = new ProductConsultMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request["zixunType"]))
                {
                    query.consult_type = Convert.ToInt32(Request["zixunType"]);
                }
                switch (query.consult_type)
                {
                    case 1:
                        query.consultType = "商品諮詢";
                        break;
                    case 2:
                        query.consultType = "庫存及配送";
                        break;
                    case 3:
                        query.consultType = "支付問題";
                        break;
                    case 4:
                        query.consultType = "發票及保修";
                        break;
                    case 5:
                        query.consultType = "促銷及贈品";
                        break;
                }
                if (!string.IsNullOrEmpty(Request.Params["ckShopClass"]))
                {
                    query.ckShopClass = Request.Params["ckShopClass"].ToString();
                }
                if (!string.IsNullOrEmpty(Request["consult_id"]))
                {
                    query.consult_id = Convert.ToInt32(Request["consult_id"]);
                }
                if (!string.IsNullOrEmpty(Request["consult_info"]))
                {
                    query.consult_info = Request["consult_info"].ToString();
                }
                if (!string.IsNullOrEmpty(Request["user_name"]))
                {
                    query.user_name = Request["user_name"].ToString();
                }
                if (!string.IsNullOrEmpty(Request["create_date"]))
                {
                    query.create_date = Convert.ToDateTime(Request["create_date"]);
                }
                FileStream fs = new FileStream(Server.MapPath("../ImportUserIOExcel/productConsultKeFu.html"), FileMode.OpenOrCreate, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                string strTemp = sr.ReadToEnd();
                sr.Close();
                fs.Close();
                strTemp = strTemp.Replace("{{$s_username$}}", query.user_name);
                strTemp = strTemp.Replace("{{$s_datetime$}}", query.create_date.ToString());
                strTemp = strTemp.Replace("{{$consultInfo$}}", query.consult_info);
                strTemp = strTemp.Replace("{{$consultType$}}", query.consultType);
                if (query.ckShopClass == "ckShopClass1" && query.consult_type == 1)//食品館商品諮詢
                {
                    query.group_code = Food_Product_Consult.Value.ToString();
                }
                if (query.ckShopClass == "ckShopClass2" && query.consult_type == 1)//用品館商品諮詢
                {
                    query.group_code = Use_Product_Consult.Value.ToString();
                }
                if (query.consult_type != 1)//其他諮詢類型問題
                {
                    query.group_code = Other_Product_Consult.Value.ToString();
                }
                int i = _productconsultMgr.UpdateConsultType(query);
                if (i > 0)
                {
                    DataTable dt = _productconsultMgr.QueryMailGroup(query);
                    MailHelper mHelper = new MailHelper();
                    if (dt.Rows.Count > 0)
                    {
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            if (!string.IsNullOrEmpty(dt.Rows[j]["row_id"].ToString()))
                            {
                                mHelper.SendToGroup(Convert.ToInt32(dt.Rows[j]["row_id"]), "吉甲地市集【商品諮詢】通知信", strTemp.ToString(), true, true);
                            }
                        }
                    }
                }
                jsonStr = "{success:true }";
            }
            catch (Exception ex)
            {

                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false}";
            }


            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;

        }
        #endregion

        #region 更改活動使用狀態 + HttpResponseBase UpdateActive()
        /// <summary>
        /// 更改活動使用狀態
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UpdateActive()
        {
            string jsonStr = string.Empty;
            try
            {
                ProductConsultQuery model = new ProductConsultQuery();
                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    model.status = Convert.ToInt32(Request.Params["active"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    model.consult_id = Convert.ToInt32(Request.Params["id"]);
                }
                _productconsultMgr = new ProductConsultMgr(mySqlConnectionString);
                if (_productconsultMgr.UpdateActive(model) > 0)
                {
                    return Json(new { success = "true" });
                }
                else
                {
                    return Json(new { success = "false" });
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = "false" });
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SaveProductConsultAnswer()
        {
            string path = Server.MapPath(xmlPath);
            SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
            SiteConfig Mail_From = _siteConfigMgr.GetConfigByName("Mail_From");
            SiteConfig Mail_Host = _siteConfigMgr.GetConfigByName("Mail_Host");
            SiteConfig Mail_Port = _siteConfigMgr.GetConfigByName("Mail_Port");
            SiteConfig Mail_UserName = _siteConfigMgr.GetConfigByName("Mail_UserName");
            SiteConfig Mail_UserPasswd = _siteConfigMgr.GetConfigByName("Mail_UserPasswd");
            string EmailFrom = Mail_From.Value;//發件人郵箱
            string SmtpHost = Mail_Host.Value;//smtp服务器
            string SmtpPort = Mail_Port.Value;//smtp服务器端口
            string EmailUserName = Mail_UserName.Value;//郵箱登陸名
            string EmailPassWord = Mail_UserPasswd.Value;//郵箱登陸密碼
            string json = string.Empty;
            string jsonStr = string.Empty;
            ProductConsultQuery query = new ProductConsultQuery();
            IAreaPactetImplMgr _iareaPacketMgr = new AreaPacketMgr(mySqlConnectionString);
            _productconsultMgr = new ProductConsultMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["consult_id"]))
                {
                    query.consult_id = Convert.ToInt32(Request.Params["consult_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["answer_status"]))
                {
                    if (Convert.ToInt32(Request.Params["answer_status"]) == 2)
                    {
                        query.answer_status = 2;
                        query.answer_user = ((Caller)Session["caller"]).user_id;
                        query.delay_reason = Request.Params["delay_reason"].ToString();
                        _productconsultMgr.UpdateAnswerStatus(query);

                        json = "{success:true}";
                    }
                    
                    else
                    {
                        query.answer_status = 3;
                        if (!string.IsNullOrEmpty(Request.Params["status"]))
                        {
                            query.status = Convert.ToInt32(Request.Params["status"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["is_sendEmail"]))
                        {
                            query.is_sendEmail = Convert.ToInt32(Request.Params["is_sendEmail"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["consult_answer"]))
                        {
                            query.consult_answer = Request.Params["consult_answer"].ToString();
                        }
                        query.answer_date = DateTime.Now;
                        query.answer_user = ((Caller)Session["caller"]).user_id;
                        int i = _productconsultMgr.SaveProductConsultAnswer(query);
                        if (!string.IsNullOrEmpty(Request.Params["is_sendEmail"]))
                        {
                            int j = Convert.ToInt32(Request.Params["is_sendEmail"]);
                            if (j == 1 && i > 0)
                            {
                                string userName = string.Empty;
                                string product_name = string.Empty;
                                string consultInfo = string.Empty;
                                string answerInfo = string.Empty;
                                string consultUrl = string.Empty;
                                string productUrl = string.Empty;
                                string userEmail = string.Empty;
                                if (!string.IsNullOrEmpty(Request.Params["user_name"]))
                                {
                                    userName = Request.Params["user_name"].ToString();
                                }
                                if (!string.IsNullOrEmpty(Request.Params["user_email"]))
                                {
                                    userEmail = Request.Params["user_email"].ToString();
                                }
                                if (!string.IsNullOrEmpty(Request.Params["consult_info"]))
                                {
                                    consultInfo = Request.Params["consult_info"].ToString();
                                }
                                if (!string.IsNullOrEmpty(Request.Params["consult_answer"]))
                                {
                                    answerInfo = Request.Params["consult_answer"].ToString();
                                }
                                if (!string.IsNullOrEmpty(Request.Params["product_url"]))
                                {
                                    productUrl = Request.Params["product_url"].ToString();
                                }
                                if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                                {
                                    query.product_id = Convert.ToUInt32(Request.Params["product_id"]);
                                }
                                if (!string.IsNullOrEmpty(Request.Params["consult_url"]))
                                {
                                    consultUrl = Request.Params["consult_url"].ToString();
                                }
                                if (!string.IsNullOrEmpty(Request.Params["item_id"]))
                                {
                                    query.item_id = Convert.ToInt32(Request.Params["item_id"].ToString());
                                }

                                ProductConsultQuery queryTemp = _productconsultMgr.GetProductInfo(query);
                                FileStream fs = new FileStream(Server.MapPath("../ImportUserIOExcel/productConsultAnwser.html"), FileMode.OpenOrCreate, FileAccess.Read);
                                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                                string strTemp = sr.ReadToEnd();
                                sr.Close();
                                fs.Close();
                                //if (queryTemp.spec_name1 == "" && queryTemp.spec_name2 == "")
                                //{
                                //    product_name = queryTemp.brand_name + queryTemp.product_name;
                                //}
                                //else
                                //{
                                //    product_name = queryTemp.brand_name + queryTemp.product_name + "(" + queryTemp.spec_name1 + "  " + queryTemp.spec_name2 + ")";
                                //}
                                product_name = queryTemp.product_name;
                                strTemp = strTemp.Replace("{{$username$}}", userName);
                                strTemp = strTemp.Replace("{{$productName$}}", product_name);
                                strTemp = strTemp.Replace("{{$consultInfo$}}", consultInfo);
                                strTemp = strTemp.Replace("{{$consultAnwser$}}", answerInfo);
                                strTemp = strTemp.Replace("{{$productUrl$}}", productUrl);
                                sendmail(EmailFrom, FromName, userEmail, userName, EmailTile, strTemp, "", SmtpHost, Convert.ToInt32(SmtpPort), EmailUserName, EmailPassWord);
                            }
                        }

                        if (i > 0)
                        {
                            json = "{success:true}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        #region C#发送邮件函数
        /// <summary>
        /// C#发送邮件函数
        /// </summary>
        /// <param name="from">发送者邮箱</param>
        /// <param name="fromer">发送人</param>
        /// <param name="sto">接受者邮箱</param>
        /// <param name="toer">收件人</param>
        /// <param name="Subject">主题</param>
        /// <param name="Body">内容</param>
        /// <param name="file">附件地址</param>
        /// <param name="SMTPHost">smtp服务器</param>
        /// <param name="SMTPuser">邮箱</param>
        /// <param name="SMTPpass">密码</param>
        /// <returns></returns>
        public bool sendmail(string sfrom, string sfromer, string sto, string stoer, string sSubject, string sBody, string sfile, string sSMTPHost, int sSMTPPort, string sSMTPuser, string sSMTPpass)
        {
            ////设置from和to地址
            MailAddress from = new MailAddress(sfrom, sfromer);
            MailAddress to = new MailAddress(sto, stoer);

            ////创建一个MailMessage对象
            MailMessage oMail = new MailMessage(from, to);
            //// 添加附件
            if (sfile != "")
            {
                oMail.Attachments.Add(new Attachment(sfile));
            }
            ////邮件标题
            oMail.Subject = sSubject;
            ////邮件内容
            //oMail.Body = sBody;
            AlternateView htmlBody = AlternateView.CreateAlternateViewFromString(sBody, null, "text/html");
            oMail.AlternateViews.Add(htmlBody);
            ////邮件格式
            oMail.IsBodyHtml = true;
            ////邮件采用的编码
            oMail.BodyEncoding = System.Text.Encoding.GetEncoding("GB2312");
            ////设置邮件的优先级为高
            oMail.Priority = MailPriority.Normal;
            ////发送邮件
            SmtpClient client = new SmtpClient();
            ////client.UseDefaultCredentials = false;
            client.Host = sSMTPHost;
            client.Port = sSMTPPort;
            client.Credentials = new NetworkCredential(sSMTPuser, sSMTPpass);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                client.Send(oMail);
                return true;
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return false;
            }
            finally
            {
                ////释放资源
                oMail.Dispose();
            }

        }
        #endregion
    }
}
