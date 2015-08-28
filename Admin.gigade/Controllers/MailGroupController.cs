using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Admin.gigade.Controllers
{
    public class MailGroupController : Controller
    {
        // 
        // GET: /MailGroup/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        string vendorServerPath = ConfigurationManager.AppSettings["vendorServerPath"];
        private IMailGroupImplMgr _IMailGroupMgr;
        IMailUserImplMgr _IMailUserMgr;
        IManageUserImplMgr _IManageUserMgr;
        #region view


        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 用戶管理視圖
        /// </summary>
        /// <returns></returns>
        public ActionResult MailUser()
        {
            return View();
        }
        #endregion

        #region 郵件群組管理之用戶管理
        /// <summary>
        /// 用戶管理列表頁
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase MailUserList()
        {
            string jsonStr = string.Empty;

            try
            {
                List<MailUserQuery> MailUserStore = new List<MailUserQuery>();
                MailUserQuery query = new MailUserQuery();
                if (!string.IsNullOrEmpty(Request.Params["user_name"]))
                {
                    query.user_name = Request.Params["user_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["user_mail"]))
                {
                    query.user_mail = Request.Params["user_mail"];
                }

                if (!string.IsNullOrEmpty(Request.Params["relation_id"]))//待回覆
                {
                    query.row_id = Convert.ToInt32(Request.Params["relation_id"]);
                }
                List<ManageUserQuery> ManageUseStore = new List<ManageUserQuery>();
                ManageUserQuery ManageUserQuery = new ManageUserQuery();
                ManageUserQuery.IsPage = false;
                #region 查詢管理人員綁定到表中
                DataTable _dtManageUser = new DataTable();
                _dtManageUser.Columns.Add("user_id", typeof(String));
                _dtManageUser.Columns.Add("user_name", typeof(String));
                int total = 0;
                _IManageUserMgr = new ManageUserMgr(mySqlConnectionString);
                ManageUseStore = _IManageUserMgr.GetNameMail(ManageUserQuery, out total);
                foreach (var item in ManageUseStore)
                {
                    DataRow dr = _dtManageUser.NewRow();
                    dr[0] = item.user_id;
                    dr[1] = item.user_name;
                    _dtManageUser.Rows.Add(dr);
                }
                #endregion


                if (!string.IsNullOrEmpty(Request.Params["pagers"]))
                {
                    if (Convert.ToInt32(Request.Params["pagers"]) == 0)
                    {
                        query.IsPage = false;
                    }
                }
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                _IMailUserMgr = new MailUserMgr(mySqlConnectionString);

                int totalCount = 0;
                MailUserStore = _IMailUserMgr.GetMailUserStore(query, out totalCount);//查询出供應商出貨單
                foreach (var item in MailUserStore)
                {
                    item.nameemail = item.user_name + "(" + item.user_mail + ")";
                    DataRow[] rows = _dtManageUser.Select("user_id='" + item.create_user + "'");
                    foreach (DataRow row in rows)//篩選出的最多只有一條數據，
                    {
                        item.create_user_name = item.create_user.ToString();
                        if (!string.IsNullOrEmpty(row["user_id"].ToString()))
                        {
                            item.create_user_name = row["user_name"].ToString();//---創建人
                        }
                    }
                    rows = _dtManageUser.Select("user_id='" + item.update_user + "'");
                    foreach (DataRow row in rows)//篩選出的最多只有一條數據，
                    {
                        item.update_user_name = item.update_user.ToString();
                        if (!string.IsNullOrEmpty(row["user_id"].ToString()))
                        {
                            item.update_user_name = row["user_name"].ToString();//---修改人
                        }
                    }
                    if (Convert.ToBoolean(Request.Params["isSecret"]))
                    {

                        if (!string.IsNullOrEmpty(item.user_name))
                        {
                            item.user_name = item.user_name.Substring(0, 1) + "**";
                        }
                        item.user_mail = item.user_mail.Split('@')[0] + "@***";

                        if (item.nameemail.ToString().Length > 3)
                        {
                            item.nameemail = item.nameemail.Substring(0, 3) + "***";
                        }
                        else
                        {
                            item.nameemail = item.nameemail + "***";
                        }
                    }
                }

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(MailUserStore, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 用戶管理中，添加用戶時的下拉框綁定數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase ManageUser()
        {
            string jsonStr = string.Empty;
            try
            {
                List<ManageUserQuery> store = new List<ManageUserQuery>();
                ManageUserQuery query = new ManageUserQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                query.IsPage = false;
                query.user_status = 1;
                _IManageUserMgr = new ManageUserMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _IManageUserMgr.GetNameMail(query, out totalCount);//查询出供應商出貨單
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 修改或者保存用戶信息
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SaveMailUser()
        {
            MailUserQuery query = new MailUserQuery();
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["row_id"]))
                {
                    query.row_id = Convert.ToInt32(Request.Params["row_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["user_name"]))
                {
                    query.user_name = Request.Params["user_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["user_mail"]))
                {
                    query.user_mail = Request.Params["user_mail"];
                }
                if (!string.IsNullOrEmpty(Request.Params["user_pwd"]))
                {
                    query.user_pwd = Request.Params["user_pwd"];
                }
                query.create_time = DateTime.Now;
                query.update_time = query.create_time;
                query.create_user = (Session["caller"] as Caller).user_id;
                query.update_user = (Session["caller"] as Caller).user_id;
                query.status = 1;
                _IMailUserMgr = new MailUserMgr(mySqlConnectionString);
                int result = _IMailUserMgr.SaveMailUser(query);
                if (result > 0)
                {
                    json = "{success:true,msg:\"" + result + "\"}";
                }
                else
                {
                    json = "{success:true,msg:\"" + result + "\"}";
                }
                //else if (result == -1)//羣組或編碼重複
                //{
                //    json = "{failure:true,msg:'-1'}";
                //}
                //else
                //{
                //    json = "{failure:true,msg:'0'}";
                //}
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'0'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 刪除用戶信息
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase DeleteMailUser()
        {
            MailUserQuery query = new MailUserQuery();
            string json = string.Empty;
            try
            {

                if (!string.IsNullOrEmpty(Request.Params["rowId"]))
                {
                    string Row_id = Request.Params["rowId"];
                    Row_id = Row_id.TrimEnd(',');
                    query.row_id_in = Row_id;
                }
                _IMailUserMgr = new MailUserMgr(mySqlConnectionString);
                int result = _IMailUserMgr.DeleteMailUser(query);
                if (result > 0)
                {
                    json = "{success:true,msg:\"" + result + "\"}";
                }
                else
                {
                    json = "{success:false,msg:\"" + result + "\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'0'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        /// <summary>
        /// 修改狀態
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateActive()
        {
            string jsonStr = string.Empty;
            try
            {
                MailUserQuery query = new MailUserQuery();
                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    query.status = Convert.ToInt32(Request.Params["active"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.row_id = Convert.ToInt32(Request.Params["id"]);
                }
                query.update_user = (Session["caller"] as Caller).user_id;
                query.update_time = DateTime.Now;
                _IMailUserMgr = new MailUserMgr(mySqlConnectionString);

                if (_IMailUserMgr.UpdateMailUserStatus(query) > 0)
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
        #region 郵件羣組管理之羣組管理
        #region 羣組列表 +HttpResponseBase MailGroupList
        public HttpResponseBase MailGroupList()
        {
            string json = string.Empty;
            MailGroupQuery query = new MailGroupQuery();
            if (!string.IsNullOrEmpty(Request["isPage"]))
                query.IsPage = bool.Parse(Request["isPage"]);
            List<MailGroupQuery> store = new List<MailGroupQuery>();
            _IMailGroupMgr = new MailGroupMgr(mySqlConnectionString);
            int totalCount = 0;
            try
            {
                query.status = 1;
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                store = _IMailGroupMgr.MailGroupList(query, out totalCount);
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{failure:true}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 新增編輯羣組+HttpResponseBase SaveMailGroup()
        public HttpResponseBase SaveMailGroup()
        {
            MailGroupQuery query = new MailGroupQuery();
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["row_id"]))
                {
                    query.row_id = Convert.ToInt32(Request.Params["row_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["groupName"]))
                {
                    query.group_name = Request.Params["groupName"];
                }
                if (!string.IsNullOrEmpty(Request.Params["groupCode"]))
                {
                    query.group_code = Request.Params["groupCode"];
                }
                if (!string.IsNullOrEmpty(Request.Params["remark"]))
                {
                    query.remark = Request.Params["remark"];
                }
                query.create_time = DateTime.Now;
                query.update_time = query.create_time;
                query.create_user = (Session["caller"] as Caller).user_id;
                query.update_user = (Session["caller"] as Caller).user_id;
                query.status = 1;
                _IMailGroupMgr = new MailGroupMgr(mySqlConnectionString);
                int result = _IMailGroupMgr.SaveMailGroup(query);
                if (result > 0)
                {
                    json = "{success:true}";
                }
                else if (result == -1)//羣組或編碼重複
                {
                    json = "{failure:true,msg:'-1'}";
                }
                else
                {
                    json = "{failure:true,msg:'0'}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{failure:true,msg:'0'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 刪除羣組 + HttpResponseBase DeleteMailGroup
        public HttpResponseBase DeleteMailGroup()
        {
            string json = string.Empty;
            MailGroupQuery query = null;
            _IMailGroupMgr = new MailGroupMgr(mySqlConnectionString);
            List<MailGroupQuery> list = new List<MailGroupQuery>();
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["rowID"]))
                {
                    string rowIDs = Request.Form["rowID"];
                    if (rowIDs.IndexOf("|") != -1)
                    {
                        foreach (string id in rowIDs.Split('|'))
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                query = new MailGroupQuery();
                                query.row_id = Convert.ToInt32(id);
                                list.Add(query);
                            }
                        }
                    }
                    if (_IMailGroupMgr.DeleteMailGroup(list))
                    {
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{failure:true}";
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{failure:true}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 變更狀態，軟刪除+JsonResult UpMailGroupStatus
        public JsonResult UpMailGroupStatus()
        {
            string json = string.Empty;
            try
            {
                MailGroupQuery query = new MailGroupQuery();
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.row_id = Convert.ToInt32(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    query.status = Convert.ToInt32(Request.Params["active"]);
                }
                query.update_time = DateTime.Now;
                query.update_user = (Session["caller"] as Caller).user_id;
                _IMailGroupMgr = new MailGroupMgr(mySqlConnectionString);
                if (_IMailGroupMgr.UpMailGroupStatus(query) > 0)
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
        #region 查詢羣組中已選的人 +string QueryUserById
        public string QueryUserById()
        {
            string json = string.Empty;
            MailGroupMapQuery query = new MailGroupMapQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["groupId"]))
                {
                    query.group_id = Convert.ToInt32(Request.Form["groupId"]);
                }
                query.status = 1;
                _IMailGroupMgr = new MailGroupMgr(mySqlConnectionString);
                json = _IMailGroupMgr.QueryUserById(query);
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
        #endregion
        #region 將所選擇的人插入到組中 + public HttpResponseBase AddCallid
        public HttpResponseBase AddCallid()
        {
            List<MailGroupMapQuery> list = new List<MailGroupMapQuery>();
            MailGroupMapQuery query = new MailGroupMapQuery();
            string json = string.Empty;
            _IMailGroupMgr = new MailGroupMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["groupId"]))
                {
                    query.group_id = Int32.Parse(Request.Form["groupId"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["callid"]))
                {
                    string[] callid = Request.Form["callid"].IndexOf(",") != -1 ? Request.Form["callid"].Split(',') : new string[] { Request.Form["callid"] };

                    _IMailGroupMgr.DeleteMailMap(query.group_id);
                    for (int i = 0; i < callid.Length; i++)
                    {
                        query = new MailGroupMapQuery();
                        query.group_id = Int32.Parse(Request.Form["groupId"]);
                        query.user_id = Convert.ToInt32(callid[i]);
                        query.create_time = DateTime.Now;
                        query.update_time = query.create_time;
                        query.create_user = (Session["caller"] as Caller).user_id;
                        query.update_user = (Session["caller"] as Caller).user_id;
                        query.status = _IMailGroupMgr.GetStatus(query.user_id);
                        list.Add(query);
                    }
                    if (_IMailGroupMgr.SaveMailMap(list))
                    {
                        json = "{success:true,msg:\"新增成功\"}";
                    }
                    else
                    {
                        json = "{success:true,msg:\"新增失败\"}";
                    }
                }
                else
                {

                    if (_IMailGroupMgr.DeleteMailMap(query.group_id) > 0)
                    {
                        json = "{success:true,msg:\"人员已清空\"}";
                    }
                    else
                    {
                        json = "{success:true,msg:\"人员已清空\"}";
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,msg:\"新增失败\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 查詢人員詳細信息
        //add by wwei0216w 2015/4/9
        public HttpResponseBase GetMemberInfo()
        {
            string json = "";
            MailGroupMapQuery query = new MailGroupMapQuery();
            try
            {

                if (!string.IsNullOrEmpty(Request.Form["rowId"]))
                {
                    query.group_id = Convert.ToInt32(Request.Form["rowId"]);
                    query.status = 1;
                    _IMailGroupMgr = new MailGroupMgr(mySqlConnectionString);
                    var list = _IMailGroupMgr.QueryUserInfo(query);
                    //StringBuilder sb = new StringBuilder("[");
                    //sb.Append("{rowId:" + query.group_id + ",groupName:\"" + list[0].group_name + "\",item:[");
                    //foreach(MailGroupMapQuery mm in list)
                    //{
                    //    sb.Append("{user_name:\"" + mm.user_mail + "\",row_id:" + mm.row_id + "}");
                    //}
                    //sb.Append("]}");
                    //sb.Append("]");
                    //json = sb.ToString().Replace("}{", "},{");

                    json = "{items:" + JsonConvert.SerializeObject(list) + "}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #endregion
    }
}
