#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：ParameterController.cs
* 摘 要：
* *  參數表controller
* 当前版本：v1.0
* 作 者： mengjuan0826j
* 完成日期：2014/08/26  參數表controller
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using BLL.gigade.Mgr;
using Vendor.CustomHandleError;
using Newtonsoft.Json;
using BLL.gigade.Model;
namespace Vendor.Controllers
{
    public class ParameterController : Controller
    {
        //
        // GET: /Parameter/
        //
        // GET: /Prameter/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];

        private ParameterMgr paraMgr;

         [CustomHandleError]
        [OutputCache(Duration = 3600, VaryByParam = "paraType", Location = System.Web.UI.OutputCacheLocation.Client)]
        public string QueryPara()
        {
            paraMgr = new ParameterMgr(connectionString);
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
         public HttpResponseBase GetProCage1()
         {
             string resultStr = "{success:false}";

             try
             {
                 paraMgr = new ParameterMgr(connectionString);
                 resultStr = "{success:true,data:" + JsonConvert.SerializeObject(paraMgr.QueryUsed(new Parametersrc { ParameterType = "product_cate" }).Where(p => p.TopValue == "0").ToList()) + "}";
             }
             catch (Exception ex)
             {
                 Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                 logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                 logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                 log.Error(logMessage);
             }


             this.Response.Clear();
             this.Response.Write(resultStr);
             this.Response.End();
             return this.Response;
         }
         public HttpResponseBase GetProCage2()
         {
             string resultStr = "{success:false}";

             try
             {
                 string topValue = Request.Params["topValue"] ?? "";
                 paraMgr = new ParameterMgr(connectionString);
                 resultStr = "{success:true,data:" + JsonConvert.SerializeObject(paraMgr.QueryUsed(new Parametersrc { ParameterType = "product_cate", TopValue = topValue })) + "}";
             }
             catch (Exception ex)
             {
                 Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                 logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                 logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                 log.Error(logMessage);
             }


             this.Response.Clear();
             this.Response.Write(resultStr);
             this.Response.End();
             return this.Response;
         }
    }
}
