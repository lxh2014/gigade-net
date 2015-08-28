#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：ProductTempMgr.cs
* 摘 要：
* producttemp的表操作
* 当前版本：v1.0
* 作 者： 
* 完成日期：
* 修改歷史：
*         v1.1修改日期：2014/8/18 15:35
*         v1.1修改人員：shuangshuang0420j
*         v1.1修改内容：新增供應商商品審核列表查詢+ List<Model.Custom.QueryandVerifyCustom> verifyWaitQuery(Model.Query.QueryVerifyCondition qcCon, out int totalCount)
 *                      新增 獲取供應商新建的商品 +Model.ProductTemp GetProTempByVendor(Model.ProductTemp proTemp)
 *                      新增 修改信息+string Update(Model.ProductTemp proTemp)
*                   
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using System.Collections;

namespace BLL.gigade.Mgr
{
    public class ProductTempMgr : IProductTempImplMgr
    {
        private IProductTempImplDao _pTempDao;
        private string connStr;
        public ProductTempMgr(string connectString)
        {
            _pTempDao = new ProductTempDao(connectString);
            this.connStr = connectString;
        }

        public ProductTemp GetProTemp(Model.ProductTemp proTemp)
        {
            return _pTempDao.GetProTemp(proTemp);
        }

        public int baseInfoSave(ProductTemp pTemp)
        {
            return _pTempDao.baseInfoSave(pTemp);
        }

        public int baseInfoUpdate(ProductTemp pTemp)
        {
            return _pTempDao.baseInfoUpdate(pTemp);
        }

        public int PriceBonusInfoSave(Model.ProductTemp proTemp)
        {
            return _pTempDao.PriceBonusInfoSave(proTemp);
        }

        public bool DescriptionInfoSave(ProductTemp proTemp, List<ProductTagSetTemp> tagTemps, List<ProductNoticeSetTemp> noticeTemps)
        {
            try
            {
                ArrayList sqls = new ArrayList();
                sqls.Add(_pTempDao.DescriptionInfoSave(proTemp));
                if (tagTemps != null)
                {
                    IProductTagSetTempImplMgr _productTagSetTempMgr = new ProductTagSetTempMgr(connStr);
                    sqls.Add(_productTagSetTempMgr.Delete(new ProductTagSetTemp { Writer_Id = proTemp.Writer_Id, Combo_Type = proTemp.Combo_Type, product_id = proTemp.Product_Id }));
                    foreach (var item in tagTemps)
                    {
                        sqls.Add(_productTagSetTempMgr.Save(item));
                    }
                }
                if (noticeTemps != null)
                {
                    IProductNoticeSetTempImplMgr _productNoticeSetTempMgr = new ProductNoticeSetTempMgr(connStr);
                    sqls.Add(_productNoticeSetTempMgr.Delete(new ProductNoticeSetTemp { Writer_Id = proTemp.Writer_Id, Combo_Type = proTemp.Combo_Type, product_id = proTemp.Product_Id }));
                    foreach (var item in noticeTemps)
                    {
                        sqls.Add(_productNoticeSetTempMgr.Save(item));
                    }
                }
                MySqlDao mySqlDao = new MySqlDao(connStr);
                return mySqlDao.ExcuteSqls(sqls);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempMgr.DescriptionInfoSave-->" + ex.Message, ex);
            }

        }

        public bool SpecInfoSave(Model.ProductTemp proTemp)
        {
            return _pTempDao.SpecInfoSave(proTemp) > 0;
        }

        public bool CategoryInfoUpdate(Model.ProductTemp proTemp)
        {
            return _pTempDao.CategoryInfoUpdate(proTemp) > 0;
        }

        public bool FortuneInfoSave(Model.ProductTemp proTemp)
        {
            return _pTempDao.FortuneInfoSave(proTemp) > 0;
        }

        public string MoveProduct(Model.ProductTemp proTemp)
        {
            return _pTempDao.MoveProduct(proTemp);
        }

        public string Delete(Model.ProductTemp proTemp)
        {
            return _pTempDao.Delete(proTemp);
        }

        public int ProductTempUpdate(Model.ProductTemp proTemp, string page)
        {
            return _pTempDao.ProductTempUpdate(proTemp, page);
        }

        /// <summary>
        /// 刪除臨時表
        /// </summary>
        /// <returns></returns>
        public bool DeleteTemp(int writerId, int combo_type, string product_Id)
        {
            try
            {
                ArrayList sqls = new ArrayList();
                sqls.Add(Delete(new ProductTemp { Writer_Id = writerId, Combo_Type = combo_type, Product_Id = product_Id.ToString() }));

                //edit by xiangwang0413w 2015/03/11
                ICourseProductTempImplMgr _courProdTempMgr = new CourseProductTempMgr("");
                _courProdTempMgr.DeleteSql(new CourseProductTemp { Writer_Id = writerId , Product_Id = uint.Parse(product_Id)});

                ProductItemTempMgr proItemTempMgr = new ProductItemTempMgr("");
                sqls.Add(proItemTempMgr.DeleteSql(new ProductItemTemp { Writer_Id = writerId, Product_Id = product_Id }));

                //edit by xiangwang0413w 2015/03/11
                ICourseDetailItemTempImplMgr _courDetaItemTempMgr = new CourseDetailItemTempMgr("");
                sqls.Add(_courDetaItemTempMgr.DeleteSql(writerId));

                //add by xiangwang0413w 2014/11/06
                ProductDeliverySetTempMgr proDelSetTempMgr = new ProductDeliverySetTempMgr("");
                sqls.Add(proDelSetTempMgr.Delete(new ProductDeliverySetTemp { Writer_Id = writerId,Combo_Type = combo_type, Product_id = int.Parse(product_Id) }));

                ProductNoticeSetTempMgr proNoticeSetTempMgr = new ProductNoticeSetTempMgr("");
                sqls.Add(proNoticeSetTempMgr.Delete(new ProductNoticeSetTemp { Writer_Id = writerId, Combo_Type = combo_type, product_id = product_Id }));

                ProductTagSetTempMgr proTagSetTempMgr = new ProductTagSetTempMgr("");
                sqls.Add(proTagSetTempMgr.Delete(new ProductTagSetTemp { Writer_Id = writerId, Combo_Type = combo_type, product_id = product_Id }));

                ProductSpecTempMgr proSpecTempMgr = new ProductSpecTempMgr("");
                sqls.Add(proSpecTempMgr.TempDelete(new ProductSpecTemp { Writer_Id = writerId, product_id = product_Id }));

                ProductPictureTempImplMgr proPicTempMgr = new ProductPictureTempImplMgr("");
                sqls.Add(proPicTempMgr.Delete(new ProductPictureTemp { writer_Id = writerId, combo_type = combo_type, product_id = product_Id },1)); //刪除說明圖
                sqls.Add(proPicTempMgr.Delete(new ProductPictureTemp { writer_Id = writerId, combo_type = combo_type, product_id = product_Id },2)); //刪除APP圖

                ProductCategorySetTempMgr proCateSetTempMgr = new ProductCategorySetTempMgr("");
                sqls.Add(proCateSetTempMgr.TempDelete(new ProductCategorySetTemp { Writer_Id = writerId, Combo_Type = combo_type, Product_Id = product_Id }));

                ItemPriceTempMgr itemPriceMgr = new ItemPriceTempMgr("");
                sqls.Add(itemPriceMgr.Delete(product_Id, combo_type, writerId));

                PriceMasterTempMgr priceMasterTempMgr = new PriceMasterTempMgr("");
                sqls.Add(priceMasterTempMgr.Delete(new PriceMasterTemp { writer_Id = writerId, product_id = product_Id, combo_type = combo_type }));

                ProductComboTempMgr proComboTempMgr = new ProductComboTempMgr("");
                sqls.Add(proComboTempMgr.TempDelete(new ProductComboTemp { Writer_Id = writerId, Parent_Id = product_Id }));

                MySqlDao mySqlDao = new MySqlDao(connStr);
                return mySqlDao.ExcuteSqls(sqls);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempMg.DeleteTemp-->" + ex.Message, ex);
            }
        }

        public string SaveFromPro(Model.ProductTemp proTemp)
        {
            return _pTempDao.SaveFromPro(proTemp);
        }

        public bool CopyProduct(int writerId, int combo_type, string product_Id)
        {
            try
            {
                if (DeleteTemp(writerId, combo_type, product_Id)) 
                {
                    ArrayList sqls = new ArrayList();
                    var prodTemp = new ProductTemp { Writer_Id = writerId, Combo_Type = combo_type, Product_Id = product_Id, Create_Channel = 1 };
                    sqls.Add(SaveFromPro(prodTemp));

                    //add by xiangwang0413w 2014/11/06
                    ProductDeliverySetTempMgr proDelSetTempMgr = new ProductDeliverySetTempMgr("");
                    sqls.Add(proDelSetTempMgr.SaveFromProDeliverySet(new ProductDeliverySetTemp { Writer_Id = writerId, Combo_Type = combo_type, Product_id = int.Parse(product_Id) }));

                    ProductNoticeSetTempMgr proNoticeSetTempMgr = new ProductNoticeSetTempMgr("");
                    sqls.Add(proNoticeSetTempMgr.SaveFromProNotice(new ProductNoticeSetTemp { Writer_Id = writerId, Combo_Type = combo_type, product_id = product_Id }));

                    ProductTagSetTempMgr proTagSetTempMgr = new ProductTagSetTempMgr("");
                    sqls.Add(proTagSetTempMgr.SaveFromTag(new ProductTagSetTemp { Writer_Id = writerId, Combo_Type = combo_type, product_id = product_Id }));

                    ProductPictureTempImplMgr proPicTempMgr = new ProductPictureTempImplMgr("");
                    sqls.Add(proPicTempMgr.SaveFromProPicture(new ProductPictureTemp { writer_Id = writerId, combo_type = combo_type, product_id = product_Id },1));//複製商品說明圖
                    sqls.Add(proPicTempMgr.SaveFromProPicture(new ProductPictureTemp { writer_Id = writerId, combo_type = combo_type, product_id = product_Id }, 2));//複製手機app圖

                    ProductCategorySetTempMgr proCateSetTempMgr = new ProductCategorySetTempMgr("");
                    sqls.Add(proCateSetTempMgr.SaveFromCategorySet(new ProductCategorySetTemp { Writer_Id = writerId, Combo_Type = combo_type, Product_Id = product_Id }));

                    PriceMasterTempMgr priceMasterTempMgr = new PriceMasterTempMgr("");

                    ArrayList specs = new ArrayList();
                    if (combo_type == 1)//單一商品
                    {
                        sqls.Add(priceMasterTempMgr.SaveFromPriceMaster(new PriceMasterTemp { writer_Id = writerId, product_id = product_Id, combo_type = combo_type }));

                        ProductItemTempMgr proItemTempMgr = new ProductItemTempMgr("");
                        var prodItemTemp = new ProductItemTemp { Writer_Id = writerId, Product_Id = product_Id };

                        sqls.Add(proItemTempMgr.SaveFromProItem(prodItemTemp));

                        ProductSpecMgr proSpecMgr = new ProductSpecMgr(connStr);
                        List<ProductSpec> proSpecs = proSpecMgr.Query(new ProductSpec { product_id = uint.Parse(product_Id) });
                        if (proSpecs != null)
                        {
                            ProductSpecTempMgr proSpecTempMgr = new ProductSpecTempMgr("");
                            StringBuilder str;
                            foreach (var item in proSpecs)
                            {
                                sqls.Add(proSpecTempMgr.SaveFromSpec(new ProductSpecTemp { Writer_Id = writerId, product_id = product_Id, spec_id = item.spec_id }));

                                str = new StringBuilder();
                                str.Append(proSpecTempMgr.UpdateCopySpecId(new ProductSpecTemp { Writer_Id = writerId, product_id = product_Id, spec_id = item.spec_id }));
                                str.Append(proItemTempMgr.UpdateCopySpecId(new ProductItemTemp { Writer_Id = writerId, Product_Id = product_Id, Spec_Id_1 = item.spec_id, Spec_Id_2 = item.spec_id }));
                                specs.Add(str.ToString());
                            }
                        }
                        return _pTempDao.CopyProduct(sqls, specs, null, null, null);
                    }
                    else
                    {
                        PriceMasterMgr priceMasterMgr = new PriceMasterMgr("");
                        string selMaster = priceMasterMgr.SelectChild(new PriceMaster { product_id = uint.Parse(product_Id) });

                        string moveMaster = priceMasterTempMgr.SaveFromPriceMasterByMasterId(new PriceMasterTemp { combo_type = combo_type, writer_Id = writerId });

                        ItemPriceTempMgr itemPriceTempMgr = new ItemPriceTempMgr("");
                        string movePrice = itemPriceTempMgr.SaveFromItemPriceByMasterId();

                        ProductComboTempMgr proComboTempMgr = new ProductComboTempMgr("");
                        sqls.Add(proComboTempMgr.SaveFromCombo(new ProductComboTemp { Writer_Id = writerId, Parent_Id = product_Id }));

                        return _pTempDao.CopyProduct(sqls, specs, selMaster, moveMaster, movePrice);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempMgr.CopyProduct-->" + ex.Message, ex);
            }
        }

        #region  獲取temp_status=12,createtor_type=2的供應商商品列表+List<Model.Custom.QueryandVerifyCustom> verifyWaitQuery(Model.Query.QueryVerifyCondition qcCon, out int totalCount)
        /// <summary>
        /// 獲取temp_status=12,createtor_type=2的供應商商品列表 edit by shuangshuang0420j 2014/8/18 15:24
        /// </summary>
        /// <param name="qcCon"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<BLL.gigade.Model.Custom.VenderProductListCustom> verifyWaitQuery(Model.Query.QueryVerifyCondition qcCon, out int totalCount)
        {
            try
            {
                return _pTempDao.verifyWaitQuery(qcCon, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductTempMgr-->verifyWaitQuery-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 獲取供應商新建的商品 + List<Model.ProductTemp> GetProTempByVendor(Model.ProductTemp proTemp)
        /// <summary>
        /// 獲取供應商新建的商品
        /// </summary>
        /// <param name="proTemp"></param>
        /// <returns></returns>
        public List<Model.ProductTemp> GetProTempByVendor(Model.ProductTemp proTemp)
        {
            try
            {
                return _pTempDao.GetProTempByVendor(proTemp);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductTempMgr-->GetProTempByVendor-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 修改信息+string Update(Model.ProductTemp proTemp)
        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="proTemp"></param>
        /// <returns></returns>
        public string Update(Model.ProductTemp proTemp)
        {
            try
            {
                return _pTempDao.Update(proTemp);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductTempMgr-->Update-->" + ex.Message, ex);
            }

        }
        #endregion
        /// <summary>
        /// 刪除供應商商品數據
        /// </summary>
        /// <returns></returns>
        public bool DeleteVendorProductTemp(int writerId, int combo_type, string product_Id)
        {
            try
            {
                ArrayList sqls = new ArrayList();
                sqls.Add(DeleteProductTemp(new ProductTemp { Writer_Id = writerId, Combo_Type = combo_type, Product_Id = product_Id }));

                ProductItemTempMgr proItemTempMgr = new ProductItemTempMgr("");
                sqls.Add(proItemTempMgr.DeleteVendorSql(new ProductItemTemp { Writer_Id = writerId, Product_Id = product_Id }));

                ProductNoticeSetTempMgr proNoticeSetTempMgr = new ProductNoticeSetTempMgr("");
                sqls.Add(proNoticeSetTempMgr.DeleteVendor(new ProductNoticeSetTemp { Writer_Id = writerId, Combo_Type = combo_type, product_id = product_Id }));

                ProductTagSetTempMgr proTagSetTempMgr = new ProductTagSetTempMgr("");
                sqls.Add(proTagSetTempMgr.DeleteVendor(new ProductTagSetTemp { Writer_Id = writerId, Combo_Type = combo_type, product_id = product_Id }));

                ProductSpecTempMgr proSpecTempMgr = new ProductSpecTempMgr("");
                sqls.Add(proSpecTempMgr.TempDeleteByVendor(new ProductSpecTemp { Writer_Id = writerId, product_id = product_Id }));

                ProductPictureTempImplMgr proPicTempMgr = new ProductPictureTempImplMgr("");
                sqls.Add(proPicTempMgr.DeleteByVendor(new ProductPictureTemp { writer_Id = writerId, combo_type = combo_type, product_id = product_Id }));

                ProductCategorySetTempMgr proCateSetTempMgr = new ProductCategorySetTempMgr("");
                sqls.Add(proCateSetTempMgr.TempDeleteByVendor(new ProductCategorySetTemp { Writer_Id = writerId, Combo_Type = combo_type, Product_Id = product_Id }));

                ItemPriceTempMgr itemPriceMgr = new ItemPriceTempMgr("");
                sqls.Add(itemPriceMgr.DeleteByVendor(product_Id.ToString(), combo_type, writerId));

                PriceMasterTempMgr priceMasterTempMgr = new PriceMasterTempMgr("");
                sqls.Add(priceMasterTempMgr.DeleteByVendor(new PriceMasterTemp { writer_Id = writerId, product_id = product_Id, combo_type = combo_type }));

                ProductComboTempMgr proComboTempMgr = new ProductComboTempMgr("");
                sqls.Add(proComboTempMgr.TempDeleteByVendor(new ProductComboTemp { Writer_Id = writerId, Parent_Id = product_Id }));

                MySqlDao mySqlDao = new MySqlDao(connStr);
                return mySqlDao.ExcuteSqls(sqls);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempMg.DeleteTemp-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 刪除product_temp 中的一條數據，專供供應商使用
        /// </summary>
        /// <param name="proTemp"></param>
        /// <returns></returns>
        public string DeleteProductTemp(Model.ProductTemp proTemp)
        {
            try
            {
                return _pTempDao.DeleteVendorTemp(proTemp);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductTempMgr-->DeleteProductTemp-->" + ex.Message, ex);
            }
        }


        public string DeleteVendorTemp(ProductTemp proTemp)
        {
            try
            {
                return _pTempDao.DeleteVendorTemp(proTemp);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductTempMgr-->DeleteVendorTemp-->" + ex.Message, ex);
            }
        }
        #region 新增供應商商品基本資料+int vendorBaseInfoSave(Model.ProductTemp p)
        /// <summary>
        /// 新增供應商商品基本資料
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public string vendorBaseInfoSave(Model.ProductTemp p)
        {
            try
            {
                return _pTempDao.vendorBaseInfoSave(p);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductTempMgr-->vendorBaseInfoSave-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 新增供應商複製商品更新基本資料+int vendorBaseInfoUpdate(Model.ProductTemp p)
        /// <summary>
        /// 新增供應商複製商品更新基本資料
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public int vendorBaseInfoUpdate(Model.ProductTemp p)
        {
            try
            {
                return _pTempDao.vendorBaseInfoUpdate(p);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductTempMgr-->vendorBaseInfoUpdate-->" + ex.Message, ex);
            }
        }
        #endregion

        public int ProductTempUpdateByVendor(Model.ProductTemp proTemp, string page)
        {
            try
            {
                return _pTempDao.ProductTempUpdateByVendor(proTemp, page);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductTempMgr-->ProductTempUpdateByVendor-->" + ex.Message, ex);
            }
        }

        #region 保存供應商單一商品描述
        public bool VendorDescriptionInfoUpdate(ProductTemp proTemp, List<ProductTagSetTemp> tagTemps, List<ProductNoticeSetTemp> noticeTemps)
        {
            try
            {
                ArrayList sqls = new ArrayList();
                sqls.Add(_pTempDao.VendorDescriptionInfoSave(proTemp));
                if (tagTemps != null)
                {
                    IProductTagSetTempImplMgr _productTagSetTempMgr = new ProductTagSetTempMgr(connStr);
                    sqls.Add(_productTagSetTempMgr.DeleteVendor(new ProductTagSetTemp { Writer_Id = proTemp.Writer_Id, Combo_Type = proTemp.Combo_Type, product_id = proTemp.Product_Id }));
                    foreach (var item in tagTemps)
                    {
                        sqls.Add(_productTagSetTempMgr.VendorTagSetTempSave(item));
                    }
                }
                if (noticeTemps != null)
                {
                    IProductNoticeSetTempImplMgr _productNoticeSetTempMgr = new ProductNoticeSetTempMgr(connStr);
                    sqls.Add(_productNoticeSetTempMgr.DeleteVendor(new ProductNoticeSetTemp { Writer_Id = proTemp.Writer_Id, Combo_Type = proTemp.Combo_Type, product_id = proTemp.Product_Id }));
                    foreach (var item in noticeTemps)
                    {
                        sqls.Add(_productNoticeSetTempMgr.Save_Vendor(item));
                    }
                }
                MySqlDao mySqlDao = new MySqlDao(connStr);
                return mySqlDao.ExcuteSqls(sqls);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempMgr.VendorDescriptionInfoSave-->" + ex.Message, ex);
            }

        }

        #endregion

        #region 執行供應商新增商品的修改方法+bool UpdateAchieve(Model.ProductTemp proTemp)
        public bool UpdateAchieve(Model.ProductTemp proTemp)
        {
            try
            {
                return _pTempDao.UpdateAchieve(proTemp);

            }
            catch (Exception ex)
            {

                throw new Exception("ProductTempMgr-->UpdateAchieve-->" + ex.Message, ex);
            }

        }
        #endregion

        public int vendorSpecInfoSave(Model.ProductTemp proTemp)
        {
            try
            {
                return _pTempDao.vendorSpecInfoSave(proTemp);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductTempMgr-->vendorSpecInfoSave-->" + ex.Message, ex);
            }
        }

        public ProductTemp GetVendorProTemp(ProductTemp proTemp)
        {
            try
            {
                return _pTempDao.GetVendorProTemp(proTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempMgr-->GetVendorProTemp-->" + ex.Message, ex);
            }
        }
        #region 保存供應商新增商品到臨時表+int SaveTemp(ProductTemp proTemp)
        /// <summary>
        /// 保存供應商新增商品到臨時表
        /// </summary>
        /// <param name="proTemp"></param>
        /// <returns></returns>
        public int SaveTempByVendor(ProductTemp proTemp)
        {
            try
            {
                return _pTempDao.SaveTempByVendor(proTemp);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductTempMgr-->SaveTemp-->" + ex.Message, ex);
            }

        }
        #endregion

        public int PriceBonusInfoSaveByVendor(Model.ProductTemp proTemp)
        {
            try
            {
                return _pTempDao.PriceBonusInfoSaveByVendor(proTemp);

            }
            catch (Exception ex)
            {

                throw new Exception("ProductTempMgr-->PriceBonusInfoSaveByVendor-->" + ex.Message, ex);
            }
        }

        public bool CategoryInfoUpdateByVendor(Model.ProductTemp proTemp)
        {
            try
            {
                return _pTempDao.CategoryInfoUpdateByVendor(proTemp) > 0;
            }
            catch (Exception ex)
            {

                throw new Exception("ProductTempMgr-->PriceBonusInfoSaveByVendor-->" + ex.Message, ex);
            }
        }


        public bool SaveTemp(ProductTemp proTemp)
        {
            try
            {
                return _pTempDao.SaveTemp(proTemp);

            }
            catch (Exception ex)
            {

                throw new Exception("ProductTempMgr-->SaveTemp(-->" + ex.Message, ex);
            }
        }
        #region Copy 臨時數據新增
        public int VendorSaveFromPro(ProductTemp proTemp)
        {
            return _pTempDao.VendorSaveFromPro(proTemp);
        }

        public bool VendorCopyProduct(int writerId, int combo_type, string old_product_Id, ref string product_id, string product_Ipfrom)
        {
            int Vendor_rid = 0;
            try
            {
                List<ProductTemp> copynumber = _pTempDao.GetProTempByVendor(new ProductTemp { Writer_Id = writerId, Combo_Type = combo_type, Create_Channel = 2, Temp_Status = 13 });
                if (copynumber.Count != 0)
                {
                    foreach (var item in copynumber)
                    {
                        DeleteVendorProductTemp(writerId, combo_type, item.Product_Id);
                    }
                }

                ArrayList sqls = new ArrayList();
                Vendor_rid = VendorSaveFromPro(new ProductTemp { Writer_Id = writerId, Combo_Type = combo_type, Product_Id = old_product_Id, Product_Ipfrom = product_Ipfrom, Product_Createdate = uint.Parse(Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString()) });
                product_id = "T" + Vendor_rid;
                ProductNoticeSetTempMgr proNoticeSetTempMgr = new ProductNoticeSetTempMgr("");
                sqls.Add(proNoticeSetTempMgr.VendorSaveFromProNotice(new ProductNoticeSetTemp { Writer_Id = writerId, Combo_Type = combo_type, product_id = product_id }, old_product_Id));

                ProductTagSetTempMgr proTagSetTempMgr = new ProductTagSetTempMgr("");
                sqls.Add(proTagSetTempMgr.VendorSaveFromTag(new ProductTagSetTemp { Writer_Id = writerId, Combo_Type = combo_type, product_id = product_id }, old_product_Id));

                ProductPictureTempImplMgr proPicTempMgr = new ProductPictureTempImplMgr("");
                sqls.Add(proPicTempMgr.VendorSaveFromProPicture(new ProductPictureTemp { writer_Id = writerId, combo_type = combo_type, product_id = product_id }, old_product_Id));

                ProductCategorySetTempMgr proCateSetTempMgr = new ProductCategorySetTempMgr("");
                sqls.Add(proCateSetTempMgr.VendorSaveFromCategorySet(new ProductCategorySetTemp { Writer_Id = writerId, Combo_Type = combo_type, Product_Id = product_id }, old_product_Id));

                PriceMasterTempMgr priceMasterTempMgr = new PriceMasterTempMgr("");
                ArrayList specs = new ArrayList();
                if (combo_type == 1)//單一商品
                {
                    sqls.Add(priceMasterTempMgr.VendorSaveFromPriceMaster(new PriceMasterTemp { writer_Id = writerId, product_id = product_id, combo_type = combo_type }, old_product_Id));
                     
                    ProductItemTempMgr proItemTempMgr = new ProductItemTempMgr("");
                    sqls.Add(proItemTempMgr.VendorSaveFromProItem(new ProductItemTemp { Writer_Id = writerId, Product_Id = product_id }, old_product_Id));

                    //因為複製的是臨時表所以換成productspectemp
                    ProductSpecTempMgr ProSpecTemp = new ProductSpecTempMgr(connStr);
                    //查詢出要複製的商品裡面的規格是否為空
                    List<ProductSpecTemp> proSpecs = ProSpecTemp.VendorQuery(new ProductSpecTemp { product_id = old_product_Id });
                    //ProductSpecMgr proSpecMgr = new ProductSpecMgr(connStr);
                    if (proSpecs != null)
                    {
                        ProductSpecTempMgr proSpecTempMgr = new ProductSpecTempMgr(connStr);
                        StringBuilder str;
                        foreach (var item in proSpecs)
                        {
                            str = new StringBuilder();
                            str.Append(proSpecTempMgr.VendorSaveFromSpec(new ProductSpecTemp { Writer_Id = writerId, product_id = product_id, spec_id = item.spec_id }, old_product_Id));
                          
                            str.Append(proItemTempMgr.VendorUpdateCopySpecId(new ProductItemTemp { Writer_Id = writerId, Product_Id = product_id, Spec_Id_1 = item.spec_id, Spec_Id_2 = item.spec_id }));
                            specs.Add(str.ToString());
                        }
                    }
                    return _pTempDao.VendorCopyProduct(sqls, specs, null, null, null);
                }
                else
                {
                    PriceMasterMgr priceMasterMgr = new PriceMasterMgr("");
                    string selMaster = priceMasterMgr.VendorSelectChild(new PriceMasterTemp { product_id = old_product_Id });
                    string moveMaster = priceMasterTempMgr.VendorSaveFromPriceMaster(new PriceMasterTemp { combo_type = combo_type, product_id = product_id, writer_Id = writerId }, old_product_Id);

                    ItemPriceTempMgr itemPriceTempMgr = new ItemPriceTempMgr("");
                    string movePrice = itemPriceTempMgr.SaveFromItemPriceByMasterId();
                    ProductComboTempMgr proComboTempMgr = new ProductComboTempMgr("");
                    sqls.Add(proComboTempMgr.VendorSaveFromCombo(new ProductComboTemp { Writer_Id = writerId, Parent_Id = product_id }, old_product_Id));
                    return _pTempDao.VendorCopyProduct(sqls, specs, selMaster, moveMaster, movePrice);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTempMgr.VendorCopyProduct-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 與供應商商品相關
        /// <summary>
        /// 管理員核可供應商建立的商品時將商品信息由臨時表移動到正式表
        /// </summary>
        /// <param name="proTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_MoveProduct(Model.ProductTemp proTemp)
        {
            return _pTempDao.Vendor_MoveProduct(proTemp);
        }

        /// <summary>
        /// 管理員核可供應商建立的商品時將商品信息由臨時表刪除
        /// </summary>
        /// <param name="proTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_Delete(ProductTemp proTemp)
        {
            return _pTempDao.Vendor_Delete(proTemp);
        }

        /// <summary>
        /// 處理product_combo_temp 和 price_mater_temp表裡面的臨時id
        /// </summary>
        /// <param name="proTemp">商品臨時表對象</param>
        /// <returns>此操作的sql語句</returns>
        public string VendorEditCM(ProductTemp proTemp)
        {
            return _pTempDao.VendorEditCM(proTemp);
        }
        #endregion

        /// <summary>
        /// 取消商品送核
        /// </summary>
        /// <param name="productTemp"></param>
        public bool CancelVerify(ProductTemp productTemp)
        {
            return _pTempDao.CancelVerify(productTemp);
        }

        public int GetDefaultArriveDays(ProductTemp temp)
        {
            return _pTempDao.GetDefaultArriveDays(temp);
        }

    }
}
