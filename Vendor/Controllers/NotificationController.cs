using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using System.Configuration;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;
using Vendor.CustomHandleError;

namespace Vendor.Controllers
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


                string subject = Resources.VendorProduct.MAIL_SUBJECT;
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

                string subject = Resources.VendorProduct.PRODUCT_UPDATE_MAIL_SUBJECT;
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

                string subject = Resources.VendorProduct.PRODUCT_UPDATE_MAIL_SUBJECT_CHANAL;//外站商品對照異動通知 add by hufeng0813w 2014/07/07
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
    }
}
