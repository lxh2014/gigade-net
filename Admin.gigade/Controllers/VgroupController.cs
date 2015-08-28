/*
* 文件名稱 :FgroupController.cs
* 文件功能描述 :供應商群組管理
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/02
* 修改人員 :shiwei0620j
* 版本資訊 : 
* 日期 : 2014/08/18
* 修改備註 : 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using BLL.gigade.Mgr;
using Resources;
using BLL.gigade.Model;

namespace Admin.gigade.Controllers
{
    public class VgroupController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private VgroupMgr vgMgr;
        private VGroupCallerMgr gcMgr;
        Vgroup vg = new Vgroup();
        groupCaller gc = new groupCaller();
        //
        // GET: /Vgroup/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Vgroups()
        {
            return View();
        }
        [CustomHandleError]
        public string QueryAll()
        {

            vgMgr = new VgroupMgr(mySqlConnectionString);

            string json = string.Empty;

            try
            {
                json = vgMgr.QueryAll();
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
        public string QueryCallid()
        {

            vgMgr = new VgroupMgr(mySqlConnectionString);

            string json = string.Empty;

            try
            {
                json = vgMgr.QueryCallid();
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
        public string QueryCallidById()
        {
            gcMgr = new VGroupCallerMgr(mySqlConnectionString);
            string json = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["groupId"]))
                {
                    gc.groupId = Convert.ToInt32(Request.Form["groupId"]);
                }
                json = gcMgr.QueryCallidById(gc);
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
        public HttpResponseBase Add()
        {
            vgMgr = new VgroupMgr(mySqlConnectionString);
            string json = string.Empty;

            try
            {
                vg.rowid = 0;
                vg.groupName = Request.Form["groupName"];
                vg.groupCode = Request.Form["groupCode"];
                vg.remark = Request.Form["remark"];
                vg.kuser = "";

                int num = vgMgr.Save(vg);

                if (num == -1)
                {
                    json = "{success:true,msg:\"群組名稱 或 群組編號已存在。\"}";
                }
                else if (num == 1)
                {
                    json = "{success:true,msg:\"新增成功\"}";
                }
                else
                {
                    json = "{success:true,msg:\"新增失败\"}";
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

        [CustomHandleError]
        /// <summary>
        /// 人員管理
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase AddCallid()
        {
            gcMgr = new VGroupCallerMgr(mySqlConnectionString);
            string json = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["groupId"]))
                {
                    gc.groupId = Int32.Parse(Request.Form["groupId"]);
                }

                if (!string.IsNullOrEmpty(Request.Form["callid"]))
                {
                    string[] callid = Request.Form["callid"].IndexOf(",") != -1 ? Request.Form["callid"].Split(',') : new string[] { Request.Form["callid"] };
                    gcMgr.Delete(gc);

                    foreach (string id in callid)
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            gc.callid = id;
                            gcMgr.Save(gc);
                        }
                    }
                }
                json = "{success:true,msg:\"新增成功\"}";
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

        [CustomHandleError]
        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase Edit()
        {
            vgMgr = new VgroupMgr(mySqlConnectionString);
            string json = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["rowid"]))
                {
                    vg.rowid = Convert.ToInt32(Request.Form["rowid"]);
                }

                if (!string.IsNullOrEmpty(Request.Form["groupName"]))
                {
                    vg.groupName = Request.Form["groupName"];
                }

                if (!string.IsNullOrEmpty(Request.Form["groupCode"]))
                {
                    vg.groupCode = Request.Form["groupCode"];
                }

                if (!string.IsNullOrEmpty(Request.Form["remark"]))
                {
                    vg.remark = Request.Form["remark"];
                }

                vg.kuser = "";
                int num = vgMgr.Save(vg);

                if (num == -1)
                {
                    json = "{success:true,msg:\"群組名稱 或 群組編號已存在。\"}";
                }
                else if (num == 2)
                {
                    json = "{success:true,msg:\"修改成功\"}";
                }
                else
                {
                    json = "{success:true,msg:\"修改失败\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,msg:\"修改失败\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        [CustomHandleError]
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase Delete()
        {
            vgMgr = new VgroupMgr(mySqlConnectionString);
            string json = string.Empty;

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
                                vg.rowid = Int32.Parse(id);
                                vgMgr.Delete(vg);
                            }
                        }
                        json = "{success:true,msg:\"删除成功\"}";
                    }
                    else
                    {
                        json = "{success:true,msg:\"删除失败\"}";
                    }
                }
                else
                {
                    json = "{success:true,msg:\"删除失败\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,msg:\"删除失败\"}";
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();

            return this.Response;
        }

    }
}
