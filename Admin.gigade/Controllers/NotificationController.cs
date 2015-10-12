using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using System.Configuration;
using System.Text;
using Admin.gigade.CustomError;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;
using System.IO;
using System.Xml;
using gigadeExcel.Comment;
using System.Data;
using BLL.gigade.Common;

namespace Admin.gigade.Controllers
{
    [HandleError]
    public class NotificationController : Controller
    {
        //
        // GET: /Notification/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        static string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];
        private IViewCheckAlarmImplMgr _viewAlarmMgr;
        private ParameterMgr _paraMgr;
        private ITableHistoryImplMgr _tableHistoryMgr;
        private ITableHistoryItemImplMgr _tableHistoryItemMgr;
        private IHistoryBatchImplMgr _historyBatchMgr;

        private IProductImplMgr _productMgr;
        private IProductItemImplMgr _productItemMgr;
        private IProductSpecImplMgr _productSpecMgr;
        private IPriceMasterImplMgr _priceMasterMgr;
        private IItemPriceImplMgr _itemPriceMgr;
        private ISiteImplMgr _siteMgr;
        private IUsersImplMgr _userMgr;
        private IVendorBrandImplMgr _vendorBrandMgr;
        private ISiteConfigImplMgr _siteConfigMgr;
        private IProductRemoveReasonImplMgr _proRemoveMgr;

        public ActionResult Index()
        {
            return View();
        }

        #region 公用的xml請求
        /// <summary>
        /// 記得在這裡加上xml請求時 必須去 Admin.gigade\Module\ActionFilter.cs 文件中加上判斷不然每次都會提示登錄過期
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetXmlInfo()
        {
            string msg = string.Empty;
            try
            {
                //公共的
                _paraMgr = new ParameterMgr(connectionString);
                _siteConfigMgr = new SiteConfigMgr(Server.MapPath(xmlPath));
                List<Parametersrc> listP = _paraMgr.QueryType("");
                string to = string.Empty, sendtime = string.Empty, status = string.Empty, HttpPath = string.Empty;
                //根據parameterType數量來循環
                if (listP.Count > 0)
                {
                    string getHttp = "http://" + HttpContext.Request.Url.Authority.ToString() + "/Notification/";
                    StringBuilder strXml = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?><root>");
                    for (int i = 0, j = listP.Count; i < j; i++)
                    {
                        string type = listP[i].ParameterType;
                        //讀取數據庫中收件人與通知狀態
                        List<Parametersrc> pList = _paraMgr.QueryUsed(new Parametersrc { ParameterType = type });
                        switch (type)
                        {
                            #region 根據不同的type來處理
                            case "warn_stock":
                                to = pList.Where(rec => rec.parameterName == "sendTo").FirstOrDefault().ParameterCode;
                                to = to.Replace("\n", "");
                                sendtime = pList.Where(rec => rec.parameterName == "sendTime").FirstOrDefault().ParameterCode;
                                sendtime = sendtime.Replace("\n", "");
                                status = pList.Where(rec => rec.parameterName == "switch").FirstOrDefault().ParameterCode;
                                HttpPath = "CheckStockAlarm";

                                strXml.Append("<mail>");
                                strXml.AppendFormat("<status>{0}</status>", status);//0 關閉，1 正常， 2 無資料，3 服務器異常
                                strXml.AppendFormat("<mailto>{0}</mailto>", to);
                                strXml.AppendFormat("<sendTime>{0}</sendTime>", sendtime);
                                strXml.AppendFormat("<getHttp>{0}</getHttp>", getHttp);//網站的IP地址+controller
                                strXml.AppendFormat("<HttpPath>{0}</HttpPath>", HttpPath);//function
                                strXml.Append("</mail>");
                                break;
                            case "warn_product":
                                to = pList.Where(rec => rec.parameterName == "sendTo").FirstOrDefault().ParameterCode;
                                to = to.Replace("\n", "");
                                sendtime = pList.Where(rec => rec.parameterName == "sendTime").FirstOrDefault().ParameterCode;
                                sendtime = sendtime.Replace("\n", "");
                                status = pList.Where(rec => rec.parameterName == "switch").FirstOrDefault().ParameterCode;
                                HttpPath = "ProductUpdateNotice";

                                strXml.Append("<mail>");
                                strXml.AppendFormat("<status>{0}</status>", status);//0 關閉，1 正常， 2 無資料，3 服務器異常
                                strXml.AppendFormat("<mailto>{0}</mailto>", to);
                                strXml.AppendFormat("<sendTime>{0}</sendTime>", sendtime);
                                strXml.AppendFormat("<getHttp>{0}</getHttp>", getHttp);//網站的IP地址+controller
                                strXml.AppendFormat("<HttpPath>{0}</HttpPath>", HttpPath);//function
                                strXml.Append("</mail>");
                                break;
                            case "warn_productMap":
                                to = pList.Where(rec => rec.parameterName == "sendTo").FirstOrDefault().ParameterCode;
                                to = to.Replace("\n", "");
                                sendtime = pList.Where(rec => rec.parameterName == "sendTime").FirstOrDefault().ParameterCode;
                                sendtime = sendtime.Replace("\n", "");
                                status = pList.Where(rec => rec.parameterName == "switch").FirstOrDefault().ParameterCode;
                                HttpPath = "ProductItemUpdateNotice";

                                strXml.Append("<mail>");
                                strXml.AppendFormat("<status>{0}</status>", status);//0 關閉，1 正常， 2 無資料，3 服務器異常
                                strXml.AppendFormat("<mailto>{0}</mailto>", to);
                                strXml.AppendFormat("<sendTime>{0}</sendTime>", sendtime);
                                strXml.AppendFormat("<getHttp>{0}</getHttp>", getHttp);//網站的IP地址+controller
                                strXml.AppendFormat("<HttpPath>{0}</HttpPath>", HttpPath);//function
                                strXml.Append("</mail>");
                                break;
                            #endregion
                        }
                    }
                    strXml.Append("</root>");
                    msg = strXml.ToString();
                }
                else
                {
                    msg = "<mail><status>3</status><msg>paras are not set.</msg></mail>";
                }
            }
            catch
            {
                msg = "<mail><status>3</status></mail>";
            }
            this.Response.Clear();
            this.Response.Write(msg);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 庫存警告
        public HttpResponseBase CheckStockAlarm()
        {
            _viewAlarmMgr = new ViewCheckAlarmMgr(connectionString);
            _paraMgr = new ParameterMgr(connectionString);
            _siteConfigMgr = new SiteConfigMgr(Server.MapPath(xmlPath));
            string msg = string.Empty;
            try
            {
                string from = string.Empty, status = string.Empty, host = string.Empty, username = string.Empty, userpasswd = string.Empty, mailport = string.Empty;

                //讀取XML中有關通知參數
                List<SiteConfig> configList = _siteConfigMgr.Query();
                if (configList.Count() > 0)
                {
                    from = configList.Where(m => m.Name.Equals("Mail_From")).FirstOrDefault().Value;
                    host = configList.Where(m => m.Name.Equals("Mail_Host")).FirstOrDefault().Value;
                    username = configList.Where(m => m.Name.Equals("Mail_UserName")).FirstOrDefault().Value;
                    userpasswd = configList.Where(m => m.Name.Equals("Mail_UserPasswd")).FirstOrDefault().Value;
                    mailport = configList.Where(m => m.Name.Equals("Mail_Port")).FirstOrDefault().Value;
                }


                string subject = Resources.Product.MAIL_SUBJECT;
                string body = _viewAlarmMgr.QueryStockAlarm();
                if (body == "") status = "2";
                StringBuilder strXml = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?><mail>");
                strXml.AppendFormat("<status>{0}</status>", status);//0 關閉，1 正常， 2 無資料，3 服務器異常
                strXml.AppendFormat("<mailfrom>{0}</mailfrom>", from);
                strXml.AppendFormat("<mailsubject>{0}</mailsubject>", subject);
                strXml.AppendFormat("<mailhost>{0}</mailhost>", host);
                strXml.AppendFormat("<mailusername>{0}</mailusername>", username);
                strXml.AppendFormat("<mailuserpasswd>{0}</mailuserpasswd>", userpasswd);
                strXml.AppendFormat("<mailport>{0}</mailport>", mailport);
                strXml.AppendFormat("<mailbody>{0}</mailbody>", HttpUtility.HtmlEncode(body));
                strXml.Append("</mail>");
                msg = strXml.ToString();
            }
            catch (Exception)
            {
                msg = "<mail><status>3</status></mail>";
            }

            this.Response.Clear();
            this.Response.Write(msg);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 商品異動通知
        [CustomHandleError]
        public HttpResponseBase ProductUpdateNotice()
        {
            string msg = string.Empty;
            try
            {
                _paraMgr = new ParameterMgr(connectionString);
                _siteConfigMgr = new SiteConfigMgr(Server.MapPath(xmlPath));

                string from = string.Empty, to = string.Empty, sendtime = string.Empty, status = string.Empty, host = string.Empty, username = string.Empty, userpasswd = string.Empty, mailport = string.Empty;

                List<SiteConfig> configList = _siteConfigMgr.Query();
                if (configList.Count() > 0)
                {
                    from = configList.Where(m => m.Name.Equals("Mail_From")).FirstOrDefault().Value;
                    host = configList.Where(m => m.Name.Equals("Mail_Host")).FirstOrDefault().Value;
                    username = configList.Where(m => m.Name.Equals("Mail_UserName")).FirstOrDefault().Value;
                    userpasswd = configList.Where(m => m.Name.Equals("Mail_UserPasswd")).FirstOrDefault().Value;
                    mailport = configList.Where(m => m.Name.Equals("Mail_Port")).FirstOrDefault().Value;
                }

                string subject = Resources.Product.PRODUCT_UPDATE_MAIL_SUBJECT;
                string body = QueryHistory(0);//0為商品異動通知
                if (body == "") status = "2";
                StringBuilder strXml = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?><mail>");
                strXml.AppendFormat("<status>{0}</status>", status);//0 關閉 ，1 正常， 2 無資料，3 服務器異常
                strXml.AppendFormat("<mailfrom>{0}</mailfrom>", from);
                strXml.AppendFormat("<mailsubject>{0}</mailsubject>", subject);
                strXml.AppendFormat("<mailhost>{0}</mailhost>", host);
                strXml.AppendFormat("<mailusername>{0}</mailusername>", username);
                strXml.AppendFormat("<mailuserpasswd>{0}</mailuserpasswd>", userpasswd);
                strXml.AppendFormat("<mailport>{0}</mailport>", mailport);
                strXml.AppendFormat("<mailbody>{0}</mailbody>", HttpUtility.HtmlEncode(body));
                strXml.Append("</mail>");
                msg = strXml.ToString();
            }
            catch (Exception)
            {
                msg = "<mail><status>3</status></mail>";
            }
            this.Response.Clear();
            this.Response.Write(msg);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 商品對照異動通知
        /// <summary>
        /// 暫時不能確定
        /// </summary>
        /// <returns>返回一個xml的字符串</returns>
        public HttpResponseBase ProductItemUpdateNotice()
        {
            string msg = string.Empty;
            try
            {
                _paraMgr = new ParameterMgr(connectionString);
                _siteConfigMgr = new SiteConfigMgr(Server.MapPath(xmlPath));

                string from = string.Empty, to = string.Empty, sendtime = string.Empty, status = string.Empty, host = string.Empty, username = string.Empty, userpasswd = string.Empty, mailport = string.Empty;

                List<SiteConfig> configList = _siteConfigMgr.Query();
                if (configList.Count() > 0)
                {
                    from = configList.Where(m => m.Name.Equals("Mail_From")).FirstOrDefault().Value;
                    host = configList.Where(m => m.Name.Equals("Mail_Host")).FirstOrDefault().Value;
                    username = configList.Where(m => m.Name.Equals("Mail_UserName")).FirstOrDefault().Value;
                    userpasswd = configList.Where(m => m.Name.Equals("Mail_UserPasswd")).FirstOrDefault().Value;
                    mailport = configList.Where(m => m.Name.Equals("Mail_Port")).FirstOrDefault().Value;
                }

                string subject = Resources.Product.PRODUCT_UPDATE_MAIL_SUBJECT_CHANAL;//外站商品對照異動通知 add by hufeng0813w 2014/07/07
                string body = QueryHistory(1);//為商品對照異動.根據商品對照表中存在的就發送沒有的就不發送
                if (body == "") status = "2";
                StringBuilder strXml = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?><mail>");
                strXml.AppendFormat("<status>{0}</status>", status);//0 關閉 ，1 正常， 2 無資料，3 服務器異常
                strXml.AppendFormat("<mailfrom>{0}</mailfrom>", from);
                strXml.AppendFormat("<mailsubject>{0}</mailsubject>", subject);
                strXml.AppendFormat("<mailhost>{0}</mailhost>", host);
                strXml.AppendFormat("<mailusername>{0}</mailusername>", username);
                strXml.AppendFormat("<mailuserpasswd>{0}</mailuserpasswd>", userpasswd);
                strXml.AppendFormat("<mailport>{0}</mailport>", mailport);
                strXml.AppendFormat("<mailbody>{0}</mailbody>", HttpUtility.HtmlEncode(body));
                strXml.Append("</mail>");
                msg = strXml.ToString();
            }
            catch (Exception)
            {
                msg = "<mail><status>3</status></mail>";
            }
            this.Response.Clear();
            this.Response.Write(msg);
            this.Response.End();
            return this.Response;
        }
        #endregion

        private string QueryHistory(int itemType)
        {
            StringBuilder html = new StringBuilder();
            try
            {
                _historyBatchMgr = new HistoryBatchMgr(connectionString);
                List<HistoryBatch> batches = _historyBatchMgr.QueryToday(itemType);

                //edit by hufeng0813w 2014/06/13
                batches.Sort(CompareToRowid);//進行一個排序
                //end edit by hufeng0813w 2014/06/13 

                if (batches != null & batches.Count > 0)
                {
                    html.Append("<html><head><style type=\"text/css\">table{text-align:center; font-size: 13px;border:1px solid #99bce8}caption{text-align:center;border:1px solid #99bce8;} td{border:1px solid #99bce8}.red{color:red;}.green{color:green;}.tstyle{width:250px;}</style></head><body>");
                    _tableHistoryMgr = new TableHistoryMgr(connectionString);
                    _tableHistoryItemMgr = new TableHistoryItemMgr(connectionString);
                    html.Append("<ul style=\"list-style:none\">");
                    string channelInfo = "";
                    int batchIndex = -1;
                    foreach (var batch in batches)
                    {
                        //edit by hufeng0813w Reason:如果是商品對照異動通知
                        batchIndex++;
                        if (itemType == 1)
                        {
                            channelInfo += "<b>" + batch.channel_name_full + "</b> (外站商品編號:" + batch.channel_detail_id + ")<br/>";
                            if (batchIndex == 0 || (batches[batchIndex].channel_detail_id == batches[batchIndex - 1].channel_detail_id && batches[batchIndex].channel_name_full == batches[batchIndex].channel_name_full && batches[batchIndex].kdate == batches[batchIndex - 1].kdate) || batchIndex < batches.Count - 1)
                            {
                                continue;
                            }
                        }
                        List<TableHistory> histories = _tableHistoryMgr.Query(new TableHistory { batchno = batch.batchno.ToString() });
                        if (histories != null && histories.Count > 0)
                        {
                            Array tbls = histories.GroupBy(m => m.table_name).Select(m => m.Key).ToArray();
                            List<TableHistoryItem> items;
                            uint productId = 0;

                            #region 初始化

                            StringBuilder pro = new StringBuilder();
                            StringBuilder spec = new StringBuilder();
                            StringBuilder category = new StringBuilder();
                            StringBuilder item = new StringBuilder();
                            StringBuilder master = new StringBuilder();
                            StringBuilder price = new StringBuilder();
                            #endregion

                            foreach (var tbl in tbls)
                            {
                                string tblName = tbl.ToString().ToLower();
                                bool isAdd = false;

                                #region 針對不同表的處理

                                switch (tblName)
                                {
                                    case "product":
                                        #region PRODUCT

                                        items = _tableHistoryItemMgr.Query4Batch(new TableHistoryItemQuery { batchno = batch.batchno.ToString(), table_name = tblName });
                                        if (items != null && items.Count > 0)
                                        {
                                            StringBuilder column_1 = new StringBuilder("<tr><td>欄位名稱</td>");
                                            StringBuilder column_2 = new StringBuilder("<tr><td>修改前</td>");
                                            StringBuilder column_3 = new StringBuilder("<tr><td>修改後</td>");
                                            Array cols = items.GroupBy(m => m.col_name).Select(m => m.Key).ToArray();
                                            foreach (var col in cols)
                                            {
                                                var tmp = items.FindAll(m => m.col_name == col.ToString());
                                                if (tmp.Count == 1 && string.IsNullOrEmpty(tmp.FirstOrDefault().old_value))
                                                { continue; }
                                                else
                                                {
                                                    tmp.Remove(tmp.Find(m => string.IsNullOrEmpty(m.old_value)));
                                                    var first = tmp.FirstOrDefault();
                                                    var last = tmp.LastOrDefault();
                                                    if (first == last)
                                                    {
                                                        GetParamCon(last, true);
                                                    }
                                                    else
                                                    {
                                                        GetParamCon(first, true);
                                                    }
                                                    GetParamCon(last, false);
                                                    column_1.AppendFormat("<td>{0}</td>", first.col_chsname);
                                                    column_2.AppendFormat("<td class=\"red\">{0}</td>", first == last ? last.old_value : first.old_value);
                                                    column_3.AppendFormat("<td class=\"green\">{0}</td>", last.col_value);
                                                    isAdd = true;
                                                }
                                            }
                                            if (isAdd)
                                            {
                                                pro.AppendFormat("<table class=\"tstyle\">{0}</tr>{1}</tr>{2}</tr></table>", column_1, column_2, column_3);
                                            }
                                            if (productId == 0)
                                            {
                                                productId = uint.Parse(histories.Find(m => m.table_name == tblName).pk_value);
                                            }
                                        }
                                        #endregion
                                        break;
                                    case "product_spec":
                                        #region SPEC

                                        StringBuilder spec_1 = new StringBuilder("<tr><td>修改前</td>");
                                        StringBuilder spec_2 = new StringBuilder("<tr><td>修改後</td>");
                                        Array specIds = histories.FindAll(m => m.table_name.ToLower() == tblName).GroupBy(m => m.pk_value).Select(m => m.Key).ToArray();
                                        foreach (var id in specIds)
                                        {
                                            items = _tableHistoryItemMgr.Query4Batch(new TableHistoryItemQuery { batchno = batch.batchno.ToString(), table_name = tblName, pk_value = id.ToString() });
                                            if (items.Count == 1 && string.IsNullOrEmpty(items.FirstOrDefault().old_value))
                                            { continue; }
                                            else
                                            {
                                                items.Remove(items.Find(m => string.IsNullOrEmpty(m.old_value)));
                                                var first = items.FirstOrDefault();
                                                var last = items.LastOrDefault();
                                                spec_1.AppendFormat("<td class=\"red\">{0}</td>", first == last ? last.old_value : first.old_value);
                                                spec_2.AppendFormat("<td class=\"green\">{0}</td>", last.col_value);
                                                isAdd = true;
                                            }
                                        }
                                        if (isAdd)
                                        {
                                            spec.AppendFormat("<table class=\"tstyle\">{0}</tr>{1}</tr></table>", spec_1, spec_2);
                                        }
                                        if (productId == 0)
                                        {
                                            _productSpecMgr = new ProductSpecMgr(connectionString);
                                            ProductSpec pSpec = _productSpecMgr.Query(new ProductSpec { spec_id = uint.Parse(histories.Find(m => m.table_name == tblName).pk_value) }).FirstOrDefault();
                                            if (pSpec != null)
                                            {
                                                productId = pSpec.product_id;
                                            }
                                        }
                                        #endregion
                                        break;
                                    case "product_category_set":
                                        #region CATEGORY

                                        if (productId == 0)
                                        {
                                            productId = uint.Parse(histories.Find(m => m.table_name.ToLower() == tblName).pk_value);
                                        }
                                        items = _tableHistoryItemMgr.Query4Batch(new TableHistoryItemQuery { batchno = batch.batchno.ToString(), table_name = tblName, pk_value = productId.ToString() });
                                        if (items.Count > 0)
                                        {
                                            var first = items.FirstOrDefault();
                                            var last = items.LastOrDefault();
                                            category.Append("<table class=\"tstyle\"><tr><td>修改前</td><td>修改後</td></tr>");
                                            category.AppendFormat("<tr><td class=\"red\">{0}</td>", first == last ? last.old_value : first.old_value);
                                            category.AppendFormat("<td class=\"green\">{0}</td></td></table>", last.col_value);
                                        }
                                        #endregion
                                        break;
                                    case "product_item":
                                        #region ITEM

                                        ProductItem pItem;
                                        _productItemMgr = new ProductItemMgr(connectionString);
                                        Array itemIds = histories.FindAll(m => m.table_name.ToLower() == tblName).GroupBy(m => m.pk_value).Select(m => m.Key).ToArray();
                                        foreach (var id in itemIds)
                                        {
                                            isAdd = false;
                                            pItem = _productItemMgr.Query(new ProductItem { Item_Id = uint.Parse(id.ToString()) }).FirstOrDefault();
                                            if (pItem != null)
                                            {
                                                if (productId == 0)
                                                {
                                                    productId = pItem.Product_Id;
                                                }
                                                string title = pItem.GetSpecName();
                                                string top = "<div style=\"float:left\"><table class=\"tstyle\"><caption>" + title + "</caption><tr><td>欄位名稱</td><td>修改前</td><td>修改后</td></tr>";
                                                string bottom = "</table></div>";
                                                string strContent = "<tr><td>{0}</td><td class=\"red\">{1}</td><td class=\"green\">{2}</td></tr>";
                                                string content = BuildContent(batch.batchno.ToString(), tblName, id.ToString(), strContent, ref isAdd);
                                                if (isAdd)
                                                {
                                                    item.Append(top);
                                                    item.Append(content);
                                                    item.Append(bottom);
                                                }
                                            }
                                        }
                                        #endregion
                                        break;
                                    case "price_master":
                                        #region PRICE_MASTER

                                        PriceMaster pMaster;
                                        _priceMasterMgr = new PriceMasterMgr(connectionString);
                                        Array masterIds = histories.FindAll(m => m.table_name.ToLower() == tblName).GroupBy(m => m.pk_value).Select(m => m.Key).ToArray();
                                        foreach (var id in masterIds)
                                        {
                                            isAdd = false;
                                            pMaster = _priceMasterMgr.Query(new PriceMaster { price_master_id = uint.Parse(id.ToString()) }).FirstOrDefault();
                                            if (pMaster != null)
                                            {
                                                if (productId == 0)
                                                {
                                                    productId = pMaster.product_id;
                                                }
                                                string siteName = QuerySiteName(pMaster.site_id.ToString());
                                                string userLevel = QueryParaName(pMaster.user_level.ToString(), "UserLevel");
                                                string userMail = pMaster.user_id == 0 ? "" : QueryMail(pMaster.user_id.ToString());
                                                string childName = string.Empty;
                                                if (pMaster.child_id != 0 && pMaster.product_id != pMaster.child_id)
                                                {
                                                    _productMgr = new ProductMgr(connectionString);
                                                    Product tmpPro = _productMgr.Query(new Product { Product_Id = Convert.ToUInt32(pMaster.child_id) }).FirstOrDefault();
                                                    if (tmpPro != null)
                                                    {
                                                        childName = tmpPro.Product_Name;
                                                    }
                                                }
                                                string title = siteName + " + " + userLevel + (string.IsNullOrEmpty(userMail) ? "" : (" + " + userMail))
                                                                + (string.IsNullOrEmpty(childName) ? "<br/>" : "<br/>子商品: " + childName);
                                                if (!title.Contains("子商品"))
                                                {
                                                    title += "<br/>";
                                                }
                                                string top = "<div style=\"float:left\"><table class=\"tstyle\"><caption>" + title + "</caption><tr><td>欄位名稱</td><td>修改前</td><td>修改后</td></tr>";
                                                string bottom = "</table></div>";
                                                string strContent = "<tr><td>{0}</td><td class=\"red\">{1}</td><td class=\"green\">{2}</td></tr>";
                                                string content = BuildContent(batch.batchno.ToString(), tblName, id.ToString(), strContent, ref isAdd);
                                                if (isAdd)
                                                {
                                                    master.Append(top);
                                                    master.Append(content);
                                                    master.Append(bottom);
                                                }
                                            }
                                        }
                                        #endregion
                                        break;
                                    case "item_price":
                                        #region ITEM_PRICE

                                        ItemPriceCustom itemPrice;
                                        PriceMaster tmpMaster;
                                        _itemPriceMgr = new ItemPriceMgr(connectionString);
                                        _priceMasterMgr = new PriceMasterMgr(connectionString);
                                        Array priceIds = histories.FindAll(m => m.table_name.ToLower() == tblName).GroupBy(m => m.pk_value).Select(m => m.Key).ToArray();
                                        foreach (var id in priceIds)
                                        {
                                            isAdd = false;
                                            itemPrice = _itemPriceMgr.Query(new ItemPrice { item_price_id = uint.Parse(id.ToString()) }).FirstOrDefault();
                                            if (itemPrice != null)
                                            {
                                                tmpMaster = _priceMasterMgr.Query(new PriceMaster { price_master_id = itemPrice.price_master_id }).FirstOrDefault();
                                                if (tmpMaster != null)
                                                {
                                                    if (productId == 0)
                                                    {
                                                        productId = tmpMaster.product_id;
                                                    }
                                                    string siteName = QuerySiteName(tmpMaster.site_id.ToString());
                                                    string userLevel = QueryParaName(tmpMaster.user_level.ToString(), "UserLevel");
                                                    string userMail = tmpMaster.user_id == 0 ? "" : QueryMail(tmpMaster.user_id.ToString());
                                                    string childName = string.Empty;
                                                    if (tmpMaster.child_id != 0 && tmpMaster.product_id != tmpMaster.child_id)
                                                    {
                                                        _productMgr = new ProductMgr(connectionString);
                                                        Product tmpPro = _productMgr.Query(new Product { Product_Id = Convert.ToUInt32(tmpMaster.child_id) }).FirstOrDefault();
                                                        if (tmpPro != null)
                                                        {
                                                            childName = tmpPro.Product_Name;
                                                        }
                                                    }
                                                    string strSpec = itemPrice.spec_name_1 + (string.IsNullOrEmpty(itemPrice.spec_name_2) ? "" : (" + " + itemPrice.spec_name_2));

                                                    string title = siteName + " + " + userLevel + (string.IsNullOrEmpty(userMail) ? "" : (" + " + userMail))
                                                        + (string.IsNullOrEmpty(childName) ? "<br/>" : "<br/>子商品: " + childName)
                                                        + "<br/>" + strSpec;
                                                    if (strSpec == "")
                                                    {
                                                        title += "<br/>";
                                                    }
                                                    string top = "<div style=\"float:left\"><table class=\"tstyle\"><caption>" + title + "</caption><tr><td>欄位名稱</td><td>修改前</td><td>修改后</td></tr>";
                                                    string bottom = "</table></div>";
                                                    string strContent = "<tr><td>{0}</td><td class=\"red\">{1}</td><td class=\"green\">{2}</td></tr>";
                                                    string content = BuildContent(batch.batchno.ToString(), tblName, id.ToString(), strContent, ref isAdd);
                                                    if (isAdd)
                                                    {
                                                        price.Append(top);
                                                        price.Append(content);
                                                        price.Append(bottom);
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                        break;
                                    default:
                                        break;
                                }
                                #endregion
                            }
                            #region 單批次拼接

                            StringBuilder batchHtml = new StringBuilder();
                            if (pro.Length > 0)
                            {
                                batchHtml.AppendFormat("<tr><td>商品信息</td><td>{0}</td></tr>", pro);
                            }
                            if (spec.Length > 0)
                            {
                                batchHtml.AppendFormat("<tr><td>規格信息</td><td>{0}</td></tr>", spec);
                            }
                            if (category.Length > 0)
                            {
                                batchHtml.AppendFormat("<tr><td>前臺分類信息</td><td>{0}</td></tr>", category);
                            }
                            if (item.Length > 0)
                            {
                                batchHtml.AppendFormat("<tr><td>商品細項信息</td><td>{0}</td></tr>", item);
                            }
                            if (master.Length > 0)
                            {
                                batchHtml.AppendFormat("<tr><td>站臺商品信息</td><td>{0}</td></tr>", master);
                            }
                            if (price.Length > 0)
                            {
                                batchHtml.AppendFormat("<tr><td>站臺價格信息</td><td>{0}</td></tr>", price);
                            };
                            if (batchHtml.Length > 0)
                            {
                                _productMgr = new ProductMgr(connectionString);
                                Product product = _productMgr.Query(new Product { Product_Id = productId }).FirstOrDefault();
                                if (product != null)
                                {
                                    string brand = string.Empty;
                                    _vendorBrandMgr = new VendorBrandMgr(connectionString);
                                    VendorBrand vendorBrand = _vendorBrandMgr.GetProductBrand(new VendorBrand { Brand_Id = product.Brand_Id });
                                    if (vendorBrand != null)
                                    {
                                        brand = vendorBrand.Brand_Name;
                                    }
                                    html.AppendFormat("<li><table><tr><td colspan='2'>商品編號：<b>{0}</b>   品牌：<b>{1}</b></td></tr>", productId, brand);
                                    html.AppendFormat("<tr><td colspan='2'><b>{0}</b>  (修改人:{1}", product.Product_Name, batch.kuser);
                                    html.AppendFormat(",修改時間:{0})</td></tr>", batch.kdate.ToString("yyyy/MM/dd HH:mm:ss"));
                                    html.Append(batchHtml);
                                    if (itemType == 1)
                                    {
                                        html.AppendFormat("<tr><td colspan='2'>{0}</td></tr>", channelInfo);
                                    }
                                    html.Append("</table></li>");
                                }
                            }
                            #endregion
                        }
                    }
                    html.Append("</ul>");
                    html.Append("</body></html>");
                }
            }
            catch (Exception)
            {
                throw;
            }
            return html.ToString();
        }

        public string BuildContent(string batch, string tblName, string pk_Value, string formatContent, ref bool isAdd)
        {
            _tableHistoryItemMgr = new TableHistoryItemMgr(connectionString);
            List<TableHistoryItem> items = _tableHistoryItemMgr.Query4Batch(new TableHistoryItemQuery { batchno = batch.ToString(), table_name = tblName, pk_value = pk_Value });
            StringBuilder strHtml = new StringBuilder();
            if (items != null && items.Count > 0)
            {
                Array cols = items.GroupBy(m => m.col_name).Select(m => m.Key).ToArray();
                foreach (var col in cols)
                {
                    var tmp = items.FindAll(m => m.col_name == col.ToString());
                    if (tmp.Count == 1 && string.IsNullOrEmpty(tmp.FirstOrDefault().old_value))
                    { continue; }
                    else
                    {
                        tmp.Remove(tmp.Find(m => string.IsNullOrEmpty(m.old_value)));
                        var first = tmp.FirstOrDefault();
                        var last = tmp.LastOrDefault();
                        if (first == last)
                        {
                            GetParamCon(last, true);
                        }
                        else
                        {
                            GetParamCon(first, true);
                        }
                        GetParamCon(last, false);
                        strHtml.Append(string.Format(formatContent, first.col_chsname, first == last ? last.old_value : first.old_value, last.col_value));
                        isAdd = true;
                    }
                }
            }
            return strHtml.ToString();
        }

        public void GetParamCon(TableHistoryItem item, bool isOld)
        {
            string content = isOld ? item.old_value : item.col_value;
            switch (item.col_name)
            {
                case "product_mode":
                    content = QueryParaName(content, "product_mode");
                    break;
                case "product_status":
                    content = QueryParaName(content, "product_status");
                    break;
                case "price_status":
                    content = QueryParaName(content, "price_status");
                    break;
                case "user_level":
                    content = QueryParaName(content, "UserLevel");
                    break;
                case "cate_id":
                    content = QueryParaName(content, "product_cate");
                    break;
                case "site_id":
                    content = QuerySiteName(content);
                    break;
                case "user_id":
                    content = QuerySiteName(content);
                    break;
                case "product_start":
                case "product_end":
                case "event_start":
                case "event_end":
                    content = content.Trim() == "0" ? "" : BLL.gigade.Common.CommonFunction.GetNetTime(long.Parse(content)).ToString("yyyy/MM/dd HH:mm:ss");
                    break;
            }
            if (isOld)
            {
                item.old_value = content;
            }
            else
            {
                item.col_value = content;
            }
        }

        public string QueryParaName(string code, string type)
        {
            _paraMgr = new ParameterMgr(connectionString);
            Parametersrc para = _paraMgr.QueryUsed(new Parametersrc { Used = 1, ParameterCode = code, ParameterType = type }).FirstOrDefault();
            if (para != null)
            {
                return para.parameterName;
            }
            return code;
        }

        public string QuerySiteName(string code)
        {
            _siteMgr = new SiteMgr(connectionString);
            Site site = _siteMgr.Query(new Site { Site_Id = uint.Parse(code) }).FirstOrDefault();
            if (site != null)
            {
                return site.Site_Name;
            }
            return code;
        }

        public string QueryMail(string code)
        {
            _userMgr = new UsersMgr(connectionString);
            BLL.gigade.Model.Users user = _userMgr.Query(new BLL.gigade.Model.Users { user_id = ulong.Parse(code) }).FirstOrDefault();
            if (user != null)
            {
                return user.user_email;
            }
            return code;
        }

        //比較器
        public static int CompareToRowid(HistoryBatch x, HistoryBatch y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                return 1;
            }

            if (y == null)
            {
                return -1;
            }

            int retavl = y.product_rowid.CompareTo(x.product_rowid);
            return retavl;
        }

        /// <summary>
        /// 用來刪除商品前後綴的請求 add by wwei 0216w 2014/12/12
        /// </summary>
        /// <returns></returns>
        public int OperateExtendName(uint product_id = 0)
        {
            string startRunInfo = "";
            string endRunInfo = "";
            try
            {
                startRunInfo = DateTime.Now.ToString() + ": OperateExtendName Start";

                //記錄服務執行的開始時間
                DateTime dt_Start = DateTime.Now;
                Caller caller = new Caller();
                IProdNameExtendImplMgr _prodnameExtendMgr = new ProdNameExtendMgr(connectionString);
                //加上商品前後綴
                _prodnameExtendMgr.AddProductExtentName(caller);
                //去掉商品前後綴
                _prodnameExtendMgr.ResetExtendName(caller, product_id);
                //記錄服務執行的結束時間
                DateTime dt_End = DateTime.Now;
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("OperateExtendNameStart:{0};OperateExtendNameEnd:{1}", dt_Start.ToString(), dt_End.ToString());
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Info(logMessage);
                endRunInfo = DateTime.Now.ToString() + ": OperateExtendName End";
                WriterInfo("OperateExtendName", startRunInfo, endRunInfo);
                return 1;

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                endRunInfo = DateTime.Now.ToString() + ": OperateExtendName Exceptional:" + ex.Message;
                WriterInfo("OperateExtendName", startRunInfo, endRunInfo);
                return -1;
            }
        }

        public int SaleStatus()
        {
            string startRunInfo = "";
            string endRunInfo = "";
            try
            {
                startRunInfo = DateTime.Now.ToString() + ": SaleStatus Start";
                //記錄服務執行的開始時間
                DateTime dt_Start = DateTime.Now;
                //更改商品販售狀態 add by wwei0216w 2015/1/30
                IProductImplMgr _product = new ProductMgr(connectionString);
                _product.UpdateSaleStatusBatch();
                //_product.UpdateSaleStatus(15246);
                //_product.UpdateSaleStatusByCondition(new Product { Brand_Id = 2});
                //記錄服務執行的結束時間
                DateTime dt_End = DateTime.Now;
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("SaleStatusStart:{0};SaleStatusEnd:{1}", dt_Start.ToString(), dt_End.ToString());
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Info(logMessage);
                endRunInfo = DateTime.Now.ToString() + ": SaleStatus End ";
                WriterInfo("SaleStatus", startRunInfo, endRunInfo);
                return 1;
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                endRunInfo = DateTime.Now.ToString() + ": SaleStatus Exceptional: " + ex.Message;
                WriterInfo("SaleStatus", startRunInfo, endRunInfo);
                return -1;
            }
        }

        /// <summary>
        /// 將記錄的信息放置在一個xml中
        /// </summary>
        private void WriterInfo(string functionName, string startRunInfo, string endRunInfo)
        {
            XmlDocument xmldoc;
            XmlElement xmlelem;
            try
            {
                string timeStr = DateTime.Now.ToString("yyyy-MM-dd");
                string path = Server.MapPath("~/XML/Exceptional.xml");
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
                    xmldoc.Save(Server.MapPath("~/XML/Exceptional.xml"));//保存创建好的XML文档
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
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }


        #region 設定有關商品自動下架等功能  product_status 7 表示缺货系统下架   
        public string SetProductRmoveDown()
        {

            int resultone = 0;
            int resulttwo = 0;
            int resultthree = 0;
            try
            {
                if (Request.Url.Host == "mng.gigade100.com")//判断是否为正式线
                {
                    return "{success:false,data:'',msg:'未驗收，暫不執行！'}";
                }
                else
                {
                    _proRemoveMgr = new ProductRemoveReasonMgr(connectionString);

                    ProductRemoveReason prr = new ProductRemoveReason();
                    ProductRemoveReason prrtwo = new ProductRemoveReason();
                    Product pt = new Product();
                    ProductStatusHistory psh = new ProductStatusHistory();

                    #region 往临时表中插入数据/或者更新数据  sql可以统一执行
                    //獲取到上架商品庫存<=0,並且排除掉庫存為0可販賣機暫停售賣的商品
                    DataTable _dt = _proRemoveMgr.GetStockLessThanZero();
                    //获取到临时表中的数据
                    DataTable _dttwo = _proRemoveMgr.GetProductRemoveReasonList();
                    //判断临时表是否存在,如果不存在,则插入数据
                    StringBuilder str = new StringBuilder();
                    if (_dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < _dt.Rows.Count; i++)
                        {
                            DataRow[] dr = _dttwo.Select("product_id=" + _dt.Rows[i]["product_id"]);
                            if (dr.Length <= 0)//小于等于0  无需处理编辑数据,因为获取的库存都是等于或者小于0的
                            {
                                prr.create_name = "system";
                                prr.create_time = Convert.ToInt32(CommonFunction.GetPHPTime(CommonFunction.DateTimeToString(DateTime.Now)));
                                prr.product_id = Convert.ToUInt32(_dt.Rows[i]["product_id"]);
                                prr.product_num = Convert.ToInt32(_dt.Rows[i]["item_stock"]);
                                str.AppendFormat(_proRemoveMgr.InsertProductRemoveReason(prr));
                            }
                        }
                    }
                    //获取出临时表中要删除的数据 
                    DataTable _dtthree = _proRemoveMgr.GetDeleteProductRemoveReasonList();
                    for (int b = 0; b < _dtthree.Rows.Count; b++)
                    {
                        prr.product_id = Convert.ToUInt32(_dtthree.Rows[b]["product_id"]);
                        str.AppendFormat(_proRemoveMgr.DeleteProductRemoveReason(prr));
                    }
                    if (str.ToString().Length > 0)
                    {
                        resultone = _proRemoveMgr.ProductRemoveReasonTransact(str.ToString());
                    }
                    else
                    {
                        resultone = 1;
                    }
                    #endregion

                    #region 缺货商品下架
                    DataTable _dtNew = _proRemoveMgr.GetStockMsg();
                    StringBuilder strsql = new StringBuilder();
                    if (_dtNew.Rows.Count > 0)
                    {
                        for (int j = 0; j < _dtNew.Rows.Count; j++)
                        {
                            int time = Convert.ToInt32(_dtNew.Rows[j]["create_time"]);
                            DateTime dttime = CommonFunction.GetNetTime(time);
                            TimeSpan ts = DateTime.Now - dttime;
                            prrtwo.product_id = Convert.ToUInt32(_dtNew.Rows[j]["product_id"]);
                            pt.Product_Id = Convert.ToUInt32(_dtNew.Rows[j]["product_id"]);
                            pt.Product_Status = 7;//7为缺货系统下架
                            psh.product_id = Convert.ToUInt32(_dtNew.Rows[j]["product_id"]);
                            psh.user_id = 2;
                            psh.create_time = DateTime.Now;
                            psh.type = 9;//缺货系统下架
                            psh.product_status = 7;
                            psh.remark = "系统账号插入";
                            if (ts.Days >= (_dtNew.Rows[j]["outofstock_days_stopselling"] == "" ? 0 : Convert.ToInt32(_dtNew.Rows[j]["outofstock_days_stopselling"])))//表示可以下架,执行下架工作
                            {
                                //改变product表中商品的状态
                                strsql.AppendFormat(_proRemoveMgr.UpdateProductStatus(pt));
                                //网记录表中插入一条数据
                                strsql.AppendFormat(_proRemoveMgr.InsertIntoProductStatusHistory(psh));
                                //删除临时表中的数据
                                strsql.AppendFormat(_proRemoveMgr.DeleteProductRemoveReason(prrtwo));
                            }
                        }
                    }
                    if (strsql.ToString().Length > 0)
                    {
                        resulttwo = _proRemoveMgr.ProductRemoveReasonTransact(strsql.ToString());
                    }
                    else
                    {
                        resulttwo = 1;
                    }
                    #endregion

                    #region 缺货商品状态改为申请审核
                    DataTable _dtOutofStock = _proRemoveMgr.GetOutofStockMsg();
                    StringBuilder strslqmsg = new StringBuilder();
                    StringBuilder strslqmsgtwo = new StringBuilder();
                    if (_dtOutofStock.Rows.Count > 0)
                    {
                        for (int z = 0; z < _dtOutofStock.Rows.Count; z++)
                        {
                            if (Convert.ToInt32(_dtOutofStock.Rows[z]["item_stock"]) > 0)
                            {
                                pt.Product_Id = Convert.ToUInt32(_dtOutofStock.Rows[z]["product_id"]);
                                pt.Product_Status = 1;//1表示申请审核
                                _proRemoveMgr.UpdateProductStatus(pt);
                                psh.product_id = Convert.ToUInt32(_dtOutofStock.Rows[z]["product_id"]);
                                psh.user_id = 2;
                                psh.create_time = DateTime.Now;
                                psh.type = 1;//1表示申请审核
                                psh.product_status = 1;//申请审核
                                psh.remark = "系统账号插入";
                                strslqmsg.AppendFormat(_proRemoveMgr.UpdateProductStatus(pt));
                                strslqmsg.AppendFormat(_proRemoveMgr.InsertIntoProductStatusHistory(psh));
                            }
                            prr.product_id = Convert.ToUInt32(_dtOutofStock.Rows[z]["product_id"]);//根据product_id删除数据
                            strslqmsgtwo.AppendFormat(_proRemoveMgr.DeleteProductRemoveReason(prr));
                        }
                    }

                    if (strslqmsg.ToString().Length > 0)
                    {
                        resultthree = _proRemoveMgr.ProductRemoveReasonTransact(strslqmsg.ToString() + strslqmsgtwo.ToString());
                    }
                    else
                    {
                        if (strslqmsgtwo.ToString().Length > 0)
                        {
                            _proRemoveMgr.ProductRemoveReasonTransact(strslqmsgtwo.ToString());
                        }
                        resultthree = 1;
                    }
                    #endregion

                    if (resultone > 0 && resulttwo > 0 && resultthree > 0)
                    {
                        SaleStatus();
                        DataTable _excelMsg = _proRemoveMgr.GetStockMsg();
                        ExeclProductRmoveDownMsg(_excelMsg);
                        return "{success:true}";
                    }
                    else
                    {
                        return "{success:false}";
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return "{success:false,data:'',msg:"+ex.Message+"}";
            }
        }
        #endregion

        public void ExeclProductRmoveDownMsg(DataTable _dtMsg)
        {
            try
            {
                #region 要汇出的excel结构
                List<string> NameList = new List<string>();
                List<DataTable> Elist = new List<DataTable>();
                List<bool> comName = new List<bool>();
                DataTable _dtExeclone = new DataTable();
                _dtExeclone.Columns.Add("商品编号");
                _dtExeclone.Columns.Add("商品状态");
                _dtExeclone.Columns.Add("库存为0是否贩售");
                _dtExeclone.Columns.Add("暂停贩卖商品");
                _dtExeclone.Columns.Add("缺货日期");
                _dtExeclone.Columns.Add("缺货停售天数");
                for (int w = 0; w < _dtMsg.Rows.Count; w++)
                {
                    DataRow drMsg = _dtExeclone.NewRow();
                    drMsg["商品编号"] = _dtMsg.Rows[w]["product_id"];
                    drMsg["商品状态"] = _dtMsg.Rows[w]["product_status"];
                    drMsg["库存为0是否贩售"] = _dtMsg.Rows[w]["shortage"] == "0" ? "是" : "否";
                    drMsg["暂停贩卖商品"] = _dtMsg.Rows[w]["ignore_stock"] == "0" ? "是" : "否";
                    drMsg["缺货日期"] = _dtMsg.Rows[w]["new_time"];
                    drMsg["缺货停售天数"] = _dtMsg.Rows[w]["outofstock_days_stopselling"];
                    _dtExeclone.Rows.Add(drMsg);
                }
                NameList.Add("临时表相关信息");
                Elist.Add(_dtExeclone);
                comName.Add(true);
                //string fileName = "D:\\系统下架处理信息" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                //ExcelHelperXhf.ExportDTtoExcel(_dtExeclone, "系统下架处理信息", fileName);
                string fileName = "系统下架处理信息" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDTNoColumnsBySdy(Elist, NameList, comName);
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
                #endregion
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }
    }
}
