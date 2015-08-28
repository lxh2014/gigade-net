#region 文件内容
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromoAllController.cs
* 摘 要：
* 點數抵用與資料庫交互方法  
* 当前版本：v1.1
* 作 者：hongfei0416j    
* 完成日期：2014/6/20 
* 修改歷史:
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：zhejiang0304j 
*         v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋 
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace Admin.gigade.Controllers
{
    public class PromoAllController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IPromoAllImplMgr _promAllMgr = new PromoAllMgr(mySqlConnectionString);

        public ActionResult Index()
        {
            return View();
        }
        #region 根據查詢條件獲取促銷活動列表+HttpResponseBase GetPromoAllList()
        /// <summary>
        /// 根據查詢條件獲取促銷活動列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetPromoAllList()
        {
            string json = string.Empty;
            int totalCount = 0;
            string brandId = Request["brandId"];
            List<PromoAll> store = new List<PromoAll>();
            PromoAllQuery query = new PromoAllQuery();
            query.status = 1;
            if (!string.IsNullOrEmpty(brandId))
            {
                query.brand_id = Convert.ToInt32(brandId);
            }
            try
            {
                store = _promAllMgr.GetList(query);
                totalCount = store.Count;
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
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

            Response.Clear();
            Response.Write(json);
            Response.End();
            return this.Response;
        } 
        #endregion
    }
}
