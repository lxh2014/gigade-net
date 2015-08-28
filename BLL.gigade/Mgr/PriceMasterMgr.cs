/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductSiteMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 10:04:24 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Mgr.Impl;
using System.Collections;
using BLL.gigade.Model;
using BLL.gigade.Common;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Mgr
{
    public class PriceMasterMgr : IPriceMasterImplMgr
    {
        private IPriceMasterImplDao _priceMasterDao;
        string connectionString = string.Empty;
        public PriceMasterMgr(string connectionStr)
        {
            _priceMasterDao = new PriceMasterDao(connectionStr);
            connectionString = connectionStr;
        }

        public List<Model.Custom.PriceMasterCustom> Query(Model.PriceMaster priceMaster)
        {
            return _priceMasterDao.Query(priceMaster);
        }

        public List<Model.PriceMaster> Query(uint[] priceMasterId)
        {
            return _priceMasterDao.Query(priceMasterId);
        }

        public Model.PriceMaster QueryPriceMaster(Model.PriceMaster pM)
        {
            return _priceMasterDao.QueryPriceMaster(pM);
        }

        public List<Model.PriceMaster> PriceMasterQuery(Model.PriceMaster query)
        {
            return _priceMasterDao.PriceMasterQuery(query);
        }
        public List<Model.PriceMaster> QuerySelf(Model.PriceMaster query)
        {
            return _priceMasterDao.QuerySelf(query);
        }

        public List<Model.PriceMaster> PriceMasterQueryByid(Model.PriceMaster query)
        {
            return _priceMasterDao.PriceMasterQueryByid(query);
        }

        public string Save(Model.PriceMaster priceMaster)
        {
            return _priceMasterDao.Save(priceMaster);
        }

        public string SaveNoProId(Model.PriceMaster priceMaster)
        {
            return _priceMasterDao.SaveNoProId(priceMaster);
        }

        public Model.PriceMaster QueryPMaster(Model.PriceMaster pM)
        {
            return _priceMasterDao.QueryPMaster(pM);
        }

        public bool Save(List<Model.PriceMaster> priceMasterList)
        {
            ArrayList sqls = new ArrayList();
            foreach (var item in priceMasterList)
            {
                sqls.Add(_priceMasterDao.Save(item));
            }
            MySqlDao mySqlDao = new MySqlDao(connectionString);
            return mySqlDao.ExcuteSqls(sqls);
        }

        public string Update(Model.PriceMaster priceMaster)
        {
            return _priceMasterDao.Update(priceMaster);
        }

        public string SelectChild(Model.PriceMaster priceMaster)
        {
            return _priceMasterDao.SelectChild(priceMaster);
        }

        public string DeleteByProductId(int product_Id)
        {
            return _priceMasterDao.DeleteByProductId(product_Id);
        }

        public int Save(Model.PriceMaster priceMaster, List<Model.ItemPrice> itemPrices, ArrayList otherSqls, ref string msg)
        {
            List<Model.Custom.PriceMasterCustom> master = Query(priceMaster);
            if (master != null && master.Count > 0)
            {
                if (priceMaster.user_id != 0 || (priceMaster.user_id == 0 && master.Where(p => p.user_id == 0).Count() > 0))
                {
                    if (Resource.CoreMessage != null)
                    {
                        msg = Resource.CoreMessage.GetResource("SITE_EXIST");
                    }
                    else
                    {
                        msg = "此站臺價格已經存在";
                    }
                    return -1;
                }
            }
            string priceMasterSql = Save(priceMaster);

            //IPriceMasterTsImplMgr _priceMasterTs = new PriceMasterTsMgr("");
            //priceMasterSql += _priceMasterTs.SaveTs("@@identity"); //保存完price_master表後，在price_master_ts表保存一個副本以便審核 edit by xiangwang0413w 2014/07/22

            ItemPriceMgr _itemPriceMgr = new ItemPriceMgr("");
            //ItemPriceTsMgr _itemPriceTsMgr = new ItemPriceTsMgr("");
            ArrayList itemPriceSql = new ArrayList();
            if (itemPrices != null)
            {
                //itemPrices.ForEach(m => itemPriceSql.Add(_itemPriceMgr.Save(m) /*+ _itemPriceTsMgr.SaveTs("@@identity")*/));//保存完item_price表後，在item_price_ts表保存一個副本以便審核 edit by xiangwang0413w 2014/07/22
                itemPrices.ForEach(m => itemPriceSql.Add(_itemPriceMgr.Save(m)));
            }
            return _priceMasterDao.Save(priceMasterSql, itemPriceSql, otherSqls);
        }

        public List<Model.Custom.SingleProductPrice> SingleProductPriceQuery(uint product_id, int pile_id)
        {
            return _priceMasterDao.SingleProductPriceQuery(product_id, pile_id);
        }
        //查詢各自定價時組合商品中單一商品信息  add by zhuoqin0830w   2015/07/09
        public List<Model.Custom.SelfSingleProductPrice> SingleProductPriceQuery(uint product_id)
        {
            return _priceMasterDao.SingleProductPriceQuery(product_id);
        }

        public List<Model.PriceMaster> QueryByUserId(Model.PriceMaster query)
        {
            return _priceMasterDao.QueryByUserId(query);
        }

        public uint QueryPriceMasterId(Model.PriceMaster query)
        {
            return _priceMasterDao.QueryPriceMasterId(query);
        }

        public List<ItemPrice> AddSingleProduct(Model.Custom.PriceMasterCustom pmc, string typePrice)
        {
            List<ItemPrice> list = new List<Model.ItemPrice>();
            list = _priceMasterDao.AddSingleProduct(pmc);

            if (typePrice == "discount")
            {
                uint primeval_money = 0;
                uint primeval_cost = 0;
                foreach (ItemPrice p in list)
                {
                    p.apply_id = pmc.apply_id;
                    primeval_money = p.item_money;
                    primeval_cost = p.item_cost;
                    p.item_money = (uint)CommonFunction.ArithmeticalDiscount((int)p.item_money, pmc.price_discount);// Convert.ToUInt32(Convert.ToDouble(p.item_money) * (pmc._discount * 0.01));
                    p.item_cost = (uint)CommonFunction.ArithmeticalDiscount((int)p.item_cost, pmc.cost_discount);//Convert.ToUInt32(Convert.ToDouble(p.item_cost) * (pmc._cost_discount * 0.01));
                    p.event_money = (uint)CommonFunction.ArithmeticalDiscount((int)primeval_money, pmc.event_price_discount);// Convert.ToUInt32(Convert.ToDouble(primeval_money) * (pmc.event_price_discount * 0.01));
                    p.event_cost = (uint)CommonFunction.ArithmeticalDiscount((int)p.event_money, pmc.event_cost_discount);// Convert.ToUInt32(Convert.ToDouble(primeval_cost) * (pmc.event_cost_discount * 0.01));
                }
            }
            else if (typePrice == "price")
            {
                foreach (ItemPrice p in list)
                {
                    p.apply_id = pmc.apply_id;
                    p.item_money = Convert.ToUInt32(pmc.price_at);
                    p.item_cost = Convert.ToUInt32(pmc.cost_at);
                    p.event_money = Convert.ToUInt32(pmc.event_price);
                    p.event_cost = Convert.ToUInt32(pmc.event_cost);
                }
            }
            return list;
        }

        public List<Model.PriceMaster> GetPriceMasterInfo(string productIds, int siteId)
        {
            return _priceMasterDao.GetPriceMasterInfo(productIds, siteId);
        }

        public List<Model.PriceMaster> GetPriceMasterInfoByID2(string productID)
        {
            return _priceMasterDao.GetPriceMasterInfoByID2(productID);
        }
        public string VendorSelectChild(Model.PriceMasterTemp priceMaster)
        {//20140905 複製供應商
            return _priceMasterDao.VendorSelectChild(priceMaster);
        }

        /// <summary>
        /// 根據條件只修改product_name
        /// edit by wwei0216w 2014/12/18
        /// </summary>
        /// <returns></returns>
        public string UpdateName(PriceMaster pm)
        {
            return _priceMasterDao.UpdateName(pm);
        }

        /// <summary>
        /// 修改商品站台價格表(price_master)信息時,申請審核以及保存價格審核記錄
        /// </summary>
        /// <param name="priceMasters">price_master列表</param>
        /// <param name="caller"></param>
        /// <returns>返品錯誤信息</returns>
        public bool UpdatePriceMasters(List<Model.PriceMaster> priceMasters, Caller caller, out string msg)
        {
            msg = string.Empty;
            bool result = false;
            try
            {
                string batchNo = CommonFunction.GetPHPTime().ToString() + "_" + caller.user_id + "_";
                PriceMasterTsMgr _priceMasterTsMgr = new PriceMasterTsMgr(connectionString);
                HistoryBatch batch = new HistoryBatch();
                batch.kuser = caller.user_email;

                //價格修改 申請審核
                PriceUpdateApply priceUpdateApply = new PriceUpdateApply();
                priceUpdateApply.apply_user = (uint)caller.user_id;

                //價格審核記錄
                PriceUpdateApplyHistory applyHistroy = new PriceUpdateApplyHistory();
                applyHistroy.user_id = (int)priceUpdateApply.apply_user;
                //applyHistroy.price_status = 1;
                //applyHistroy.type = 3;
                applyHistroy.price_status = 1; //edit by wwei0216w 2014/12/16 價格修改時 price_status為 2申請審核
                applyHistroy.type = 1;//edit by wwei0216w 所作操作為 1:申請審核的操作 

                PriceUpdateApplyMgr _priceUpdateApplyMgr = new PriceUpdateApplyMgr(connectionString);
                PriceUpdateApplyHistoryMgr _priceUpdateApplyHistoryMgr = new PriceUpdateApplyHistoryMgr(connectionString);
                TableHistoryMgr _tableHistoryMgr = new TableHistoryMgr(connectionString);

                foreach (var pM in priceMasters)
                {
                    ArrayList excuteSql = new ArrayList();
                    priceUpdateApply.price_master_id = pM.price_master_id;
                    int apply_id = _priceUpdateApplyMgr.Save(priceUpdateApply);
                    if (apply_id != -1)
                    {
                        pM.apply_id = (uint)apply_id;
                        pM.price_status = 2;//申請審核
                        applyHistroy.apply_id = apply_id;

                        batch.batchno = batchNo + pM.product_id;//批號

                        excuteSql.Add(_priceMasterTsMgr.UpdateTs(pM));//將修改數據保存至price_master_ts表
                        excuteSql.Add(_priceUpdateApplyHistoryMgr.SaveSql(applyHistroy));//保存審核記錄
                        if (!_tableHistoryMgr.SaveHistory<PriceMaster>(pM, batch, excuteSql))
                            msg += pM.product_id.ToString() + ";";
                        else
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                msg += e.Message;
            }
            return result;
        }

        public List<PriceMasterCustom> GetExcelItemIdInfo(string price_master_ids)
        {
            try
            {
                return _priceMasterDao.GetExcelItemIdInfo(price_master_ids);
            }
            catch (Exception ex)
            {
                throw new Exception("IPriceMasterImplMgr-->GetExcelItemIdInfo" + ex.Message,ex) ;
            }
        }
    }
}
