using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using Newtonsoft.Json;
using BLL.gigade.Common;
using Newtonsoft.Json.Converters;
using System.Collections;
using BLL.gigade.Dao;
using System.IO;



namespace Admin.gigade.Controllers
{
    public class ProductParticularsController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        private IProductExtImplMgr _productExtMgr;
        private IProdNameExtendImplMgr _prodnameExtendMgr;
        //private ITableHistoryImplMgr _tableHistoryMgr; //用於歷史記錄保存的類
        //private IFunctionImplMgr _functionMgr; //用於找到控件的function信息
        //add by zhuoqin0830w  2015/05/19  添加 有效期限的 接口
        private IParticularsSrcImplMgr _particularsSrcMgr;
        //
        // GET: /ProductParticulars/

        #region 商品細項管理
        /// <summary>
        /// 商品細項管理
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 查詢商品細項詳情
        /// </summary>
        /// <returns></returns>
        public ActionResult QueryParticulars(int condition, string ids)
        {
            int[] id = (from i in ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) select int.Parse(i)).ToArray();
            try
            {
                _productExtMgr = new ProductExtMgr(connectionString);
                return Json(_productExtMgr.Query((ProductExtCustom.Condition)condition, id));
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return new EmptyResult();
            }
        }

        /// <summary>
        /// 修改or新增
        /// </summary>
        /// <param name="sc"></param>
        /// <returns></returns>
        public ActionResult SaveParticulars(string particulars)
        {
            bool result = false;
            try
            {
                _productExtMgr = new ProductExtMgr(connectionString);
                List<ProductExtCustom> lists = JsonConvert.DeserializeObject<List<ProductExtCustom>>(particulars);
                Caller _caller = (Session["caller"] as Caller);
                string controlId = Request["eventUpdate"];
                if (_caller == null)
                {
                    throw new Exception("session is overdue!");
                }

                result = _productExtMgr.UpdateProductExt(lists, _caller, controlId);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return Json(new { success = result });
        }
        #endregion

        #region 商品名稱管理
        /// <summary>
        /// 商品名稱管理
        /// </summary>
        /// <returns></returns>
        public ActionResult ProdnameExtend()
        {
            return View();
        }

        /// <summary>
        /// 查詢前後綴
        /// </summary>
        /// <param name="pron"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult QueryProdname(ProdNameExtendCustom pron, bool isOverdue)
        {
            string ids = Request["Ids"];
            try
            {
                int state = 0;//判斷是否需要重新查詢的依據
                _prodnameExtendMgr = new ProdNameExtendMgr(connectionString);
                IPriceMasterImplMgr pmMgr = new PriceMasterMgr(connectionString);
                MySqlDao _mySqlDao = new MySqlDao(connectionString);
                ArrayList sqlList = new ArrayList();
                List<ProdNameExtendCustom> list = _prodnameExtendMgr.Query(pron, ids);//根據條件查詢商品信息
                //判斷過期商品 和 審核商品 中是否存在錯誤數據  edit by zhuoqin0830w  2015/05/07
                List<ProdNameExtendCustom> listDelete = list.FindAll(m => m.Flag == 3 || m.Flag == 1); //查詢過程中,為防止未去掉前後綴,卻到期的商品,進行一次遍歷,去掉前後綴 edit by wwei 0216w 2014/12/22
                //獲取 前一天 時間的時間戳  edit by zhuoqin0830w  2015/05/07
                long time = CommonFunction.GetPHPTime(Convert.ToString(DateTime.Now)) - 86400;
                foreach (ProdNameExtendCustom pec in listDelete)
                {
                    pec.Product_Name = pec.Product_Name.Replace(PriceMaster.L_HKH + pec.Product_Prefix + PriceMaster.R_HKH, "").Replace(PriceMaster.L_HKH + pec.Product_Suffix + PriceMaster.R_HKH, "");
                    PriceMaster p = new PriceMaster();
                    p.price_master_id = pec.Price_Master_Id;
                    p.product_name = pec.Product_Name;
                    switch (pec.Flag)
                    {
                        case 1://審核商品
                            //判斷結束時間 是否 小於 前一天 時間的時間戳  edit by zhuoqin0830w  2015/05/07
                            if (pec.Event_End <= time)
                            {
                                sqlList.Add(pmMgr.UpdateName(p));
                                state = 1;//如果審核商品中結束時間小於前一天的時間，則賦值為1;
                                _mySqlDao.ExcuteSqls(sqlList);
                                break;
                            }
                            break;
                        case 3://過期商品
                            //如果在過期的商品名稱中能找到前綴後綴~則調用刪除前後綴的方法
                            if ((pec.Product_Name.Contains(PriceMaster.L_HKH + pec.Product_Prefix + PriceMaster.R_HKH) || pec.Product_Name.Contains(PriceMaster.L_HKH + pec.Product_Suffix + PriceMaster.R_HKH)))
                            {
                                _prodnameExtendMgr.ResetExtendName(new Caller());
                                state = 1;//如果有過期商品包含前後綴,則賦值為1;
                                break;
                            }
                            break;
                    }
                }
                if (state == 1)
                {
                    list = _prodnameExtendMgr.Query(pron, ids);//替換掉之前包含前後綴的商品名稱
                }

                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Key_Id = i;
                }//add by wwei0216w 2014/12/30 為key_id賦值

                if (isOverdue)//查詢過期
                {
                    list.RemoveAll(p => p.Flag != 3);
                }
                else //查詢未過期
                {
                    list.RemoveAll(p => p.Flag == 3);
                }
                return Json(new { item = list });
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return Json(new List<ProdNameExtendCustom>());
        }

        /// <summary>
        /// 修改or新增
        /// </summary>
        /// <param name="sc"></param>
        /// <returns></returns>
        public ActionResult SaveProdname(string prodNameExtend)
        {
            bool result = false;
            _prodnameExtendMgr = new ProdNameExtendMgr(connectionString);
            try
            {
                Caller caller = Session["caller"] as Caller;
                DateTime now = DateTime.Now;
                List<ProdNameExtendCustom> list = JsonConvert.DeserializeObject<List<ProdNameExtendCustom>>(prodNameExtend);//將前臺信息序列化

                foreach (var prod in list)
                {
                    if (!PriceMaster.CheckProdName(prod.Product_Prefix) || !PriceMaster.CheckProdName(prod.Product_Suffix)) //判斷添加的前後綴是否是"〖","〗"這種類型
                    {
                        return Json(new { success = false, msg = Resources.Product.FORBIDDEN_CHARACTER });
                    }
                    prod.Kuser = caller.user_email;
                    prod.Kdate = now;
                }
                result = _prodnameExtendMgr.SaveByList(list, caller);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return Json(new { success = result });
        }
        #endregion

        #region 有效期限對照表  add by  zhuoqin0830w  創建日期：2015/05/19  修改日期：2015/09/29  修改原因：將使用 xml 資料庫 改為使用 db 數據庫

        #region 獲取 ParticularsSrc 的信息  + QueryParticularsSrc()
        /// <summary>
        /// 獲取 ParticularsSrc 的信息
        /// </summary>
        /// <returns></returns>
        public ActionResult QueryParticularsSrc()
        {
            JsonResult json = null;
            //對 db 數據庫 和 xml 資料庫 進行區分  其中 particularsSrcType = 1 表示使用 db 數據庫  particularsSrcType = 2 表示 使用 xml 資料庫
            int particularsSrcType = 1;  //人為 定義
            try
            {
                _particularsSrcMgr = new ParticularsSrcMgr(connectionString, particularsSrcType);
                switch (particularsSrcType)
                {
                    case 1: //使用 db 數據庫
                        json = Json(_particularsSrcMgr.GetParticularsSrc());
                        break;
                    case 2://使用 xml 資料庫
                        string path = Server.MapPath("../XML/ParticularsSrc.xml");
                        if (System.IO.File.Exists(path))
                        {
                            json = Json(_particularsSrcMgr.GetParticularsSrc());
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return json;
        }
        #endregion

        #region 新增 和 修改 ParticularsSrc 里的 相關信息  + SaveNode(string particularsNode)
        /// <summary>
        /// 新增 和 修改 ParticularsSrc 里的 相關信息
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveNode(string particularsNode)
        {
            bool result = false;
            int particularsSrcType = 1;  //人為 定義
            try
            {
                List<ParticularsSrc> particularsSrc = JsonConvert.DeserializeObject<List<ParticularsSrc>>(particularsNode);//將前臺信息序列化
                _particularsSrcMgr = new ParticularsSrcMgr(connectionString, particularsSrcType);
                switch (particularsSrcType)
                {
                    case 1: //使用 db 數據庫
                        result = _particularsSrcMgr.SaveNode(particularsSrc, connectionString);
                        break;
                    case 2://使用 xml 資料庫
                        string path = Server.MapPath("../XML/ParticularsSrc.xml");
                        if (System.IO.File.Exists(path))
                        {
                            result = _particularsSrcMgr.SaveNode(particularsSrc, connectionString);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return Json(new { success = result });
        }
        #endregion

        #region  刪除 ParticularsSrc 里的 相關信息  + DeleteNode()
        /// <summary>
        /// 刪除 ParticularsSrc  里的 相關信息
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteNode()
        {
            bool result = false;
            int particularsSrcType = 1;  //人為 定義
            try
            {
                string particularsName = particularsSrcType == 1 ? Request.Form["Rowid"].Trim(',') : Request.Form["particularsName"].Trim(',');
                _particularsSrcMgr = new ParticularsSrcMgr(connectionString, particularsSrcType);

                if (particularsName.Length != 0)
                {
                    switch (particularsSrcType)
                    {
                        case 1: //使用 db 數據庫
                            result = _particularsSrcMgr.DeleteNode(particularsName);
                            break;
                        case 2://使用 xml 資料庫
                            string path = Server.MapPath("../XML/ParticularsSrc.xml");
                            if (System.IO.File.Exists(path))
                            {
                                result = _particularsSrcMgr.DeleteNode(particularsName);
                            }
                            break;
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
            return Json(new { success = result });
        }
        #endregion
        #endregion

    }
}