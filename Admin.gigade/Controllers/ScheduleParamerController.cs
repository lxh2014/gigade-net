using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Admin.gigade.Controllers
{
    public class ScheduleParamerController : Controller
    {
        //
        // GET: /ScheduleParamer/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private ScheduleParamerMgr _schmerMgr;
        #region view
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ScheduleParamer()
        {
            return View();
        }
        #endregion

        public HttpResponseBase ScheduleParamerAll()
        {
            string json = string.Empty;
            ScheduleParamer sp = new ScheduleParamer();
            try
            {
                //if (!string.IsNullOrEmpty(Request.Params["schedule_code"]))//判斷id是不是存在.也就是說是否選擇了運送方式.當id不為0時表示選擇了運送方式.
                //{
                //    sp.schedule_code = Request.Params["schedule_code"];
                //}
                sp.schedule_code = Request.Params["code"];
                _schmerMgr = new ScheduleParamerMgr(mySqlConnectionString);
                DataTable _dt = _schmerMgr.GetScheduleParamerList(sp.schedule_code);
                //根據行para_value和行para_name拼接成一個json
                int lenght = _dt.Rows.Count;
                json = "{";
                for (int i = 1; i <= _dt.Rows.Count; i++)
                {
                    json = json + "\"" + _dt.Rows[i - 1]["para_name"] + "\":\"" + _dt.Rows[i - 1]["para_value"] + "\"";
                    if (lenght == i)
                    {
                        continue;
                    }
                    else
                    {
                        json = json + ",";
                    }
                }
                if (lenght > 0)
                {
                    json = json + "}";
                }
                else
                {
                    json = "{\"msg\":\"無數據信息\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        #region 加載列表頁
        public HttpResponseBase ScheduleParamerList()
        {
            string json = string.Empty;
            ScheduleParamer sp = new ScheduleParamer();
            List<ScheduleParamer> spList = new List<ScheduleParamer>();
            int totalCount = 0;
            try
            {
                _schmerMgr = new ScheduleParamerMgr(mySqlConnectionString);
                sp.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                sp.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                spList = _schmerMgr.GetScheduleParameterList(sp, out totalCount);
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(spList, Formatting.Indented) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 新增/編輯保存
        public HttpResponseBase ScheduleParamerSave()
        {
            string json = string.Empty;
            ScheduleParamer sp = new ScheduleParamer();
            try
            {
                _schmerMgr = new ScheduleParamerMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["para_id"]))
                {
                    sp.para_id = Convert.ToInt32(Request.Params["para_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["para_value"]))
                {
                    sp.para_value = Request.Params["para_value"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["para_name"]))
                {
                    sp.para_name = Request.Params["para_name"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["schedule_code"]))
                {
                    sp.schedule_code = Request.Params["schedule_code"].ToString();
                }
                sp.para_status = 1;
                if (sp.para_id != 0)
                {
                    int res = _schmerMgr.UpdateScheduleParamer(sp);
                    if (res > 0)
                    {
                        json = "{success:true}";//返回json數據
                    }
                    else
                    {
                        json = "{success:false}";//返回json數據
                    }
                }
                else
                {
                    int res = _schmerMgr.InsertScheduleParamer(sp);
                    if (res > 0)
                    {
                        json = "{success:true}";//返回json數據
                    }
                    else
                    {
                        json = "{success:false}";//返回json數據
                    }
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 更新狀態
        public JsonResult UpdateActive()
        {
            ScheduleParamer sp = new ScheduleParamer();
            try
            {
                _schmerMgr = new ScheduleParamerMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    sp.para_id = Convert.ToInt32(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    sp.para_status = Convert.ToInt32(Request.Params["active"].ToString());
                }
                if (sp.para_id != 0)
                {
                    int res = _schmerMgr.UpdateActive(sp);
                    if (res > 0)
                    {
                        return Json(new { success = "true" });
                    }
                    else
                    {
                        return Json(new { success = "false" });
                    }
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
        #region 列表刪除
        public HttpResponseBase DeleteScheduleParameter()
        {
            string json = string.Empty;
            ScheduleParamer sp = new ScheduleParamer();
            try
            {
                _schmerMgr = new ScheduleParamerMgr(mySqlConnectionString);
                string id = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["rowID"]))
                {
                    id = Request.Params["rowID"].ToString();
                }
                string[] ids = id.Split('|');
                for (int i = 0; i < ids.Length - 1; i++)
                {
                    sp.para_id = Convert.ToInt32(ids[i]);
                    if (sp.para_id > 0)
                    {
                        int delresult = _schmerMgr.DelScheduleParamer(sp);
                        if (delresult <= 0)
                        {
                            json = "{success:true,msg:'" + i + 1 + "'}";
                            break;
                        }
                    }
                    else
                    {
                        json = "{success:false}";
                        break;
                    }
                }
                json = "{success:true,msg:''}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        #endregion

    }
}
