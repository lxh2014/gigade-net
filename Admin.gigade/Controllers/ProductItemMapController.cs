using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Configuration;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using Newtonsoft.Json;
using LHUntil;
using Admin.gigade.CustomError;
using System.Collections;
using BLL.gigade.Common;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.Data;
using System.IO;
using gigadeExcel.Comment;
namespace Admin.gigade.Controllers
{
    [HandleError]
    public class ProductItemMapController : Controller
    {
        //
        // GET: /ProductItemMap/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        static string excelPath = ConfigurationManager.AppSettings["ImportCompareExcel"];
        private IProductItemMapImplMgr _ProductItemMapMgr;
        private IProductItemMapExcelImplMgr _ProductItemMapExcelImplMgr;
        private string jsonStr = string.Empty;
        //外站商品對照
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryProductItemMap()
        {
            try
            {
                ProductItemMapQuery query = new ProductItemMapQuery();
                if (!string.IsNullOrEmpty(Request.Form["condition"]))
                {
                    query.condition = (ProductItemMapQuery.conditionNo)Int32.Parse(Request.Form["condition"]);
                }
                query.content = Request.Form["value"];
                query.Start = Convert.ToInt32(Request.Form["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Form["limit"] ?? "20");

                _ProductItemMapMgr = new BLL.gigade.Mgr.ProductItemMapMgr(connectionString);
                int totalCount = 0;
                List<ProductItemMapCustom> productmaps = _ProductItemMapMgr.QueryProductItemMap(query, out totalCount);
                jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(productmaps) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:true,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryProductByNo()
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["pNo"]))
                {
                    int pNo = int.Parse(Request.Form["pNo"].ToString());
                    int cId = 0;
                    int pmId = 0;//增加站點價格price_master_id
                    Int32.TryParse(Request.Form["cId"] ?? "0", out cId);
                    Int32.TryParse(Request.Form["pmId"] ?? "0", out pmId);

                    _ProductItemMapMgr = new BLL.gigade.Mgr.ProductItemMapMgr(connectionString);
                    int totalCount = 0;
                    List<ProductCompare> productmaps = _ProductItemMapMgr.QueryProductByNo(cId, pNo,pmId, out totalCount);
                    jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(productmaps) + "}";
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:true,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase QueryComboItem()
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["ParentId"]))
                {
                    _ProductItemMapMgr = new ProductItemMapMgr(connectionString);
                    List<ProductComboMap> resultList = _ProductItemMapMgr.ComboItemQuery(new ProductCombo { Parent_Id = int.Parse(Request.Params["ParentId"]) });
                    jsonStr = "{success:true,data:" + JsonConvert.SerializeObject(resultList) + "}";
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:true,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }


        /// <summary>
        /// 查詢 商品類型及組合商品下的子商品
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryCombination()
        {
            string result = "{success:false}";
            uint pid = uint.Parse(Request.Params["ProductId"] ?? "0");
            uint pmId = uint.Parse(string.IsNullOrEmpty(Request.Params["PriceMasterId"]) ? "0" : Request.Params["PriceMasterId"]);
            if (pid != 0)
            {
                ProductItemMapMgr _mapMgr = new ProductItemMapMgr(connectionString);
                List<BLL.gigade.Model.Custom.ProductMapCustom> resultList = _mapMgr.CombinationQuery(new ProductItemMapCustom { product_id = pid, price_master_id = pmId });
                int ListCount = resultList.Count();
                BLL.gigade.Model.Custom.ProductMapCustom pmc = resultList.FirstOrDefault();
                if (ListCount > 0 && pmc.combination == 4)
                {
                    ListCount = resultList.LastOrDefault().pile_id;
                }
                //当为任选组合且每种仅限一单位时，要计算出显示在页面上的组合商品数量！                
                if (ListCount > 0 && pmc.combination == 3&&pmc.buy_limit == 1) {

                    ListCount = pmc.g_must_buy - resultList.Sum(p => p.s_must_buy) + resultList.Count(p => p.s_must_buy != 0);
                }
                if (ListCount > 0)
                {

                    StringBuilder stb = new StringBuilder();
                    StringBuilder strSpec = new StringBuilder();
                    StringBuilder strGroupGMust = new StringBuilder();
                    if (resultList[0].combination != 0 && resultList[0].combination != 1)
                    {
                        stb.Append("[");
                        for (int i = 1, j = ListCount; i <= j; i++)
                        {
                            if (i > 1)
                            {
                                stb.Append(",");
                                strSpec.Append(",");
                                strGroupGMust.Append(",");
                            }
                            stb.Append("{name:'item_id_" + i + "',type:'int'},{name:'product_name_" + i + "',type:'string'},{name:'product_num_" + i + "',type:'int'},{name:'s_must_buy_" + i + "',type:'int'}");
                            strSpec.Append(resultList[i - 1].product_spec);
                            if (resultList.FirstOrDefault().combination == 4)
                            {
                                strGroupGMust.Append(resultList.Where(e => e.pile_id == i).FirstOrDefault().g_must_buy);
                            }
                        }
                        stb.Append("]");
                    }

                    if (resultList.FirstOrDefault().combination != 4)
                    {
                        strGroupGMust.Clear();
                        strGroupGMust.Append(resultList[0].g_must_buy);
                    }

                    result = "{success:true,data:{combination:" + resultList[0].combination + ",price_type:'"+resultList[0].price_type+"',product_name:'" + resultList[0].product_name + "',cost:" + resultList[0].cost + ",price:" + resultList[0].price + ",strSpec:'" + strSpec + "',buy_limit:" + resultList[0].buy_limit + ",g_must_buy:'" + strGroupGMust + "',parent_id:" + pid + ",count:" + ListCount + ",items:\"" + stb.ToString() + "\"}}";

                }
            }
            this.Response.Clear();
            this.Response.Write(result);
            this.Response.End();
            return this.Response;
        }
        
        /// <summary>
        /// 遞歸組合固定/群組商品
        /// </summary>
        /// <param name="count"></param>
        /// <param name="itemAll"></param>
        /// <param name="result"></param>
        /// <param name="r"></param>
        public void getCombo(int count, List<List<ProductComboMap>> itemAll, string result, ref string r)
        {
            if (count >= itemAll.Count)
            {
                r += result + "&";
                return;
            }
            if (count > 0)
            {
                result += ",";
            }
            foreach (ProductComboMap s in itemAll[count])
            {
                getCombo(count + 1, itemAll, result + s.item_id, ref r);
                //getCombo(count + 1, itemAll, result + "item_id" + ":" + s.item_id + ",product_name_" + (count + 1).ToString() + ":'" + s.product_name + (s.spec_name_1 != "" ? "(" + s.spec_name_1 + " " + s.spec_name_2 + ")" : "") + "',product_num_" + (count + 1).ToString() + ":" + s.set_num + ",", ref r);
            }
        }

        /// <summary>
        /// 遞歸任意商品
        /// </summary>
        /// <param name="count"></param>
        /// <param name="a1"></param>
        /// <param name="result"></param>
        /// <param name="r"></param>
        /// <param name="last"></param>
        /// <param name="SelectCount"></param>
        public void GetOptional(int count, List<ProductComboMap> a1, string result, ref string r, int last, int SelectCount)
        {
            if (result.Split(',').Length - 1 == SelectCount)
            {
                r += result + "&";
                return;
            }

            for (int flag = 0; flag < a1.Count; flag++)
            {
                if (result.IndexOf(a1[flag].item_id.ToString()) < 0 && (last < flag + count || count == 0))
                {
                    GetOptional(count + 1, a1, result + a1[flag].item_id + ",", ref r, flag + count, SelectCount);
                }
            }
        }

        /// <summary>
        /// 判斷對照中必選的是否都存在
        /// </summary>
        /// <param name="HasCheckStr">要進行篩選的字符串</param>
        /// <param name="whereStr">篩選條件</param>
        /// <returns></returns>
        public string CheckHas(string HasCheckStr, string whereStr)
        {
            int XhCount = 0;
            string StrRank = "";//篩選前的字符串
            string StrBefore = "";//篩選后的字符串
            for (int i = 0, j = HasCheckStr.Split('&').Length; i < j; i++)
            {
                StrRank = CommonFunction.Rank_ItemId(HasCheckStr.Split('&')[i].ToString());
                for (int m = 0, n = StrRank.Split(',').Length; m < n; m++)//以外面的組合為循環
                {
                    for (int k = 0, l = whereStr.Split(',').Length; k < l; k++)
                    {
                        if (StrRank.Split(',')[m] == whereStr.Split(',')[k])
                        {
                            XhCount++;
                        }
                    }
                }
                if (XhCount == whereStr.Split(',').Length)
                {
                    if (StrBefore != "")
                    {
                        StrBefore += "&";
                    }
                    StrBefore += StrRank;
                }
                XhCount = 0;
            }
            return StrBefore;
        }

        /// <summary>
        /// 固定組合商品查詢
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase FixedQuery()
        {
            string result = "{success:false}";
            try
            {
                uint pid = uint.Parse(Request.Params["ProductId"] ?? "0");
                uint channel_id = uint.Parse(Request.Params["cId"] ?? "0");
                uint pmId = uint.Parse(Request.Params["pmId"] ?? "0"); // edit by xiangwang 0413w 2014/07/02 增加站點價格price_master_id
                if (pid != 0)
                {
                    ProductItemMapMgr _mapMgr = new ProductItemMapMgr(connectionString);
                    _ProductItemMapExcelImplMgr = new ProductItemMapExcelMgr(connectionString);
                    List<BLL.gigade.Model.Custom.ProductMapCustom> resultList = _mapMgr.CombinationQuery(new ProductItemMapCustom { product_id = pid });
                    if (resultList.Count() > 0)
                    {
                        List<List<ProductComboMap>> itemAll = new List<List<ProductComboMap>>();
                        for (int i = 0, j = resultList.Count(); i < j; i++)
                        {
                            List<ProductComboMap> itemList = _mapMgr.ProductComboItemQuery(int.Parse(resultList[i].child_id.ToString()), int.Parse(pid.ToString()));

                            itemAll.Add(itemList);
                        }
                        string s = "";
                        getCombo(0, itemAll, "", ref s);
                        s = s.Substring(0, s.Length - 1);
                        string strb = "[";
                        if (!string.IsNullOrEmpty(s))
                        {
                            for (int Si = 0, Sj = s.Split('&').Length; Si < Sj; Si++)
                            {
                                ProductItemMap _productitemmap = new ProductItemMap();
                                _productitemmap.product_id = pid;
                                _productitemmap.channel_id = channel_id;
                                _productitemmap.price_master_id = pmId;//edit by xiangwang0413w 2014/07/02
                                _productitemmap.group_item_id = CommonFunction.Rank_ItemId(s.Split('&')[Si].ToString());
                                _productitemmap = _ProductItemMapExcelImplMgr.QueryProductItemMap(_productitemmap).FirstOrDefault();
                                strb += "{";
                                for (int Li = 0, Lj = s.Split('&')[Si].Split(',').Length; Li < Lj; Li++)
                                {
                                    uint ItemId = uint.Parse(s.Split('&')[Si].Split(',')[Li].ToString());
                                    string product_name = itemAll[Li].Where(e => e.item_id == ItemId).FirstOrDefault().product_name;
                                    string spec_name_1 = itemAll[Li].Where(e => e.item_id == ItemId).FirstOrDefault().spec_name_1;
                                    string spec_name_2 = itemAll[Li].Where(e => e.item_id == ItemId).FirstOrDefault().spec_name_2;
                                    string set_num = itemAll[Li].Where(e => e.item_id == ItemId).FirstOrDefault().set_num.ToString();
                                    strb += "item_id_" + (Li + 1).ToString() + ":" + ItemId.ToString() + ",product_name_" + (Li + 1).ToString() + ":'" + product_name + (spec_name_1 != "" ? "(" + spec_name_1 + " " + spec_name_2 + ")" : "") + "',product_num_" + (Li + 1).ToString() + ":" + set_num;
                                    if (Li != Lj - 1)
                                    {
                                        strb += ",";
                                    }
                                }
                                strb += ",group_item_id:'" + BLL.gigade.Common.CommonFunction.Rank_ItemId(s.Split('&')[Si].ToString()) + "'";
                                if (_productitemmap != null)
                                {
                                    strb += ",rid:" + _productitemmap.rid + ",product_name:'" + _productitemmap.product_name + "',channel_detail_id:'" + _productitemmap.channel_detail_id + "',product_cost:" + _productitemmap.product_cost + ",product_price:" + _productitemmap.product_price;
                                }
                                else
                                {
                                    strb += ",product_name:'',channel_detail_id:'',product_cost:'',product_price:''";
                                }
                                strb += "}";
                                if (Si != Sj - 1)
                                {
                                    strb += ",";
                                }
                            }
                        }
                        strb += "]";
                        result = "{success:true,data:" + strb + "}";
                    }
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
            this.Response.Write(result);
            this.Response.End();
            return this.Response;
        }

        /// <summary>
        /// 組合商品對照保存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase ComboCompareSave()
        {
            string result = "{success:false}";
            try
            {
                uint channelId = uint.Parse(Request.Params["ChannelId"]);
                string dataJson = Request.Params["dataJson"];

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<ProductItemMapCustom> resultList = js.Deserialize<List<ProductItemMapCustom>>(dataJson);
                if (resultList.Count > 0)
                {
                    foreach (var item in resultList)
                    {
                        List<ProductMapSet> mapset = js.Deserialize<List<ProductMapSet>>(item.strChild);
                        item.MapChild = mapset;
                    }
                    _ProductItemMapMgr = new ProductItemMapMgr(connectionString);
                    if (_ProductItemMapMgr.ComboCompareSave(resultList))
                    {
                        result = "{success:true}";
                    }
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
            this.Response.Write(result);
            this.Response.End();
            return this.Response;
        }

        /// <summary>
        /// 任選組合商品查詢
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase OptionalQuery()
        {
            string result = "{success:false}";
            try
            {
                uint pid = uint.Parse(Request.Params["ProductId"] ?? "0");
                uint channel_id = uint.Parse(Request.Params["cId"] ?? "0");
                uint pmId = uint.Parse(Request.Form["pmId"] ?? "0");
                string type = Request.Params["type"] ?? "";
                if (pid != 0)
                {
                    ProductMapSetMgr _mapsetMgr = new ProductMapSetMgr(connectionString);
                    ProductItemMapMgr _mapMgr = new ProductItemMapMgr(connectionString);
                    _ProductItemMapExcelImplMgr = new ProductItemMapExcelMgr(connectionString);
                    List<BLL.gigade.Model.Custom.ProductMapCustom> resultList = _mapMgr.CombinationQuery(new ProductItemMapCustom { product_id = pid });
                    //查詢是否已經建立過對照
                    List<BLL.gigade.Model.ProductMapSet> maplist = _mapsetMgr.Query(pid);
                    List<ProductComboMap> itemAll = new List<ProductComboMap>();
                    for (int i = 0, j = resultList.Count(); i < j; i++)
                    {
                        itemAll.Add(_mapMgr.ProductComboItemQuery(int.Parse(resultList[i].child_id.ToString()), int.Parse(pid.ToString())).FirstOrDefault());
                    }
                    //獲取任選中是否有必選單位 hufeng0813w
                    string str_must_itemId = "";
                    for (int i = 0, j = itemAll.Count; i < j; i++)
                    {
                        if (itemAll[i].set_num > 0)
                        {
                            if (str_must_itemId != "")
                            {
                                str_must_itemId += ",";
                            }
                            str_must_itemId += itemAll[i].item_id;
                        }
                    }
                    str_must_itemId = CommonFunction.Rank_ItemId(str_must_itemId);
                    string optional = "";
                    string StrOptional = "";

                    #region 僅限一單位勾選
                    if (resultList[0].buy_limit == 1)
                    {
                        //遞歸獲取所有的組合
                        GetOptional(0, itemAll, "", ref optional, 0, itemAll[0].g_must_buy);
                        optional = optional.Substring(0, optional.Length - 2).Replace(",&", "&");
                        //進行必選的篩選 hufeng0813w
                        if (str_must_itemId != "")
                        {
                            optional = CheckHas(optional, str_must_itemId);
                        }
                        StrOptional = "[";
                        if (!string.IsNullOrEmpty(optional))
                        {
                            for (int i = 0, Li = optional.Split('&').Length; i < Li; i++)
                            {
                                if (StrOptional != "" && i != 0)
                                {
                                    StrOptional += ",";
                                }
                                ProductItemMap _productitemmap = new ProductItemMap();
                                _productitemmap.product_id = pid;
                                _productitemmap.channel_id = channel_id;
                                _productitemmap.price_master_id = pmId;//edit by xiangwang0413w 2014/07/02
                                _productitemmap.group_item_id = CommonFunction.Rank_ItemId(optional.Split('&')[i].ToString());
                                _productitemmap = _ProductItemMapExcelImplMgr.QueryProductItemMap(_productitemmap).FirstOrDefault();
                                StrOptional += "{";
                                for (int j = 0, Lj = optional.Split('&')[i].Split(',').Length; j < Lj; j++)
                                {
                                    string product_name = itemAll.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j])).FirstOrDefault().product_name;
                                    string spec_name_1 = itemAll.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j])).FirstOrDefault().spec_name_1;
                                    string spec_name_2 = itemAll.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j])).FirstOrDefault().spec_name_2;
                                    //先判斷是否已經建立對照 
                                    string set_num = maplist.Count == 0 ? itemAll.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j])).FirstOrDefault().set_num.ToString()
                                        : (maplist.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j]) && e.map_rid == (_productitemmap == null ? 0 : _productitemmap.rid)).FirstOrDefault() == null ? itemAll.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j])).FirstOrDefault().set_num.ToString()
                                        : maplist.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j]) && e.map_rid == (_productitemmap == null ? 0 : _productitemmap.rid)).FirstOrDefault().set_num.ToString());
                                    string s_must_buy = itemAll.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j])).FirstOrDefault().set_num.ToString();//必選數量
                                    StrOptional += "item_id_" + (j + 1).ToString() + ":" + optional.Split('&')[i].Split(',')[j].ToString() + ",product_name_" + (j + 1).ToString() + ":'" + product_name + (spec_name_1 != "" ? "(" + spec_name_1 + " " + spec_name_2 + ")" : "") + "',product_num_" + (j + 1).ToString() + ":" + set_num + ",s_must_buy_" + (j + 1) + ":" + s_must_buy;
                                    if (j != Lj - 1)
                                    {
                                        StrOptional += ",";
                                    }
                                }

                                StrOptional += ",group_item_id:'" + BLL.gigade.Common.CommonFunction.Rank_ItemId(optional.Split('&')[i].ToString()) + "'";
                                if (_productitemmap != null)
                                {
                                    StrOptional += ",rid:" + _productitemmap.rid + ",product_name:'" + _productitemmap.product_name + "',channel_detail_id:'" + _productitemmap.channel_detail_id + "',product_cost:" + _productitemmap.product_cost + ",product_price:" + _productitemmap.product_price;
                                }
                                else
                                {
                                    StrOptional += ",product_name:'',channel_detail_id:'',product_cost:'',product_price:''";
                                }
                                StrOptional += "}";
                            }
                        }
                        StrOptional += "]";
                    }
                    #endregion

                    #region 僅限一單位不勾選
                    else
                    {
                        //不是必須所有item_id 都需要拼接上 add by hufeng0813w 2014/03/31
                        for (int i = 0, j = itemAll.Count; i < j; i++)
                        {
                            if (i != 0)
                            {
                                optional += ",";
                            }
                            optional += itemAll[i].item_id;
                        }
                        //end add by hufeng0813w 2014/03/31
                        ProductItemMap _pitemMap = new ProductItemMap();
                        _pitemMap.product_id = pid;
                        _pitemMap.channel_id = channel_id;
                        _pitemMap.price_master_id = pmId;//edit by xiangwang0413w 2014/07/02
                        List<ProductItemMap> _listPitemMap = _mapMgr.QueryOldItemMap(_pitemMap);

                        #region 查詢最初的商品信息
                        if (type == "searchInfo")
                        {
                            StrOptional = "[";
                            if (!string.IsNullOrEmpty(optional))
                            {
                                for (int i = 0, Li = optional.Split('&').Length; i < Li; i++)
                                {
                                    if (StrOptional != "" && i != 0)
                                    {
                                        StrOptional += ",";
                                    }
                                    ProductItemMap _productitemmap = new ProductItemMap();
                                    _productitemmap.product_id = pid;
                                    _productitemmap.channel_id = channel_id;
                                    _productitemmap.price_master_id = pmId;//edit by xiangwang0413w 2014/07/02
                                    _productitemmap.group_item_id = CommonFunction.Rank_ItemId(optional.Split('&')[i].ToString());
                                    _productitemmap = _ProductItemMapExcelImplMgr.QueryProductItemMap(_productitemmap).FirstOrDefault();
                                    StrOptional += "{";
                                    for (int j = 0, Lj = optional.Split('&')[i].Split(',').Length; j < Lj; j++)
                                    {
                                        string product_name = itemAll.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j])).FirstOrDefault().product_name;
                                        string spec_name_1 = itemAll.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j])).FirstOrDefault().spec_name_1;
                                        string spec_name_2 = itemAll.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j])).FirstOrDefault().spec_name_2;
                                        //先判斷是否已經建立對照 
                                        string set_num = maplist.Count == 0 ? itemAll.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j])).FirstOrDefault().set_num.ToString()
                                            : (maplist.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j]) && e.map_rid == (_productitemmap == null ? 0 : _productitemmap.rid)).FirstOrDefault() == null ? itemAll.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j])).FirstOrDefault().set_num.ToString()
                                            : maplist.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j]) && e.map_rid == (_productitemmap == null ? 0 : _productitemmap.rid)).FirstOrDefault().set_num.ToString());
                                        string s_must_buy = itemAll.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j])).FirstOrDefault().set_num.ToString();//必選數量
                                        StrOptional += "item_id_" + (j + 1).ToString() + ":" + optional.Split('&')[i].Split(',')[j].ToString() + ",product_name_" + (j + 1).ToString() + ":'" + product_name + (spec_name_1 != "" ? "(" + spec_name_1 + " " + spec_name_2 + ")" : "") + "',product_num_" + (j + 1).ToString() + ":" + set_num + ",s_must_buy_" + (j + 1) + ":" + s_must_buy;
                                        if (j != Lj - 1)
                                        {
                                            StrOptional += ",";
                                        }
                                    }

                                    StrOptional += ",group_item_id:'" + BLL.gigade.Common.CommonFunction.Rank_ItemId(optional.Split('&')[i].ToString()) + "'";
                                    if (_productitemmap != null)
                                    {
                                        StrOptional += ",rid:" + _productitemmap.rid + ",product_name:'" + _productitemmap.product_name + "',channel_detail_id:'" + _productitemmap.channel_detail_id + "',product_cost:" + _productitemmap.product_cost + ",product_price:" + _productitemmap.product_price;
                                    }
                                    else
                                    {
                                        StrOptional += ",product_name:'',channel_detail_id:'',product_cost:'',product_price:''";
                                    }
                                    StrOptional += "}";
                                }
                            }
                            StrOptional += "]";
                        }
                        #endregion

                        #region --建立過對照
                        else if (_listPitemMap.Count > 0)
                        {
                            StrOptional = "[";
                            for (int i = 0, j = _listPitemMap.Count; i < j; i++)
                            {
                                //對照下面的item_id
                                string list_groupItem = optional;
                                if (i != 0)
                                {
                                    StrOptional += ",";
                                }
                                StrOptional += "{";
                                for (int m = 0, Lj = list_groupItem.Split(',').Length; m < Lj; m++)
                                {
                                    string product_name = itemAll.Where(e => e.item_id == uint.Parse(list_groupItem.Split(',')[m])).FirstOrDefault().product_name;
                                    string spec_name_1 = itemAll.Where(e => e.item_id == uint.Parse(list_groupItem.Split(',')[m])).FirstOrDefault().spec_name_1;
                                    string spec_name_2 = itemAll.Where(e => e.item_id == uint.Parse(list_groupItem.Split(',')[m])).FirstOrDefault().spec_name_2;
                                    //先判斷是否已經建立對照 
                                    string set_num = _mapsetMgr.Query(_listPitemMap[i].rid, uint.Parse(list_groupItem.Split(',')[m])).FirstOrDefault() == null ? "0" : _mapsetMgr.Query(_listPitemMap[i].rid, uint.Parse(list_groupItem.Split(',')[m])).FirstOrDefault().set_num.ToString();
                                    string s_must_buy = itemAll.Where(e => e.item_id == uint.Parse(list_groupItem.Split(',')[m])).FirstOrDefault().set_num.ToString();//必選數量
                                    StrOptional += "item_id_" + (m + 1).ToString() + ":" + list_groupItem.Split(',')[m].ToString() + ",product_name_" + (m + 1).ToString() + ":'" + product_name + (spec_name_1 != "" ? "(" + spec_name_1 + " " + spec_name_2 + ")" : "") + "',product_num_" + (m + 1).ToString() + ":" + set_num + ",s_must_buy_" + (m + 1) + ":" + s_must_buy;
                                    if (m != Lj - 1)
                                    {
                                        StrOptional += ",";
                                    }
                                }

                                StrOptional += ",group_item_id:'" + list_groupItem + "'";
                                if (_listPitemMap != null)
                                {
                                    StrOptional += ",rid:" + _listPitemMap[i].rid + ",product_name:'" + _listPitemMap[i].product_name + "',channel_detail_id:'" + _listPitemMap[i].channel_detail_id + "',product_cost:" + _listPitemMap[i].product_cost + ",product_price:" + _listPitemMap[i].product_price;
                                }
                                else
                                {
                                    StrOptional += ",product_name:'',channel_detail_id:'',product_cost:'',product_price:''";
                                }
                                StrOptional += "}";
                            }
                            StrOptional += "]";
                        }
                        #endregion

                        #region --沒有建立過對照
                        else
                        {
                            StrOptional = "[";
                            if (!string.IsNullOrEmpty(optional))
                            {
                                for (int i = 0, Li = optional.Split('&').Length; i < Li; i++)
                                {
                                    if (StrOptional != "" && i != 0)
                                    {
                                        StrOptional += ",";
                                    }
                                    ProductItemMap _productitemmap = new ProductItemMap();
                                    _productitemmap.product_id = pid;
                                    _productitemmap.channel_id = channel_id;
                                    _productitemmap.price_master_id = pmId;//edit by xiangwang0413w 2014/07/02
                                    _productitemmap.group_item_id = CommonFunction.Rank_ItemId(optional.Split('&')[i].ToString());
                                    _productitemmap = _ProductItemMapExcelImplMgr.QueryProductItemMap(_productitemmap).FirstOrDefault();
                                    StrOptional += "{";
                                    for (int j = 0, Lj = optional.Split('&')[i].Split(',').Length; j < Lj; j++)
                                    {
                                        string product_name = itemAll.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j])).FirstOrDefault().product_name;
                                        string spec_name_1 = itemAll.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j])).FirstOrDefault().spec_name_1;
                                        string spec_name_2 = itemAll.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j])).FirstOrDefault().spec_name_2;
                                        //先判斷是否已經建立對照 
                                        string set_num = maplist.Count == 0 ? itemAll.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j])).FirstOrDefault().set_num.ToString()
                                            : (maplist.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j]) && e.map_rid == (_productitemmap == null ? 0 : _productitemmap.rid)).FirstOrDefault() == null ? itemAll.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j])).FirstOrDefault().set_num.ToString()
                                            : maplist.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j]) && e.map_rid == (_productitemmap == null ? 0 : _productitemmap.rid)).FirstOrDefault().set_num.ToString());
                                        string s_must_buy = itemAll.Where(e => e.item_id == uint.Parse(optional.Split('&')[i].Split(',')[j])).FirstOrDefault().set_num.ToString();//必選數量
                                        StrOptional += "item_id_" + (j + 1).ToString() + ":" + optional.Split('&')[i].Split(',')[j].ToString() + ",product_name_" + (j + 1).ToString() + ":'" + product_name + (spec_name_1 != "" ? "(" + spec_name_1 + " " + spec_name_2 + ")" : "") + "',product_num_" + (j + 1).ToString() + ":" + set_num + ",s_must_buy_" + (j + 1) + ":" + s_must_buy;
                                        if (j != Lj - 1)
                                        {
                                            StrOptional += ",";
                                        }
                                    }

                                    StrOptional += ",group_item_id:'" + BLL.gigade.Common.CommonFunction.Rank_ItemId(optional.Split('&')[i].ToString()) + "'";
                                    if (_productitemmap != null)
                                    {
                                        StrOptional += ",rid:" + _productitemmap.rid + ",product_name:'" + _productitemmap.product_name + "',channel_detail_id:'" + _productitemmap.channel_detail_id + "',product_cost:" + _productitemmap.product_cost + ",product_price:" + _productitemmap.product_price;
                                    }
                                    else
                                    {
                                        StrOptional += ",product_name:'',channel_detail_id:'',product_cost:'',product_price:''";
                                    }
                                    StrOptional += "}";
                                }
                            }
                            StrOptional += "]";
                        }
                        #endregion
                    }
                    #endregion
                    result = "{success:true,data:" + StrOptional + "}";
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
            this.Response.Write(result);
            this.Response.End();
            return this.Response;
        }

        /// <summary>
        /// 群組組合商品查詢
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GroupQuery()
        {
            string result = "{success:false}";
            try
            {
                uint pid = uint.Parse(Request.Params["ProductId"] ?? "0");
                uint channel_id = uint.Parse(Request.Params["cId"] ?? "0");
                uint pmId = uint.Parse(Request.Params["pmId"] ?? "0");
                ProductItemMapMgr _mapMgr = new ProductItemMapMgr(connectionString);
                _ProductItemMapExcelImplMgr = new ProductItemMapExcelMgr(connectionString);
                List<BLL.gigade.Model.Custom.ProductMapCustom> resultList = _mapMgr.CombinationQuery(new ProductItemMapCustom { product_id = pid });
                int pileCount = resultList.LastOrDefault().pile_id;
                if (resultList.Count() > 0)
                {
                    List<List<ProductComboMap>> itemAll = new List<List<ProductComboMap>>();
                    List<ProductComboMap> itemList = new List<ProductComboMap>();
                    for (int i = 0, j = resultList.Count(); i < j; i++)
                    {
                        itemList.Add(_mapMgr.ProductComboItemQuery(int.Parse(resultList[i].child_id.ToString()), int.Parse(pid.ToString())).FirstOrDefault());
                    }
                    for (int Pi = 1; Pi <= pileCount; Pi++)
                    {
                        itemAll.Add(itemList.Where(e => e.pile_id == Pi).ToList());
                    }
                    string s = "";
                    getCombo(0, itemAll, "", ref s);
                    s = s.Substring(0, s.Length - 1);
                    string strb = "[";
                    if (!string.IsNullOrEmpty(s))
                    {
                        for (int Si = 0, Sj = s.Split('&').Length; Si < Sj; Si++)
                        {
                            ProductItemMap _productitemmap = new ProductItemMap();
                            _productitemmap.product_id = pid;
                            _productitemmap.channel_id = channel_id;
                            _productitemmap.price_master_id = pmId;//edit by xiangwang0413w 2014/07/07
                            _productitemmap.group_item_id = CommonFunction.Rank_ItemId(s.Split('&')[Si].ToString());
                            _productitemmap = _ProductItemMapExcelImplMgr.QueryProductItemMap(_productitemmap).FirstOrDefault();
                            strb += "{";
                            for (int Li = 0, Lj = s.Split('&')[Si].Split(',').Length; Li < Lj; Li++)
                            {
                                uint ItemStr = uint.Parse(s.Split('&')[Si].Split(',')[Li].ToString());
                                string product_name = itemAll[Li].Where(e => e.item_id == ItemStr).FirstOrDefault().product_name;
                                string spec_name_1 = itemAll[Li].Where(e => e.item_id == ItemStr).FirstOrDefault().spec_name_1;
                                string spec_name_2 = itemAll[Li].Where(e => e.item_id == ItemStr).FirstOrDefault().spec_name_2;
                                string set_num = itemAll[Li].Where(e => e.item_id == ItemStr).FirstOrDefault().set_num.ToString();
                                strb += "item_id_" + (Li + 1).ToString() + ":" + s.Split('&')[Si].Split(',')[Li].ToString() + ",product_name_" + (Li + 1).ToString() + ":'" + product_name + (spec_name_1 != "" ? "(" + spec_name_1 + " " + spec_name_2 + ")" : "") + "',product_num_" + (Li + 1).ToString() + ":" + set_num;
                                if (Li != Lj - 1)
                                {
                                    strb += ",";
                                }
                            }
                            strb += ",group_item_id:'" + BLL.gigade.Common.CommonFunction.Rank_ItemId(s.Split('&')[Si].ToString()) + "'";
                            if (_productitemmap != null)
                            {
                                strb += ",rid:" + _productitemmap.rid + ",product_name:'" + _productitemmap.product_name + "',channel_detail_id:'" + _productitemmap.channel_detail_id + "',product_cost:" + _productitemmap.product_cost + ",product_price:" + _productitemmap.product_price;
                            }
                            else
                            {
                                strb += ",product_name:'',channel_detail_id:'',product_cost:'',product_price:''";
                            }
                            strb += "}";
                            if (Si != Sj - 1)
                            {
                                strb += ",";
                            }
                        }
                    }
                    strb += "]";
                    result = "{success:true,data:" + strb + "}";
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
            this.Response.Write(result);
            this.Response.End();
            return this.Response;
        }

        /// <summary>
        /// 查詢外站
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetOutSite()
        {
            try
            {
                _ProductItemMapMgr = new BLL.gigade.Mgr.ProductItemMapMgr(connectionString);
                jsonStr = "{success:true,data:" + JsonConvert.SerializeObject(_ProductItemMapMgr.QueryOutSite()) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }



            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;

        }

        /// <summary>
        /// 查詢站點價格信息
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetSitePriceOption(uint productId, uint channelId)
        {//addy by xiangwang0413w 2014/07/02
            try
            {
                _ProductItemMapMgr = new BLL.gigade.Mgr.ProductItemMapMgr(connectionString);
                var items = _ProductItemMapMgr.QuerySitePriceOption(productId, channelId);
                jsonStr = new JObject(
                    new JProperty("success", true),
                    new JProperty("data",
                        new JArray(//创建数据数组
                            from p in items
                            select new JObject(
                                    new JProperty("price_master_id", p.price_master_id),
                                    new JProperty("site_price_option", p.site_name.Replace('1','*').Replace('0',' ') + "," + p.user_level_name + "," + (string.IsNullOrEmpty(p.user_email) ? "無" : p.user_email))
                                )
                            ))).ToString();//创建json字符串
            }
            catch(Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }

        /// <summary>
        /// 單一商品對照保存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase SaveProductItem()
        {
            string result = "{success:true}";
            if (!string.IsNullOrEmpty(Request.Params["jsonSave"]))
            {
                try
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    List<ProductItemMap> saveList = js.Deserialize<List<ProductItemMap>>(Request.Params["jsonSave"]);
                    _ProductItemMapMgr = new ProductItemMapMgr(connectionString);
                    if (!_ProductItemMapMgr.SingleCompareSave(saveList))
                    {
                        result = "{success:false}";
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

            Response.Clear();
            Response.Write(result);
            Response.End();
            return this.Response;
        }

        /// <summary>
        /// 刪除單一商品對照
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public string DeleteProductMap()
        {
            ProductItemMap p = new ProductItemMap();
            p.rid = UInt32.Parse(Request.Form["rid"]);
            p.channel_detail_id = Request.Form["pno"];
            p.channel_id = UInt32.Parse(Request.Form["cid"]);
            _ProductItemMapMgr = new ProductItemMapMgr(connectionString);
            int rows = _ProductItemMapMgr.Delete(p);
            if (rows == -1)
            {
                jsonStr = "{success:false,rpackCode:'" + Resources.ProductItemMap.EXIST_DELETE_FAIL + "'}";
            }
            else
            {
                jsonStr = "{success:true,rpackCode:'" + Resources.ProductItemMap.DELETE_SUCCESS + "'}";
            }
            return jsonStr;
        }


        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase ComboMapDelele()
        {
            string jsonResult = "{success:false}";
            uint c_rid = 0;
            uint.TryParse(Request.Params["rid"] ?? "0", out c_rid);
            _ProductItemMapMgr = new ProductItemMapMgr(connectionString);
            bool result = _ProductItemMapMgr.ComboDelete(new ProductItemMap { rid = c_rid });
            if (result)
            {
                jsonResult = "{success:true}";
            }
            this.Response.Clear();
            this.Response.Write(jsonResult);
            this.Response.End();
            return this.Response;
        }

        /// <summary>
        /// 對照上傳
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public string DoUpload()
        {
            int channel_id = int.Parse(Request.Form["w_comboxOutSite"]);
            string fileName = Request.Files[0].FileName;
            string filePath = "";
            string fName = "";
            string fExtension = "";
            try
            {
                FileOperator fo = new FileOperator(fileName);
                fExtension = fo.Extension;
                if (fExtension != ".xls" && fExtension != ".xlsx")
                {
                    return "{success:false,msg:'" + Resources.ProductItemMap.EXCEL_FORMAT_ERROR + "'}";
                }

                if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
                }

                fName = DateTime.Now.ToString("yyyyMMddhhmmss") + fo.Extension;
                filePath = Server.MapPath(excelPath) + "/" + fName;

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return "{success:false,msg:'" + Resources.ProductItemMap.EXCEL_FORMAT_ERROR + "'}";
            }

            try
            {
                Request.Files[0].SaveAs(filePath);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            IProductItemMapExcelImplMgr ife = new ProductItemMapExcelMgr(connectionString);
            List<ProductItemMapCustom> pm = new List<ProductItemMapCustom>();
            try
            {
                List<ProductItemMapCustom> errorPm = new List<ProductItemMapCustom>();
                Resource.CoreMessage = new CoreResource("ProductItemMap");
                bool isHeaderError = false;
                //讀取excel裡賣弄的內容
                pm = ife.ReadFile(filePath, channel_id, fExtension, ref isHeaderError, errorPm);
                if (!isHeaderError)
                {
                    if (errorPm.Count == 0)
                    {
                        ife.SaveToDB(pm);
                    }
                    else
                    {
                        return "{success:true,error:true,path:'" + fName + "',data:" + JsonConvert.SerializeObject(errorPm) + "}";
                    }
                }
                else
                {
                    return "{success:true,headerError:true}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return "{success:false,msg:'" + Resources.ProductItemMap.IMPORT_FAIL + "'}";
            }
            return "{success:true,msg:'" + string.Format(Resources.ProductItemMap.IMPORT_SUCCESS_TOTAL, pm.Count) + "'}";
        }
        #region 賣場對照批次修改價格商品名稱 +EditUpload() 
        /// <summary>
        /// 賣場對照批次修改價格商品名稱 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public string EditUpload()
        {
            ProductItemMap m = new ProductItemMap();
            int channel_id = int.Parse(Request.Form["channel"]);
            int Ins = 0; int Upd = 0;
            #region 讀取excel文件
            string fileName = Request.Files[0].FileName;
            string filePath = "";
            string fName = "";
            string fExtension = "";
            try
            {
                FileOperator fo = new FileOperator(fileName);
                fExtension = fo.Extension;
                if (fExtension != ".xls" && fExtension != ".xlsx")
                {
                    return "{success:false,msg:'" + Resources.ProductItemMap.EXCEL_FORMAT_ERROR + "'}";
                }
                if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
                }
                fName = DateTime.Now.ToString("yyyyMMddhhmmss") + fo.Extension;
                filePath = Server.MapPath(excelPath) + "/" + fName;
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return "{success:false,msg:'" + Resources.ProductItemMap.EXCEL_FORMAT_ERROR + "'}";
            }
            try
            {
                Request.Files[0].SaveAs(filePath);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            #endregion
            try
            {
                Resource.CoreMessage = new CoreResource("ProductItemMap");
                //讀取excel裡賣弄的內容
                NPOI4ExcelHelper fm = new NPOI4ExcelHelper(filePath);
                _ProductItemMapMgr = new ProductItemMapMgr(connectionString);
                DataTable dt = fm.SheetData();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (_ProductItemMapMgr.Selrepeat(dt.Rows[i]["外站商品編號"].ToString()) > 0)
                        {//編輯
                            try
                            {
                                m.channel_detail_id = dt.Rows[i]["外站商品編號"].ToString();
                                m.product_name = dt.Rows[i]["外站商品名稱"].ToString();
                                m.product_price = Int32.Parse(dt.Rows[i]["外站商品售價"].ToString());
                                _ProductItemMapMgr.UpdatePIM(m);
                                Upd++;
                            }
                            catch (Exception ex)
                            {
                                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                log.Error(logMessage);
                            }
                        }
                        else
                        {//保存 未知price_master_id
                            try
                            {
                                m.channel_id = UInt32.Parse(channel_id.ToString());
                                m.channel_detail_id = dt.Rows[i]["外站商品編號"].ToString();
                                m.product_name = dt.Rows[i]["外站商品名稱"].ToString();
                                try
                                {
                                    m.product_id = UInt32.Parse(dt.Rows[i]["商品編號(5碼)"].ToString());
                                }
                                catch
                                {
                                    m.product_id = 0;
                                }
                                try
                                {
                                    m.item_id = UInt32.Parse(dt.Rows[i]["商品細項編號(6碼)"].ToString());
                                }
                                catch
                                {
                                    m.item_id = 0;
                                }
                                //m.product_cost = Int32.Parse(dt.Rows[i]["外站商品成本"].ToString());
                                try
                                {
                                    m.product_price = Int32.Parse(dt.Rows[i]["外站商品售價"].ToString());
                                }
                                catch
                                {
                                    m.product_price = 0;
                                }
                                _ProductItemMapMgr.Save(m);
                                Ins++;
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
                    return "{success:true,msg:'新增成功" + Ins + "條,更新成功" + Upd + "條'}";
                }
                else
                    return "{success:false,msg:'Excel no data'}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return "{success:false,msg:'" + Resources.ProductItemMap.IMPORT_FAIL + "'}";
            }
            //return "{success:true,msg:'" + string.Format(Resources.ProductItemMap.IMPORT_SUCCESS_TOTAL, Ins+Upd) + "'}";
        } 
        #endregion

        #region 對照繼續上傳 +ContinueUpload()
        /// <summary>
        /// 對照繼續上傳
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase ContinueUpload()
        {
            string fileName = Request.Params["fname"];
            int channel_id = int.Parse(Request.Params["outSite"]);
            jsonStr = "{success:false,msg:'" + Resources.ProductItemMap.IMPORT_FAIL + "'}";
            IProductItemMapExcelImplMgr ife = new ProductItemMapExcelMgr(connectionString);
            List<ProductItemMapCustom> pm = new List<ProductItemMapCustom>();

            string filePath = Server.MapPath(excelPath) + "/" + fileName;
            string fExtension = "." + fileName.Split('.')[1];
            try
            {
                List<ProductItemMapCustom> errorPm = new List<ProductItemMapCustom>();
                bool isHeader = true;
                pm = ife.ReadFile(filePath, channel_id, fExtension, ref isHeader, errorPm);
                if (pm.Count > 0)
                {
                    ife.SaveToDB(pm);
                }

                jsonStr = "{success:true,msg:'" + string.Format(Resources.ProductItemMap.IMPORT_SUCCESS_TOTAL, pm.Count) + "',data:" + JsonConvert.SerializeObject(pm) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);

            }

            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        } 
        #endregion

        #region 同一外站下存在相同賣場商品ID +HttpResponseBase HasRepeartSql()
        /// <summary>
        /// 同一外站下存在相同賣場商品ID
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase HasRepeartSql()
        {
            string result = "{success:false}";
            try
            {
                uint channelId = uint.Parse(Request.Params["ChannelId"]);
                string dataJson = Request.Params["dataJson"];
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<ProductItemMap> resultList = js.Deserialize<List<ProductItemMap>>(dataJson);
                string condition = "(";
                for (int i = 0, j = resultList.Count; i < j; i++)
                {
                    if ((condition != "(" && resultList[i].rid == 0) || (condition != "(" && resultList[i].msg.ToLower() == "true"))
                    {
                        condition += ",";
                    }
                    if (resultList[i].rid == 0 || resultList[i].msg.ToLower() == "true")
                    {
                        condition += "'" + resultList[i].channel_detail_id + "'";
                    }
                }
                condition += ")";
                if (condition != "()")
                {
                    _ProductItemMapMgr = new ProductItemMapMgr(connectionString);
                    List<ProductItemMap> p = _ProductItemMapMgr.QueryAll(channelId, condition);
                    if (p.Count == 0)
                    {
                        result = "{success:true}";
                    }
                    else
                    {
                        string data = "";
                        for (int i = 0, j = p.Count; i < j; i++)
                        {
                            if (i > 0)
                            {
                                data += ",";
                            }
                            data += p[i].channel_detail_id;
                        }
                        data += "<br />賣場商品編號已經存在!";

                        result = "{success:false,data:'" + data + "'}";
                    }
                }
                else
                {
                    result = "{success:true}";
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
            this.Response.Write(result);
            this.Response.End();
            return this.Response;
        } 
        #endregion

        #region 匯出賣場商品對照+void OutExcel()
        /// <summary>
        /// 匯出賣場商品對照（payeasy）
        /// </summary>
        public void OutExcel()
        {
            ProductItemMap m = new ProductItemMap();
            string fileName = "ProductMap_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".xls";
            DataTable new_Table = new DataTable();
            _ProductItemMapMgr = new ProductItemMapMgr(connectionString);
            try
            {
                try
                {
                    m.rid = UInt32.Parse(Request.Params["rid"]);
                }
                catch (Exception)
                {
                    m.rid = 0;
                }
                try
                {
                    m.channel_id = UInt32.Parse(Request.Params["channelid"]);
                }
                catch (Exception)
                {
                    m.channel_id = 0;
                }
                new_Table = _ProductItemMapMgr.ProductItemMapExc(m);
                MemoryStream ms = ExcelHelperXhf.ExportDT(new_Table, "");
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(fileName));
                Response.BinaryWrite(ms.ToArray());
                //Response.Clear();
                //Response.End();
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }         
        #endregion


    }
}
