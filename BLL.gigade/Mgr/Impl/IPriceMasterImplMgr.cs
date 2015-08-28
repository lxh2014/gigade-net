/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductSiteImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 10:04:14 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Mgr.Impl
{
    public interface IPriceMasterImplMgr
    {
        List<Model.Custom.PriceMasterCustom> Query(Model.PriceMaster priceMaster);
        List<Model.PriceMaster> Query(uint[] priceMasterId); //add by wwei 2014/12/11
        string Save(Model.PriceMaster priceMaster);
        string SaveNoProId(Model.PriceMaster priceMaster);
        string Update(Model.PriceMaster priceMaster);
        string SelectChild(Model.PriceMaster priceMaster);
        int Save(Model.PriceMaster priceMaster, List<Model.ItemPrice> itemPrices, ArrayList otherSqls, ref string errorString);
        bool Save(List<Model.PriceMaster> priceMasterList);
        List<Model.Custom.SingleProductPrice> SingleProductPriceQuery(uint product_id, int pile_id);
        //查詢各自定價時組合商品中單一商品信息  add by zhuoqin0830w   2015/07/09
        List<Model.Custom.SelfSingleProductPrice> SingleProductPriceQuery(uint product_id);

        Model.PriceMaster QueryPMaster(Model.PriceMaster pM);
        Model.PriceMaster QueryPriceMaster(Model.PriceMaster pM);
        List<Model.PriceMaster> PriceMasterQuery(Model.PriceMaster query);
        List<Model.PriceMaster> QuerySelf(Model.PriceMaster query);
        List<Model.PriceMaster> PriceMasterQueryByid(Model.PriceMaster query);
        List<Model.PriceMaster> QueryByUserId(Model.PriceMaster query);
        uint QueryPriceMasterId(Model.PriceMaster query);
        List<Model.ItemPrice> AddSingleProduct(Model.Custom.PriceMasterCustom pmc, string typePrice);
        List<Model.PriceMaster> GetPriceMasterInfo(string productIds, int siteId);// add by wangwei0216w 2014/8/27
        List<Model.PriceMaster> GetPriceMasterInfoByID2(string productID);// add bywangwei 0216w 2014/8/28
        string VendorSelectChild(Model.PriceMasterTemp priceMaster);
        bool UpdatePriceMasters(List<Model.PriceMaster> priceMasters, Model.Caller caller, out string msg);
        /// <summary>
        /// 根據條件只修改product_name
        /// edit by wwei0216w 2014/12/18
        /// </summary>
        /// <returns></returns>
        string UpdateName(Model.PriceMaster pm);
        List<PriceMasterCustom> GetExcelItemIdInfo(string price_master_ids);
    }
}
