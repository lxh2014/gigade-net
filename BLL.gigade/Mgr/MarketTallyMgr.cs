using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class MarketTallyMgr
    {
        private IinvdImplDao _ivddao;
        private IAseldImplDao _aseldDao;
        private IIialgImplDao _iagDao;
        private IinvdImplDao _iinvdDao;
        
        private BLL.gigade.Mgr.Impl.IProductItemImplMgr _proditemMgr;
        private string mySqlConnectionString;
        public MarketTallyMgr(string connectionString)
        {
            this.mySqlConnectionString = connectionString;
            _ivddao = new IinvdDao(connectionString);
            _aseldDao = new AseldDao(connectionString);
        }
        public bool RFAutoMarketTally(int seld_id)
        {
            int totalCount = 0;
            List<string> iinvd;
            List<string> pick;
            bool markettally = false;
            try
            {
                //根據seld_id獲取aseld記錄信息；
                AseldQuery query_ase = new AseldQuery();
                query_ase.seld_id = seld_id;
                List<AseldQuery> ase = _aseldDao.GetAllAseldList(query_ase, out totalCount);
                if (ase.Count == 1)
                {
                }
                else
                {
                    return false;
                }

                //根據item_id獲取庫存信息；
                IinvdQuery query_ivd = new IinvdQuery()
                {
                    ista_id = "A"
                };
                query_ivd.item_id = ase[0].item_id;
                int out_qty = ase[0].out_qty;
                if (out_qty < 0)
                {
                    return false;
                }
                List<IinvdQuery> store_ivd = GetIinvdListByItemid(query_ivd, out totalCount);
                int stocksum = GetStockSum(store_ivd, out_qty, out iinvd, out pick);
                //if (ase[0].sel_loc == "YY999999")
                //{
                //    //沒有主料位,記庫存帳卡
                //    //結單
                //    bool markettally = GETMarkTallyWD(ase[0], stocksum, iinvd.ToArray(), pick.ToArray());
                //}
                if (stocksum < out_qty)
                {
                    //庫調
                    int result = RFKT(ase[0], store_ivd[0], out_qty - stocksum + store_ivd[0].prod_qty);
                    if (result == 100)
                    {
                        //刷新庫存信息
                        store_ivd = GetIinvdListByItemid(query_ivd, out totalCount);
                        stocksum = GetStockSum(store_ivd, out_qty, out iinvd, out pick);
                        if (stocksum <= out_qty)
                        {
                            //結單
                            markettally = GETMarkTallyWD(ase[0], stocksum, iinvd.ToArray(), pick.ToArray());
                        }
                        else
                        {
                            markettally = false;
                        }
                    }
                    else
                    {
                        //庫調失敗
                        markettally = false;
                    }
                }
                else if (stocksum >= out_qty)
                {
                    //結單
                    markettally = GETMarkTallyWD(ase[0], out_qty, iinvd.ToArray(), pick.ToArray());
                }
                else
                {
                    markettally = false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("MarketTallyMgr-->RFAutoMarketTally" + ex.Message, ex);
            }

            return markettally;
        }
        /// <summary>
        /// 統計庫存是否夠揀;如果不夠嘗試庫調；
        /// </summary>
        /// <returns></returns>
        private int GetStockSum(List<IinvdQuery> store_ivd, int out_qty, out List<string> iinvd, out List<string> pick)
        {
            int sum = 0;
            try
            {
                iinvd = new List<string>();
                pick = new List<string>();
                foreach (IinvdQuery item in store_ivd)
                {
                    if (sum <= out_qty)
                    {
                        if (sum + item.prod_qty <= out_qty)
                        {
                            iinvd.Add(item.row_id.ToString());
                            pick.Add(item.prod_qty.ToString());
                        }
                        else
                        {
                            iinvd.Add(item.row_id.ToString());
                            pick.Add((out_qty - sum).ToString());
                        }
                    }
                    sum += item.prod_qty;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("MarketTallyMgr-->GetStockSum" + ex.Message, ex);
            }

            return sum;
        }
        private List<IinvdQuery> GetIinvdListByItemid(IinvdQuery query_ivd, out int totalCount)
        {
            try
            {
                List<IinvdQuery> store_ivd = _ivddao.GetIinvdListByItemid(query_ivd, out totalCount);
                if (store_ivd.Count == 0)
                {
                    IinvdQuery m = new IinvdQuery();
                    m.prod_qty = 0;
                    m.made_date = DateTime.Now;
                    m.cde_dt = DateTime.Now;
                    store_ivd.Add(m);
                }
                return store_ivd;
            }
            catch (Exception ex)
            {
                throw new Exception("MarketTallyMgr-->GetIinvdListByItemid" + ex.Message, ex);
            }
        }
        public int RFKT(AseldQuery ase, IinvdQuery ivd,int pnum)
        {
            string json = string.Empty;
            int result = 0;
            IialgQuery q = new IialgQuery();
            uint id = 0; DateTime dt = new DateTime(); int sun = 0;
            _proditemMgr = new ProductItemMgr(mySqlConnectionString);
            ProductItem Proitems = new ProductItem();

            try
            {
                {//商品id
                    q.item_id = ase.item_id;
                    Proitems.Item_Id = ase.item_id;
                }
                // (DateTime.TryParse(Request.Params["made_date"].ToString(), out dt))
                {//商品製造日期
                    //q.made_dt = ivd.made_date;
                    q.made_dt = new DateTime(3000,1,1);
                }
                //if (int.TryParse(Request.Params["prod_qty"].ToString(), out sun))
                {//商品原有數量
                    q.qty_o = ivd.prod_qty;
                }
                //if (int.TryParse(Request.Params["pnum"].ToString(), out sun))
                {//商品撿貨數量
                    q.pnum = pnum;
                }
                //if (!string.IsNullOrEmpty(Request.Params["loc_id"].ToString()))
                {//商品撿貨數量
                    q.loc_id = ase.sel_loc;
                }
                //if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                {
                    q.order_id = ase.ord_id.ToString();
                }
                q.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                //進行庫調
                _iagDao = new IialgDao(mySqlConnectionString);
                Caller call = new Caller();
                call = (System.Web.HttpContext.Current.Session["caller"] as Caller);
                string path = "/WareHouse/KutiaoAddorReduce";
                //if (q.loc_id == "YY999999") 無主料位時也進行庫調
                if (false)
                {
                    json = "{success:false}";
                }
                else
                {
                    Proitems.Item_Stock = q.pnum - q.qty_o;
                    result = _iagDao.addIialgIstock_AutoMarket(q);
                    if (result == 2)
                    {
                        json = "{success:true,msg:2}";
                    }
                    if (result == 100)
                    {
                        //_proditemMgr.UpdateItemStock(Proitems, path, call);
                        json = "{success:true,msg:100}";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("MarketTallyMgr-->RFKT-->" + ex.Message, ex);
            }
            return result;
        }
        public bool GETMarkTallyWD(AseldQuery ase, int act_pick_qty,string[] iinvd,string[] pick)
        {
            ////
            StringBuilder sb = new StringBuilder();
            string json = String.Empty;
            AseldQuery m = new AseldQuery();
            List<AseldQuery> list = new List<AseldQuery>();
            _aseldDao = new AseldDao(mySqlConnectionString);
            int flag = 2;
            int try1 = 0;
            try
            {
                m.seld_id = ase.seld_id;//aseld的流水號
                m.commodity_type = "2";//獲取寄倉2和調度3
                m.ord_qty = ase.ord_qty;//需要訂貨數量
                m.act_pick_qty = act_pick_qty;
                
                m.item_id = ase.item_id;
                m.out_qty = ase.out_qty - m.act_pick_qty;//缺貨數量
                m.act_pick_qty = m.ord_qty - m.out_qty;
                m.complete_dtim = DateTime.Now;
                m.assg_id = ase.assg_id;
                m.ord_id = ase.ord_id;
                m.ordd_id = ase.ordd_id;//商品細項編號。操作iwms_record需要
                m.change_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                m.deliver_code = ase.deliver_code;
                m.deliver_id = int.Parse(m.deliver_code.Substring(1, m.deliver_code.Length - 1).ToString());
                if (m.out_qty == 0)
                {//揀完了,判斷缺貨數量是否為0
                    m.wust_id = "COM";
                }
                else
                {//沒拿夠貨物
                    m.wust_id = "SKP";
                }
                sb.Append(_aseldDao.UpdAseld(m));
                if (m.commodity_type == "2")
                {
                    #region 寄倉--對庫存進行操作
                    Dictionary<string, string> dickuCun = new Dictionary<string, string>();
                    //if (Int32.TryParse(Request.Params["act_pick_qty"], out try1))
                    {
                        //string[] iinvd = Request.Params["pickRowId"].Split(',');
                        //string[] pick = Request.Params["pickInfo"].Split(',');
                        for (int i = 0; i < iinvd.Length; i++)
                        {
                            if (!dickuCun.Keys.Contains(iinvd[i]))
                            {
                                dickuCun.Add(iinvd[i], pick[i]);
                            }
                            else
                            {
                                dickuCun[iinvd[i]] = pick[i];
                            }
                        }
                    }
                    _iinvdDao = new IinvdDao(mySqlConnectionString);
                    if (!string.IsNullOrEmpty(_iinvdDao.updgry(m, dickuCun)))
                    {
                        sb.Append(_iinvdDao.updgry(m, dickuCun));
                    }
                    if (!string.IsNullOrEmpty(sb.ToString()))
                    {
                        _aseldDao.InsertSql(sb.ToString());//執行SQL語句裡面有事物處理
                    }
                    int ord = 1;
                    int can = 0;
                    #region  判斷項目狀態
                    if (_aseldDao.SelCom(m) == 0)
                    {
                        ord = 0;//訂單揀貨完成，可以封箱
                    }
                    if (_aseldDao.SelComA(m) == 0)
                    {
                        flag = 0;//項目訂單揀貨完成
                    }
                    if (ord == 0)
                    {//有沒有臨時取消的商品
                        if (_aseldDao.SelComC(m) > 0)
                        {
                            can = 1;
                        }
                    }
                    #endregion
                    json = "{success:true,qty:'" + m.out_qty + "',flag:'" + flag + "',ord:'" + ord + "',can:'" + can + "'}";//返回json數據  
                    //qty 該物品是否缺貨，如果為零揀貨完成，否則彈框提示缺貨數量。
                    //over：0表示該訂單已經揀貨完畢，如果qty為零則提示該訂單可以封箱，qty不為零則提示該訂單還缺物品的數量。不為零則不提示任何信息。
                    #endregion
                }
                
                //else if (m.commodity_type == "3")
                //{
                //    #region 調度--對庫存進行操作
                //    m.change_user = int.Parse((Session["caller"] as Caller).user_id.ToString());//操作iwms_record 需要插入create_uaer_id。对aseld中的change_user未做任何改变
                //    m.act_pick_qty = Int32.Parse(Request.Params["act_pick_qty"]);//下一步插入檢貨記錄表，每檢一次記錄一次，實際撿貨量以傳過來的值為標準
                //    if (_iasdMgr.getTime(m).Rows.Count > 0)
                //    {//獲取到有效期控管商品的保質期
                //        m.cde_dt_incr = int.Parse(_iasdMgr.getTime(m).Rows[0]["cde_dt_incr"].ToString());
                //        m.cde_dt_shp = int.Parse(_iasdMgr.getTime(m).Rows[0]["cde_dt_shp"].ToString());
                //    }
                //    if (!string.IsNullOrEmpty(Request.Params["cde_dt"]))
                //    {//獲取有效日期算出製造日期
                //        m.cde_dt = DateTime.Parse(Request.Params["cde_dt"]);
                //        if (m.cde_dt_incr > 0)
                //        {
                //            m.made_dt = m.cde_dt.AddDays(-m.cde_dt_incr);
                //        }
                //        else
                //        {
                //            m.made_dt = DateTime.Now;
                //        }
                //    }
                //    else if (!string.IsNullOrEmpty(Request.Params["made_dt"]))
                //    {//獲取製造日期獲取有效日期
                //        m.made_dt = DateTime.Parse(Request.Params["made_dt"]);
                //        if (m.cde_dt_incr > 0)
                //        {
                //            m.cde_dt = m.made_dt.AddDays(m.cde_dt_incr);
                //        }
                //        else
                //        {
                //            m.cde_dt = DateTime.Now;
                //        }
                //    }
                //    else
                //    {//不是有效期控管
                //        m.made_dt = DateTime.Now;
                //        m.cde_dt = DateTime.Now;
                //    }
                //    if (m.act_pick_qty > 0)
                //    {
                //        sb.Append(_iasdMgr.AddIwsRecord(m));
                //    }
                //    //m.act_pick_qty = m.ord_qty - m.out_qty;
                //    _iasdMgr.InsertSql(sb.ToString());//執行SQL語句裡面有事物處理
                //    int result = _iasdMgr.DecisionBulkPicking(m, 3);//判斷調度是否檢完，是否檢夠，是否可以裝箱

                //    json = "{success:true,msg:'" + result + "'" + "}";//返回json數據  
                //    #endregion
                //}
            }
            catch (Exception ex)
            {
                throw new Exception("MarketTallyMgr-->GETMarkTallyWD-->" + ex.Message, ex);
            }
            return true;
        }
    }
}
