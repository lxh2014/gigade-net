/*
* 文件名稱 :ChannelController.cs
* 文件功能描述 :外網諮詢表
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/19
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using System.Configuration;
using Admin.gigade.CustomError;

namespace Admin.gigade.Controllers
{
    [HandleError]
    public class ChannelController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string connectString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private ChannelMgr chMgr;
        private ZipMgr zMgr;
        private UsersMgr uMgr;
        private ChannelContactMgr chcMgr;
        private ChannelShippingMgr chsMgr;
        private ParameterMgr paraMgr;
        Channel ch = new Channel();
        ChannelContact chc = new ChannelContact();
        ChannelShipping chs = new ChannelShipping();
        Zip z = new Zip();
        Users u = new Users();
        //
        // GET: /ChannelList/

        #region View
        
        //外站新增
        public ActionResult Index()
        {
            if (Request.QueryString["id"] != null) {
                ViewBag.channel_id = Request.QueryString["id"].ToString();
            }
            return View();
        }

        //外站列表
        public ActionResult ChannelList()
        {
            return View();
        }

        //外站新增Tab1->基本資料
        public ActionResult Channel()
        {
            return View();
        }

        //外站新增Tab2->聯絡人
        public ActionResult ChannelContact()
        {
            return View();
        }

        //外站新增Tab3->運費設定
        public ActionResult ChannelShipping()
        {
            return View();
        }

        //外站新增Tab4->其它資訊
        public ActionResult ChannelOther()
        {
            return View();
        }

        #endregion

        [CustomHandleError]
        public string Query()
        {
            ch.Replace4MySQL();
            chMgr = new ChannelMgr(connectString);

            string json = string.Empty;
            string strSel = string.Empty;
            int startPage = Convert.ToInt32(Request.Form["start"] ?? "0");
            int endPage = Convert.ToInt32(Request.Form["limit"] ?? "20");

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["txtSel"]))
                {
                    if (Request.Form["ddlSel"].ToString() == "full")
                    {
                        ch.channel_name_full = Request.Form["txtSel"].Trim();
                        strSel += " and channel_name_full like '%" + ch.channel_name_full + "%'";
                    }
                    else if (Request.Form["ddlSel"].ToString() == "simple")
                    {
                        ch.channel_name_simple = Request.Form["txtSel"].Trim();
                        strSel += " and channel_name_simple like '%" + ch.channel_name_simple + "%'";
                    }
                }

                if (!string.IsNullOrEmpty(Request.Form["cobStatus"]))
                {
                    if (Request.Form["cobStatus"].ToString() != "0")
                    {
                        ch.channel_status = Int32.Parse(Request.Form["cobStatus"].ToString());
                        strSel += " and channel_status = '" + ch.channel_status + "'";
                    }
                }
                
                json = chMgr.DataPager(strSel,startPage,endPage);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "[]";
            }
            return json;
        }

        [CustomHandleError]
        public string QueryChannelById()
        {
            chMgr = new ChannelMgr(connectString);

            string json = string.Empty;
            string strSel = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["channel_id"]))
                {
                    strSel = "and channel_id = '" + Int32.Parse(Request.Form["channel_id"].ToString()) + "'";

                    json = chMgr.Query(strSel);
                }
                else
                {
                    json = chMgr.Query();
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "[]";
            }

            return json;
        }

        [CustomHandleError]
        public string QueryCity()
        {
            zMgr = new ZipMgr(connectString);

            string json = string.Empty;

            try
            {
                json = zMgr.QueryMiddle(Request.Form["topValue"]??"");
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "[]";
            }

            return json;
        }

        [CustomHandleError]
        public string QueryZip()
        {
            zMgr = new ZipMgr(connectString);

            string json = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["topValue"]))
                {
                    json = zMgr.QuerySmall(Request.Form["topValue"].ToString(), Request.Form["topText"].ToString());
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "[]";
            }

            return json;
        }

        [CustomHandleError]
        public string QueryContact()
        {
            chcMgr = new ChannelContactMgr(connectString);

            string json = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["channel_id"]))
                {
                    json = chcMgr.Query(Request.QueryString["channel_id"].ToString());
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "[]";
            }

            return json;
        }

        [CustomHandleError]
        public string QueryShipping()
        {
            chsMgr = new ChannelShippingMgr(connectString);

            string json = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["channel_id"]))
                {
                    json = chsMgr.Query(Request.QueryString["channel_id"].ToString());
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "[]";
            }

            return json;
        }

        [CustomHandleError]
        public string QueryOther()
        {
            chMgr = new ChannelMgr(connectString);

            string json = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["channel_id"]))
                {
                    json = chMgr.QueryOther(Int32.Parse(Request.Form["channel_id"].ToString()));
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "[]";
            }

            return json;
        }

        [CustomHandleError]
        public string QueryDeliver()
        {
            paraMgr = new ParameterMgr(connectString);
            string json = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["paraType"]))
                {
                    json = paraMgr.Query(Request.QueryString["paraType"].ToString());
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "[]";
            }

            return json;
        }

        #region 保存外站
        
        [CustomHandleError]
        public HttpResponseBase SaveChannel()
        {
            zMgr = new ZipMgr(connectString);
            chMgr = new ChannelMgr(connectString);
            uMgr = new UsersMgr(connectString);

            string json = string.Empty;
            bool result = false;
            string channel_id = Request.Form["channel_id"] ?? string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["channel_status"]))
                {
                    ch.channel_status = Int32.Parse(Request.Form["channel_status"].ToString());
                }

                if (!string.IsNullOrEmpty(Request.Form["channel_type"]))
                {
                    ch.channel_type = Int32.Parse(Request.Form["channel_type"].ToString());
                }

                if (!string.IsNullOrEmpty(Request.Form["channel_name_full"]))
                {
                    ch.channel_name_full = Request.Form["channel_name_full"].ToString();
                    u.user_name = Request.Form["channel_name_full"].ToString();
                    List<Channel> channel = chMgr.QueryC(" and channel_name_full='" + Request.Form["channel_name_full"] + "'");
                    if (channel != null && channel.Count > 0 && channel.Where(m => m.channel_id == int.Parse(string.IsNullOrEmpty(channel_id) ? "0" : channel_id)).Count() == 0)
                    {
                        json = "{success:true,msg:\""+Resources.Channel.CHANNEL_HAS_EXIST+"。\",channelId:\"" + channel_id + "\"}";

                        this.Response.Clear();
                        this.Response.Write(json);
                        this.Response.End();
                        return this.Response;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Form["channel_name_simple"]))
                {
                    ch.channel_name_simple = Request.Form["channel_name_simple"].ToString();
                }

                if (!string.IsNullOrEmpty(Request.Form["channel_invoice"]))
                {
                    ch.channel_invoice = Request.Form["channel_invoice"].ToString();
                    u.user_password = Request.Form["channel_invoice"].ToString();
                }

                if (!string.IsNullOrEmpty(Request.Form["channel_email"]))
                {
                    ch.channel_email = Request.Form["channel_email"].ToString();
                }
                else
                {
                    ch.channel_email = "";
                }

                if (!string.IsNullOrEmpty(Request.Form["company_cphonenum"]))
                {
                    ch.company_phone = Request.Form["company_cphone"].ToString() +Request.Form["company_cphonenum"].ToString();
                }
                else
                {
                    ch.company_phone = "";
                }

                if (!string.IsNullOrEmpty(Request.Form["company_cfaxnum"]))
                {
                    ch.company_fax = Request.Form["company_cfax"].ToString() +Request.Form["company_cfaxnum"].ToString();
                }
                else
                {
                    ch.company_fax = "";
                }

                if (!string.IsNullOrEmpty(Request.Form["company_zip"]))
                {
                    ch.company_zip = Int32.Parse(Request.Form["company_zip"].ToString());
                }
                else
                {
                    ch.company_zip = 0;
                }

                if (!string.IsNullOrEmpty(Request.Form["company_address"]))
                {
                    ch.company_address = Request.Form["company_address"].ToString();
                }
                else
                {
                    ch.company_address = "";
                }

                if (!string.IsNullOrEmpty(Request.Form["invoice_title"]))
                {
                    ch.invoice_title = Request.Form["invoice_title"].ToString();
                }

                if (!string.IsNullOrEmpty(Request.Form["invoice_zip"]))
                {
                    ch.invoice_zip = Int32.Parse(Request.Form["invoice_zip"].ToString());
                }

                if (!string.IsNullOrEmpty(Request.Form["invoice_address"]))
                {
                    ch.invoice_address = Request.Form["invoice_address"].ToString();
                }

                if (!string.IsNullOrEmpty(Request.Form["contract_createdate"]))
                {
                    ch.contract_createdate = DateTime.Parse(Request.Form["contract_createdate"].ToString());
                }
                else
                {
                    ch.contract_createdate = DateTime.MinValue;
                }

                if (!string.IsNullOrEmpty(Request.Form["contract_start"]))
                {
                    ch.contract_start = DateTime.Parse(Request.Form["contract_start"].ToString());
                }
                else
                {
                    ch.contract_start = DateTime.MinValue;
                }

                if (!string.IsNullOrEmpty(Request.Form["contract_end"]))
                {
                    ch.contract_end = DateTime.Parse(Request.Form["contract_end"].ToString());
                }
                else
                {
                    ch.contract_end = DateTime.MinValue;
                }

                if (!string.IsNullOrEmpty(Request.Form["annaul_fee"]))
                {
                    ch.annaul_fee = Decimal.Parse(Request.Form["annaul_fee"].ToString());
                }
                else
                {
                    ch.annaul_fee = 0;
                }

                if (!string.IsNullOrEmpty(Request.Form["renew_fee"]))
                {
                    ch.renew_fee = Decimal.Parse(Request.Form["renew_fee"].ToString());
                }
                else
                {
                    ch.renew_fee = 0;
                }

                if (!string.IsNullOrEmpty(Request.Form["model_in"]))
                {
                    ch.model_in = Request.Form["model_in"].ToString();
                }
                else
                {
                    ch.model_in = "1";
                }

                if (!string.IsNullOrEmpty(Request.Form["notify_sms"]))
                {
                    ch.notify_sms = Int32.Parse(Request.Form["notify_sms"].ToString());
                }
                else
                {
                    ch.notify_sms = 0;
                }

                //add by xiangwang0413w 2014/06/26 增加 ERP客戶代號
                if (!string.IsNullOrEmpty(Request.Form["erp_id"]))
                {
                    ch.erp_id = Request.Form["erp_id"];
                }

                DataTable dtUsers = uMgr.Query(Request.Form["manager_Email"].ToString());
                if (dtUsers.Rows.Count > 0)//會員信箱已存在
                {
                    DataTable dtUserId = chMgr.QueryUser(dtUsers.Rows[0]["user_id"].ToString());
                    if (string.IsNullOrEmpty(channel_id))//新增
                    {                        
                        if (dtUserId.Rows.Count > 0)//是否被外站使用
                        {
                            json = "{success:true,msg:\""+Resources.Channel.MAIL_WAS_USED+"。\",channelId:\"" + channel_id + "\"}";

                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                        else
                        {
                            ch.user_id = Int32.Parse(dtUsers.Rows[0]["user_id"].ToString());
                            channel_id = chMgr.Save(ch).ToString();
                            result = Int32.Parse(channel_id) > 0;
                        }
                    }                    
                    else
                    {
                        //修改
                        if (dtUserId.Rows.Count > 0)
                        {
                            if (dtUserId.Rows[0]["channel_id"].ToString() != channel_id)
                            {
                                json = "{success:true,msg:\"" + Resources.Channel.MAIL_WAS_USED + "。\",channelId:\"" + channel_id + "\"}";

                                this.Response.Clear();
                                this.Response.Write(json);
                                this.Response.End();
                                return this.Response;
                            }
                            else
                            {
                                ch.user_id = Int32.Parse(dtUsers.Rows[0]["user_id"].ToString());
                            }
                        }
                        else
                        {
                            ch.user_id = Int32.Parse(dtUsers.Rows[0]["user_id"].ToString());
                        }

                        ch.channel_id = Int32.Parse(channel_id);

                        result = chMgr.Edit(ch) > 0;
                    }
                }
                else
                {
                    //會員信箱不存在
                    u.user_email = Request.Form["manager_Email"].ToString();
                    u.user_name = Request.Form["channel_name_full"].ToString();

                    BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                    u.user_password = hash.SHA256Encrypt(Request.Form["invoice_title"].ToString());

                    u.user_mobile = "0227833183";
                    u.user_phone = "0227833183";
                    u.user_zip = 115;
                    u.user_address = Resources.Channel.USER_ADDRESS;
                    u.user_status = 1;
                    u.user_reg_date = (int)BLL.gigade.Common.CommonFunction.GetPHPTime();
                    u.user_updatedate = (int)BLL.gigade.Common.CommonFunction.GetPHPTime();
                    u.user_birthday_year = 1900;
                    u.user_birthday_month = 01;
                    u.user_birthday_day = 01;

                    if (uMgr.SelSaveID(u) > 0)
                    {
                        ch.user_id = Convert.ToInt32(u.user_id);
                    }
                    if (string.IsNullOrEmpty(channel_id))
                    {
                        channel_id=chMgr.Save(ch).ToString();
                        /**
                         *如果外站類型是gigade,則將參數表中type=deliver_store(即所有物流方式)全部插入channel_shipping表中
                         ***/
                        if (ch.channel_type == 2)
                        {
                            paraMgr = new ParameterMgr(connectString);
                            chsMgr = new ChannelShippingMgr(connectString);
                            List<Parametersrc> Deliver_result = paraMgr.QueryUsed(new Parametersrc { ParameterType = "deliver_store", Used = 1 });
                            foreach (Parametersrc item in Deliver_result)
                            {
                                chs.channel_id = int.Parse(channel_id);
                                chs.shipping_carrior = int.Parse(item.ParameterCode);
                                chs.shipco = item.parameterName;
                                chsMgr.Save(chs);
                            }

                        }
                        /*
                         end by zhongyu0304w at 2013/10/10
                         */
                        result = Int32.Parse(channel_id) > 0;
                    }
                    else
                    {
                        ch.channel_id = int.Parse(channel_id);
                        result = chMgr.Edit(ch) > 0;
                    }
                }

                
                if (result)
                {
                    
                    json = "{success:true,msg:\""+Resources.Channel.SUCCESS+"。\",channelId:\"" + channel_id + "\"}";
                }
                else
                {
                    json = "{success:true,msg:\""+Resources.Channel.FAILED+"\",channelId:\"" + channel_id + "\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,msg:\"" + Resources.Channel.FAILED + "\",channelId:\"" + channel_id + "\"}";
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 保存外站聯繫人

        [CustomHandleError]
        public HttpResponseBase SaveChannelContact()
        {
            chcMgr = new ChannelContactMgr(connectString);

            string json = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["InsertValues"]))
                {
                    string[] Values = Request.Form["InsertValues"].ToString().Split(';');
                    for (int n = 0; n < Values.Length-1; n++)
                    {
                        string[] Val = Values[n].Split(',');

                        chc.contact_type = Val[1].ToString();
                        chc.contact_name = Val[2].ToString();
                        chc.contact_phone1 = Val[3].ToString();
                        chc.contact_phone2 = Val[4].ToString();
                        chc.contact_mobile = Val[5].ToString();
                        chc.contact_email = Val[6].ToString();

                        if (!string.IsNullOrEmpty(Val[0].ToString()))
                        {
                            chc.rid = Int32.Parse(Val[0].ToString());
                            chcMgr.Edit(chc);
                        }
                        else
                        {
                            chc.channel_id = Int32.Parse(Request.Form["channel_id"].ToString());
                            chcMgr.Save(chc);
                        }
                    }
                    json = "{success:true,msg:\"" + Resources.Channel.SUCCESS + "。\"}";
                }
                else
                {
                    json = "{success:true,msg:\"" + Resources.Channel.NO_OPERSTION_DATA + "。\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,msg:\"" + Resources.Channel.FAILED + "\"}";
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 保存外站物流
        
        [CustomHandleError]
        public HttpResponseBase SaveChannelShipping()
        {
            chsMgr = new ChannelShippingMgr(connectString);

            string json = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["InsertValues"]) && !string.IsNullOrEmpty(Request.Form["channel_id"]))
                {
                    string[] Values = Request.Form["InsertValues"].ToString().Split(';');
                    for (int n = 0; n < Values.Length - 1; n++)
                    {
                        string[] Val = Values[n].Split(',');
                        if (!string.IsNullOrEmpty(Val[1].ToString()))
                            chs.shipping_carrior = Int32.Parse(Val[1].ToString());
                        if (!string.IsNullOrEmpty(Val[2].ToString()))
                            chs.n_threshold = Int32.Parse(Val[2].ToString());
                        if (!string.IsNullOrEmpty(Val[3].ToString()))
                            chs.l_threshold = Int32.Parse(Val[3].ToString());
                        
                        if (!string.IsNullOrEmpty(Val[4].ToString()))
                            chs.n_fee = Int32.Parse(Val[4].ToString());
                        if (!string.IsNullOrEmpty(Val[5].ToString()))
                            chs.l_fee = Int32.Parse(Val[5].ToString());
                        if (!string.IsNullOrEmpty(Val[6].ToString()))
                            chs.n_return_fee = Int32.Parse(Val[6].ToString());
                        if (!string.IsNullOrEmpty(Val[7].ToString()))
                            chs.l_return_fee = Int32.Parse(Val[7].ToString());
                        if (!string.IsNullOrEmpty(Val[10].ToString()))
                            chs.shipco = Val[10].ToString();
                        if (!string.IsNullOrEmpty(Val[9].ToString()))
                            chs.retrieve_mode = Int32.Parse(Val[9].ToString());
                        chs.createdate = DateTime.Now;
                        chs.updatedate = DateTime.Now;


                        chs.channel_id = Int32.Parse(Request.Form["channel_id"].ToString());
                        List<ChannelShipping> result = chsMgr.Query(chs);
                        if (!string.IsNullOrEmpty(Val[0].ToString()) /*&& !string.IsNullOrEmpty(Val[1].ToString())*/)
                        {
                            int shippingCarrior = Int32.Parse(Val[0].ToString());
                            //int shippingType = Int32.Parse(Val[1]);
                            string shipco_content = Val[8].ToString();
                            chs.updatedate = DateTime.Now;
                            if (Val[0].ToString() != Val[1].ToString() || Val[10].ToString() != Val[8].ToString())//判斷是否對主鍵進行了修改
                            {
                                List<ChannelShipping> resultEdit = chsMgr.Query(chs);   //若修改就去比對數據庫
                                if (resultEdit.Count > 0) //修改重複
                                {
                                    json = "{success:true,msg:\"" + Resources.Channel.RECORD_REPEAT + "。\"}";
                                    this.Response.Clear();
                                    this.Response.Write(json);
                                    this.Response.End();
                                    return this.Response;
                                }
                            }
                            chsMgr.Edit(chs, shippingCarrior, shipco_content);
                        }
                        else
                        {
                            if (result.Count > 0)
                            {
                                json = "{success:true,msg:\"" + Resources.Channel.RECORD_REPEAT + "。\"}";
                                this.Response.Clear();
                                this.Response.Write(json);
                                this.Response.End();
                                return this.Response;
                            }

                            chsMgr.Save(chs);
                        }
                    }
                    json = "{success:true,msg:\"" + Resources.Channel.SUCCESS + "。\"}";
                }
                else
                {
                    json = "{success:true,msg:\"" + Resources.Channel.FAILED + "\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,msg:\"" + Resources.Channel.FAILED + "\"}";
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 保存外站其他信息
        
        [CustomHandleError]
        public HttpResponseBase SaveChannelOther()
        {
            chMgr = new ChannelMgr(connectString);

            string json = string.Empty;
            int num = 0;

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["channel_id"]))
                {
                    ch.channel_id =Int32.Parse(Request.Form["channel_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["deal_method"]))
                {
                    ch.deal_method = Int32.Parse(Request.Form["deal_method"].ToString());

                    if (Request.Form["deal_method"].ToString() == "2")
                    {
                        ch.deal_percent = float.Parse(Request.Form["deal_percent"].ToString());
                        ch.deal_fee = 0;
                    }
                    else if (Request.Form["deal_method"].ToString() == "3")
                    {
                        ch.deal_percent = 0;
                        ch.deal_fee = Int32.Parse(Request.Form["deal_fee"].ToString());
                    }
                    else
                    {
                        ch.deal_percent = 0;
                        ch.deal_fee = 0;
                    }
                }
                else
                {
                    ch.deal_method = 0;
                    ch.deal_percent = 0;
                    ch.deal_fee = 0;
                }

                if (!string.IsNullOrEmpty(Request.Form["creditcard_1_percent"]))
                {
                    ch.creditcard_1_percent = float.Parse(Request.Form["creditcard_1_percent"].ToString());
                }
                else
                {
                    ch.creditcard_1_percent = 0;
                }

                if (!string.IsNullOrEmpty(Request.Form["creditcard_3_percent"]))
                {
                    ch.creditcard_3_percent = float.Parse(Request.Form["creditcard_3_percent"].ToString());
                }
                else
                {
                    ch.creditcard_3_percent = 0;
                }

                if (!string.IsNullOrEmpty(Request.Form["shopping_car_percent"]))
                {
                    ch.shopping_car_percent = float.Parse(Request.Form["shopping_car_percent"].ToString());
                }
                else
                {
                    ch.shopping_car_percent = 0;
                }

                if (!string.IsNullOrEmpty(Request.Form["commission_percent"]))
                {
                    ch.commission_percent = float.Parse(Request.Form["commission_percent"].ToString());
                }
                else
                {
                    ch.commission_percent = 0;
                }

                if (!string.IsNullOrEmpty(Request.Form["cost_by_percent"]))
                {
                    ch.cost_by_percent = Int32.Parse(Request.Form["cost_by_percent"].ToString());
                }
                else
                {
                    ch.cost_by_percent = 0;
                }

                if (!string.IsNullOrEmpty(Request.Form["cost_low_percent"]))
                {
                    ch.cost_low_percent = float.Parse(Request.Form["cost_low_percent"].ToString());
                }
                else
                {
                    ch.cost_low_percent = 0;
                }

                if (!string.IsNullOrEmpty(Request.Form["cost_normal_percent"]))
                {
                    ch.cost_normal_percent = float.Parse(Request.Form["cost_normal_percent"].ToString());
                }
                else
                {
                    ch.cost_normal_percent = 0;
                }

                if (!string.IsNullOrEmpty(Request.Form["invoice_checkout_day"]))
                {
                    ch.invoice_checkout_day = Int32.Parse(Request.Form["invoice_checkout_day"].ToString());
                }
                else
                {
                    ch.invoice_checkout_day = 0;
                }

                if (!string.IsNullOrEmpty(Request.Form["invoice_apply_start"]))
                {
                    ch.invoice_apply_start = Int32.Parse(Request.Form["invoice_apply_start"].ToString());
                }
                else
                {
                    ch.invoice_apply_start = 0;
                }

                if (!string.IsNullOrEmpty(Request.Form["invoice_apply_end"]))
                {
                    ch.invoice_apply_end = Int32.Parse(Request.Form["invoice_apply_end"].ToString());
                }
                else
                {
                    ch.invoice_apply_end = 0;
                }

                if (!string.IsNullOrEmpty(Request.Form["checkout_note"]))
                {
                    ch.checkout_note = Request.Form["checkout_note"].ToString();
                }
                else
                {
                    ch.checkout_note = "";
                }

                if (!string.IsNullOrEmpty(Request.Form["receipt_to"]))
                {
                    ch.receipt_to = Int32.Parse(Request.Form["receipt_to"].ToString());
                }
                else
                {
                    ch.receipt_to = 0;
                }

                if (!string.IsNullOrEmpty(Request.Form["channel_manager"]))
                {
                    ch.channel_manager = Request.Form["channel_manager"].ToString();
                }
                else
                {
                    ch.channel_manager = "";
                }

                if (!string.IsNullOrEmpty(Request.Form["channel_note"]))
                {
                    ch.channel_note = Request.Form["channel_note"].ToString();
                }
                else
                {
                    ch.channel_note = "";
                }

                num = chMgr.SaveOther(ch);

                if (num > 0)
                {
                    json = "{success:true,msg:\"" + Resources.Channel.SUCCESS + "。\",channelId:" + num.ToString() + "}";
                }
                else
                {
                    json = "{success:true,msg:\"" + Resources.Channel.FAILED + "\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,msg:\"" + Resources.Channel.FAILED + "\"}";
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 刪除外站聯繫人
        
        [CustomHandleError]
        public HttpResponseBase DeleteContact()
        {
            chcMgr = new ChannelContactMgr(connectString);
            string json = string.Empty;
            int num = 0;
            int rid = 0;

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["rid"]))
                {
                    rid = Int32.Parse(Request.Form["rid"].ToString());
                }
                num = chcMgr.Delete(rid);

                if (num > 0)
                {
                    json = "{success:true,msg:\"" + Resources.Channel.SUCCESS + "。\",channelId:" + num.ToString() + "}";
                }
                else
                {
                    json = "{success:true,msg:\"" + Resources.Channel.FAILED + "\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,msg:\"" + Resources.Channel.FAILED + "\"}";
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 刪除外站物流
       
        [CustomHandleError]
        public HttpResponseBase DeleteShipping()
        {
            chsMgr = new ChannelShippingMgr(connectString);

            string json = string.Empty;
            int num = 0;
            int channelid = 0;
            int shippingcarrior = 0;
            //int shippingtype = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["channel_id"]) && !string.IsNullOrEmpty(Request.Form["shippingcarrior"]) /*&& !string.IsNullOrEmpty(Request.Form["shippingtype"])*/)
                {
                    channelid = Int32.Parse(Request.Form["channel_id"].ToString());
                    shippingcarrior = Int32.Parse(Request.Form["shippingcarrior"].ToString());
                    //shippingtype = Int32.Parse(Request.Form["shippingtype"].ToString());
                }
                num = chsMgr.Delete(channelid, shippingcarrior);

                if (num > 0)
                {
                    json = "{success:true,msg:\"\",channelId:" + num.ToString() + "}";
                }
                else
                {
                    json = "{success:true,msg:\"\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,msg:\"" + Resources.Channel.FAILED + "\"}";
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
    }    
}
