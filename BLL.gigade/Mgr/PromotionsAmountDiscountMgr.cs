
#region 文件信息
/* 
* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsAmountDiscountMgr.cs    
* 摘 要：                                                                               
* 幾件幾元、幾件幾折
* 当前版本：v1.1                                                                 
* 作 者： shuangshuang0420j                                           
* 完成日期：2014/6/20 
* 修改歷史：                                                                     
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：shuangshuang0420j     
*         v1.1修改内容：規範代碼結構，完善異常拋出，添加注釋
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using System.Data;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model;
using System.Collections;

namespace BLL.gigade.Mgr 
{
    public class PromotionsAmountDiscountMgr : IPromotionsAmountDiscountImplMgr
    {
        private IPromotionsAmountDiscountImplDao _padDao;
        private MySqlDao _mysqlDao;
        ProductCategoryDao _proCateDao = null;
        ProductCategorySetDao _prodCategSet = null;
        ProdPromoDao _prodpromoDao = null;
        PromoAllDao _proAllDao = null;
        UserConditionDao _usconDao = null;
        PromoDiscountDao _promoDisDao = null;
        PromotionsMaintainDao _promoMainDao = null;
        ProductDao _prodDao = null;
        VendorBrandDao _vendorBrandDao = null;
        private string connStr;
        public PromotionsAmountDiscountMgr(string connectionstring)
        {
            _mysqlDao = new MySqlDao(connectionstring);
            _padDao = new PromotionsAmountDiscountDao(connectionstring);
            _prodpromoDao = new ProdPromoDao(connectionstring);
            _proCateDao = new ProductCategoryDao(connectionstring);
            _prodCategSet = new ProductCategorySetDao(connectionstring);
            _proAllDao = new PromoAllDao(connectionstring);
            _usconDao = new UserConditionDao(connectionstring);
            _promoDisDao = new PromoDiscountDao(connectionstring);
            _promoMainDao = new PromotionsMaintainDao(connectionstring);
            _prodDao = new ProductDao(connectionstring);
            _vendorBrandDao = new VendorBrandDao(connectionstring);
            connStr = connectionstring;
        }
        #region 獲取幾件幾元和幾件幾折的信息查詢  + List<PromotionAmountDiscountQuery> Query(PromotionAmountDiscountQuery query, out int totalCount, string eventtype)
        public List<PromotionAmountDiscountQuery> Query(PromotionAmountDiscountQuery query, out int totalCount, string eventtype)
        {

            try
            {
                List<PromotionAmountDiscountQuery> _list = _padDao.Query(query, out totalCount, eventtype);
                if (_list.Count > 0)
                {
                    ShopClassDao _scDao = new ShopClassDao(connStr);
                    VipUserGroupDao _vugDao = new VipUserGroupDao(connStr);
                    SiteDao _sDao = new SiteDao(connStr);
                    foreach (PromotionAmountDiscountQuery item in _list)
                    {
                        if (item.class_id != 0)
                        {
                            item.class_name = _scDao.QueryStore(item.class_id).FirstOrDefault().class_name;
                        }
                        if (item.brand_id != 0)
                        {
                            item.brand_name = _vendorBrandDao.GetProductBrand(new VendorBrand { Brand_Id = Convert.ToUInt32(item.brand_id) }).Brand_Name;
                        }
                        if (item.group_id != 0)
                        {
                            item.group_name = _vugDao.GetModelById(Convert.ToUInt32(item.group_id)).group_name;
                        }
                        string[] arrySite = item.siteId.Split(',');
                        foreach (string i in arrySite)
                        {
                            if (i != "" && i != "0")
                            {
                                item.site += _sDao.Query(new Site { Site_Id = Convert.ToUInt32(i) }).FirstOrDefault().Site_Name + ",";
                            }
                        }
                        item.site = item.site.TrimEnd(',');
                        item.isallclass = GetProductFromProCatSetAsCid(item.category_id, 999999);
                    }
                }
                return _list;
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountDiscountMgr-->Query-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 第一次保存幾件幾元和幾件幾折的信息 +  DataTable Save(Model.PromotionsAmountDiscount query)
        public DataTable Save(Model.PromotionsAmountDiscount query)
        {
            try
            {
                return _padDao.Save(query);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountDiscountMgr-->Save-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 修改幾件幾元和幾件幾折的狀態 + int UpdateActive(PromotionsAmountDiscountCustom query)
        public bool UpdateActive(PromotionsAmountDiscountCustom query)
        {
            ArrayList _list = new ArrayList();
            try
            {
                _list.Add(_prodpromoDao.DeleteProdProm(query.event_id));
                if (query.active)//true 為啟用，false為棄用
                {
                    List<ProductCategorySet> lmodelSet = _prodCategSet.Query(new ProductCategorySet { Category_Id = query.category_id });
                    foreach (ProductCategorySet item in lmodelSet)
                    {
                        ProdPromo ppmodel = new ProdPromo();
                        ppmodel.product_id = Convert.ToInt32(item.Product_Id);
                        ppmodel.event_id = query.event_id;
                        ppmodel.event_type = query.event_type;
                        ppmodel.event_desc = query.event_desc;
                        ppmodel.start = query.start;
                        ppmodel.end = query.end;
                        ppmodel.page_url = query.category_link_url;
                        if (query.group_id != 0 || query.condition_id != 0)
                        {
                            ppmodel.user_specified = 1;
                        }
                        else ppmodel.user_specified = 0;
                        ppmodel.kuser = query.muser;
                        ppmodel.kdate = query.modified;
                        ppmodel.muser = query.muser;
                        ppmodel.mdate = query.modified;
                        ppmodel.status = query.status;
                        _list.Add(_prodpromoDao.SaveProdProm(ppmodel));
                    }
                }
                _list.Add(_padDao.UpdatePromoAmountDisActive(query));

                return _mysqlDao.ExcuteSqls(_list);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountDiscountMgr-->UpdateActive-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 刪除幾件幾元和幾件幾折的信息 + int Delete(Model.PromotionsAmountDiscount query, string eventid)
        public int Delete(Model.PromotionsAmountDiscount query, string eventid)
        {
            try
            {
                return _padDao.Delete(query, eventid);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountDiscountMgr-->Delete-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 根據id獲取幾件幾元和幾件幾折的信息 + PromotionsAmountDiscountCustom GetModelById(int id)
        public PromotionsAmountDiscountCustom GetModelById(int id)
        {

            try
            {
                return _padDao.GetModelById(id);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountDiscountMgr-->GetModelById-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 第二次保存幾件幾折的信息  修正幾件幾折 edit by shuangshuang0420j 2015.01.27 16:30:21
        public bool ReSaveDiscount(PromotionsAmountDiscountCustom model)
        {

            ArrayList arryList = new ArrayList();
            try
            {
                if (model.product_id == 999999)
                {
                    ProductCategorySet pcsModel = new ProductCategorySet();
                    pcsModel.Brand_Id = 0;
                    pcsModel.Category_Id = model.category_id;
                    pcsModel.Product_Id = 999999;
                    arryList.Add(_prodCategSet.SaveProdCategorySet(pcsModel));

                }
                else
                {
                    //刪除全館商品
                    ProductCategorySet qgSet = new ProductCategorySet();
                    qgSet.Category_Id = model.category_id;
                    qgSet.Product_Id = 999999;//全館商品刪除 id=999999
                    //根據category_id刪除product_category_set表數據

                    if (model.url_by == 1)
                    {
                        if (model.brand_id != 0)//專區時當品牌不為空時講該品牌下的所有商品加入set表
                        {
                            arryList.Add(_prodCategSet.DelProdCateSetByCPID(qgSet));
                            QueryVerifyCondition query = new QueryVerifyCondition();
                            query.brand_id = Convert.ToUInt32(model.brand_id);
                            query.site_ids = model.site;
                            query.combination = 1;
                            query.IsPage = false;
                            int totalCount = 0;
                            List<QueryandVerifyCustom> qvcList = _promoMainDao.GetProList(query, out totalCount);
                            foreach (QueryandVerifyCustom qvcItem in qvcList)
                            {
                                ProductCategorySet pcsModel = new ProductCategorySet();
                                pcsModel.Product_Id = qvcItem.product_id;
                                pcsModel.Brand_Id = Convert.ToUInt32(model.brand_id);
                                pcsModel.Category_Id = model.category_id;
                                arryList.Add(_prodCategSet.SaveProdCategorySet(pcsModel));
                            }
                        }
                    }
                    else//非專區時
                    {
                        if (model.event_type == "M1")
                        {
                            arryList.Add(_prodCategSet.DelProdCateSetByCPID(qgSet));
                        }
                        if (model.product_id != 0)
                        {
                            arryList.Add(_prodCategSet.DelProdCateSetByCPID(qgSet));
                            ProductCategorySet pcsModel = new ProductCategorySet();
                            pcsModel.Product_Id = Convert.ToUInt32(model.product_id);
                            pcsModel.Brand_Id = _prodDao.Query(new Product { Product_Id = pcsModel.Product_Id }).FirstOrDefault().Brand_Id;
                            pcsModel.Category_Id = model.category_id;
                            arryList.Add(_prodCategSet.SaveProdCategorySet(pcsModel));
                        }
                        else if (model.brand_id != 0)
                        {
                            arryList.Add(_prodCategSet.DelProdCateSetByCPID(qgSet));
                            QueryVerifyCondition query = new QueryVerifyCondition();
                            query.brand_id = Convert.ToUInt32(model.brand_id);
                            query.site_ids = model.site;
                            query.combination = 1;
                            query.IsPage = false;
                            int totalCount = 0;//篩選出該品牌下符合條件的商品
                            List<QueryandVerifyCustom> qvcList = _promoMainDao.GetProList(query, out totalCount);
                            foreach (QueryandVerifyCustom qvcItem in qvcList)
                            {
                                ProductCategorySet pcsModel = new ProductCategorySet();
                                pcsModel.Product_Id = qvcItem.product_id;
                                pcsModel.Brand_Id = Convert.ToUInt32(model.brand_id);
                                pcsModel.Category_Id = model.category_id;
                                arryList.Add(_prodCategSet.SaveProdCategorySet(pcsModel));
                            }
                        }
                        else if (model.class_id != 0)
                        {
                            arryList.Add(_prodCategSet.DelProdCateSetByCPID(qgSet));
                            VendorBrandSetDao _vbsDao = new VendorBrandSetDao(connStr);
                            List<VendorBrandSet> brandIDs = _vbsDao.Query(new VendorBrandSet { class_id = (uint)model.class_id });

                            if (brandIDs.Count > 0)
                            {
                                QueryVerifyCondition query = new QueryVerifyCondition();
                                foreach (VendorBrandSet item in brandIDs)
                                {
                                    query.brand_ids += item.brand_id + ",";
                                }
                                query.brand_ids = query.brand_ids.TrimEnd(',');
                                query.site_ids = model.site;
                                query.combination = 1;
                                query.IsPage = false;
                                int totalCount = 0;
                                List<QueryandVerifyCustom> qvcList = _promoMainDao.GetProList(query, out totalCount);
                                foreach (QueryandVerifyCustom qvcItem in qvcList)
                                {
                                    ProductCategorySet pcsModel = new ProductCategorySet();
                                    pcsModel.Product_Id = qvcItem.product_id;
                                    pcsModel.Brand_Id = Convert.ToUInt32(model.brand_id);
                                    pcsModel.Category_Id = model.category_id;
                                    arryList.Add(_prodCategSet.SaveProdCategorySet(pcsModel));
                                }
                            }
                        }
                    }
                }

                PromoAll pamodel = new PromoAll();
                pamodel.event_id = model.event_id;
                pamodel.event_type = model.event_type;
                pamodel.brand_id = model.brand_id;
                pamodel.class_id = model.class_id;
                pamodel.category_id = Convert.ToInt32(model.category_id);
                pamodel.startTime = model.start;
                pamodel.end = model.end;
                pamodel.status = model.status;
                pamodel.kuser = model.kuser;
                pamodel.kdate = model.created;
                pamodel.muser = model.kuser;
                pamodel.mdate = model.created;
                pamodel.product_id = model.product_id;
                pamodel.class_id = model.class_id;
                pamodel.brand_id = model.brand_id;
                arryList.Add(_proAllDao.SavePromAll(pamodel));
                ProductCategory pcmodel = _proCateDao.GetModelById(Convert.ToUInt32(model.category_id));
                pcmodel.category_id = Convert.ToUInt32(model.category_id);
                pcmodel.banner_image = model.banner_image;
                pcmodel.category_link_url = model.category_link_url;
                pcmodel.category_display = Convert.ToUInt32(model.status);
                pcmodel.category_name = model.name;
                pcmodel.category_father_id = model.category_father_id;
                arryList.Add(_proCateDao.UpdateProdCate(pcmodel));
                arryList.Add(_padDao.UpdatePromoAmountDis(model));
                return _mysqlDao.ExcuteSqls(arryList);
            }
            catch (Exception ex)
            {

                throw new Exception("PromotionsAmountDiscountMgr-->ReSaveDiscount-->" + ex.Message, ex);
            }


        }
        #endregion

        #region 編輯幾件幾折信息 修正幾件幾折 edit by shuangshuang0420j 2015.01.27 16:30:21
        public bool ReUpdateDiscount(PromotionsAmountDiscountCustom model, string oldEventId)
        {
            ArrayList arryList = new ArrayList();
            try
            {

                arryList.Add(_prodpromoDao.DeleteProdProm(oldEventId));//修改時先刪除原有的prodprom
                if (model.product_id == 999999)
                {
                    ProductCategorySet pcsModel = new ProductCategorySet();

                    pcsModel.Brand_Id = 0;
                    pcsModel.Category_Id = model.category_id;
                    pcsModel.Product_Id = 999999;
                    arryList.Add(_prodCategSet.DeleteProdCateSet(pcsModel.Category_Id));
                    arryList.Add(_prodCategSet.SaveProdCategorySet(pcsModel));

                }
                else
                {
                    if (model.url_by == 1)
                    {
                        if (model.brand_id != 0)//當品牌不為空時講該品牌下的所有商品加入set表
                        {
                            arryList.Add(_prodCategSet.DeleteProdCateSet(model.category_id));//brand_id不為0時先刪除set表中數據，在新增

                            QueryVerifyCondition query = new QueryVerifyCondition();
                            query.brand_id = Convert.ToUInt32(model.brand_id);
                            query.site_ids = model.site;
                            query.combination = 1;
                            query.IsPage = false;
                            int totalCount = 0;
                            List<QueryandVerifyCustom> qvcList = _promoMainDao.GetProList(query, out totalCount);
                            foreach (QueryandVerifyCustom qvcItem in qvcList)
                            {
                                ProductCategorySet pcsModel = new ProductCategorySet();
                                pcsModel.Product_Id = qvcItem.product_id;
                                pcsModel.Brand_Id = Convert.ToUInt32(model.brand_id);
                                pcsModel.Category_Id = model.category_id;
                                //刪除已有的 新增異動的
                                arryList.Add(_prodCategSet.SaveProdCategorySet(pcsModel));
                            }
                        }
                    }
                    else//非專區
                    {
                        if (model.event_type == "M1")
                        {
                            arryList.Add(_prodCategSet.DeleteProdCateSet(model.category_id));//幾件幾折的非專區時先刪除set表中數據，在新增
                        }

                        if (model.product_id != 0)
                        {
                            arryList.Add(_prodCategSet.DeleteProdCateSet(model.category_id));//非專區時先刪除set表中數據，在新增
                            ProductCategorySet pcsModel = new ProductCategorySet();
                            pcsModel.Product_Id = Convert.ToUInt32(model.product_id);
                            pcsModel.Brand_Id = _prodDao.Query(new Product { Product_Id = pcsModel.Product_Id }).FirstOrDefault().Brand_Id;
                            pcsModel.Category_Id = model.category_id;
                            arryList.Add(_prodCategSet.SaveProdCategorySet(pcsModel));
                        }
                        else if (model.brand_id != 0)
                        {
                            arryList.Add(_prodCategSet.DeleteProdCateSet(model.category_id));//非專區時先刪除set表中數據，在新增
                            QueryVerifyCondition query = new QueryVerifyCondition();
                            query.brand_id = Convert.ToUInt32(model.brand_id);
                            query.site_ids = model.site;
                            query.combination = 1;
                            int totalCount = 0;
                            query.IsPage = false;
                            List<QueryandVerifyCustom> qvcList = _promoMainDao.GetProList(query, out totalCount);
                            foreach (QueryandVerifyCustom qvcItem in qvcList)
                            {
                                ProductCategorySet pcsModel = new ProductCategorySet();
                                pcsModel.Product_Id = qvcItem.product_id;
                                pcsModel.Brand_Id = Convert.ToUInt32(model.brand_id);
                                pcsModel.Category_Id = model.category_id;
                                arryList.Add(_prodCategSet.SaveProdCategorySet(pcsModel));
                            }
                        }
                        else if (model.class_id != 0)
                        {
                            arryList.Add(_prodCategSet.DeleteProdCateSet(model.category_id));//非專區時先刪除set表中數據，在新增
                            VendorBrandSetDao _vbsDao = new VendorBrandSetDao(connStr);
                            List<VendorBrandSet> brandIDs = _vbsDao.Query(new VendorBrandSet { class_id = (uint)model.class_id });

                            if (brandIDs.Count > 0)
                            {
                                QueryVerifyCondition query = new QueryVerifyCondition();
                                foreach (VendorBrandSet item in brandIDs)
                                {
                                    query.brand_ids += item.brand_id + ",";
                                }
                                query.brand_ids = query.brand_ids.TrimEnd(',');
                                query.site_ids = model.site;
                                query.combination = 1;
                                query.IsPage = false;
                                int totalCount = 0;
                                List<QueryandVerifyCustom> qvcList = _promoMainDao.GetProList(query, out totalCount);
                                foreach (QueryandVerifyCustom qvcItem in qvcList)
                                {
                                    ProductCategorySet pcsModel = new ProductCategorySet();
                                    pcsModel.Product_Id = qvcItem.product_id;
                                    pcsModel.Brand_Id = Convert.ToUInt32(model.brand_id);
                                    pcsModel.Category_Id = model.category_id;
                                    arryList.Add(_prodCategSet.SaveProdCategorySet(pcsModel));
                                }
                            }
                        }
                    }
                }
                if (model.event_id != "")
                {
                    arryList.Add(_proAllDao.DelPromAll(oldEventId));
                }
                PromoAll pamodel = new PromoAll();
                pamodel.event_id = model.event_id;
                pamodel.event_type = model.event_type;
                pamodel.brand_id = model.brand_id;
                pamodel.class_id = model.class_id;
                pamodel.category_id = Convert.ToInt32(model.category_id);
                pamodel.startTime = model.start;
                pamodel.end = model.end;
                pamodel.status = model.status;
                pamodel.kuser = model.kuser;
                pamodel.kdate = model.created;
                pamodel.muser = model.muser;
                pamodel.mdate = model.modified;
                pamodel.status = model.status;
                pamodel.product_id = model.product_id;
                pamodel.class_id = model.class_id;
                pamodel.brand_id = model.brand_id;
                arryList.Add(_proAllDao.SavePromAll(pamodel));
                ProductCategory pcmodel = _proCateDao.GetModelById(Convert.ToUInt32(model.category_id));
                pcmodel.category_id = Convert.ToUInt32(model.category_id);
                pcmodel.banner_image = model.banner_image;
                pcmodel.category_link_url = model.category_link_url;
                pcmodel.category_display = Convert.ToUInt32(model.status);
                pcmodel.category_name = model.name;
                pcmodel.category_updatedate = (uint)BLL.gigade.Common.CommonFunction.GetPHPTime(model.modified.ToString());
                pcmodel.category_father_id = model.category_father_id;
                arryList.Add(_proCateDao.UpdateProdCate(pcmodel));
                arryList.Add(_padDao.UpdatePromoAmountDis(model));
                return _mysqlDao.ExcuteSqls(arryList);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountDiscountDao-->ReUpdate-->" + ex.Message, ex);
            }

        }
        #endregion

        #region 滿額滿件折扣 shiwei0620j 20150514

        public List<PromotionsAmountDiscountCustom> GetList(PromotionsAmountDiscountCustom query, out int totalCount)
        {
            try
            {
                List<PromotionsAmountDiscountCustom>store=_padDao.GetList(query, out totalCount);
                     SiteDao _sDao = new SiteDao(connStr);
                foreach (var item in store)
                {

                    string[] araySite = item.site.Split(',');
                    foreach (string site in araySite)
                    {
                        if (item.group_name == "")
                        {
                            item.group_name = "不分";
                        }
                        if (site != "" && site != "0")
                        {
                            item.site_name += _sDao.Query(new Site { Site_Id = Convert.ToUInt32(site) }).FirstOrDefault().Site_Name + ",";
                        }
                    }
                    item.site_name = item.site_name.TrimEnd(',');
                }
                return store;
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountDiscountMgr-->GetList-->" + ex.Message, ex);
            }
        }

        public int Save(PromotionsAmountDiscountCustom query)
        {
            try
            {
                return _padDao.Save(query);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountDiscountMgr-->Save-->" + ex.Message, ex);
            }
        }

        public bool Delete(List<PromotionsAmountDiscountCustom> list)
        {
            try
            {
                return _padDao.Delete(list);
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountDiscountMgr-->Delete-->" + ex.Message, ex);
            }
        }
        #endregion


        public bool UpdatePromoAmountDisActive(PromotionsAmountDiscountCustom model)
        {
            try
            {

                ArrayList arrList = new ArrayList();
                arrList.Add(_padDao.UpdatePromoAmountDisActive(model));
                if (_mysqlDao.ExcuteSqls(arrList))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountDiscountMgr-->UpdatePromoAmountDisActive-->" + ex.Message, ex);
            }
        }


        #region 查看活動商品表中是否有全館商品 -int GetProductFromProCatSetAsCid()
        /// <summary>
        /// 查看活動商品表中是否有全館商品
        /// </summary>
        /// <returns></returns>
        private int GetProductFromProCatSetAsCid(UInt32 category_id, UInt32 product_id)
        {
            int allproduct = 0;
            try
            {
                ProductCategorySet pcsModel = new ProductCategorySet();
                pcsModel.Category_Id = Convert.ToUInt32(category_id);
                pcsModel.Product_Id = product_id;
                if (_prodCategSet.Query(pcsModel).Count > 0)
                {
                    allproduct = 1;
                }
                return allproduct;
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsAmountDiscountMgr-->GetProductFromProCatSetAsCid-->" + ex.Message, ex);
            }

        }
        #endregion
    }
}
