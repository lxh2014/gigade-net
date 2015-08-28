/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：Combination 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/1/13 13:35:21 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using ProductMigration.Model;
using BLL.gigade.Model;
using System.Collections;

namespace ProductMigration
{
    public class Combination
    {
        private IVendorBrandImplMgr _vendorBrandMgr;
        private IVendorImplMgr _vendorMgr;
        private IProductImplMgr _productMgr;
        private IProductItemImplMgr _productItemMgr;
        private ISiteImplMgr _siteMgr;
        private IProductMigrationImplMgr _productMigrationMgr;
        private IProductComboImplMgr _productComboMgr;
        private IProductCategoryImplMgr _cateMgr;
        private IProductCategorySetImplMgr _productCategorySetMgr;
        private IProductNoticeSetImplMgr _productNoticeSetMgr;
        private IProductTagSetImplMgr _productTagSetMgr;
        private IProductPictureImplMgr _productPictureMgr;
        private IProductStatusHistoryImplMgr _productStatusHistoryMgr;
        private IPriceMasterImplMgr _priceMasterMgr;
        private string strConn;
        private MainForm form;
        public Combination(string connectionString, MainForm form)
        {
            this.strConn = connectionString;
            this.form = form;
        }

        public void PrepareData(List<CombinationExcel> source)
        {
            if (source != null)
            {
                source.ForEach(m => m.Validate());
                var parents = source.Where(m => m.combination.Trim() != "299" && string.IsNullOrEmpty(m.msg));

                _productMgr = new ProductMgr(strConn);
                _productItemMgr = new ProductItemMgr(strConn);
                _vendorMgr = new VendorMgr(strConn);
                _vendorBrandMgr = new VendorBrandMgr(strConn);
                _siteMgr = new SiteMgr(strConn);
                _productMigrationMgr = new ProductMigrationMgr(strConn);
                _productComboMgr = new ProductComboMgr(strConn);
                _cateMgr = new ProductCategoryMgr(strConn);
                _productCategorySetMgr = new ProductCategorySetMgr(strConn);
                _productNoticeSetMgr = new ProductNoticeSetMgr(strConn);
                _productTagSetMgr = new ProductTagSetMgr(strConn);
                _productPictureMgr = new ProductPictureMgr(strConn);
                _productStatusHistoryMgr = new ProductStatusHistoryMgr(strConn);

                foreach (var parent in parents)
                {
                    form.change(1);
                    uint product_id = uint.Parse(parent.product_id);
                    #region Product

                    ProductMigrationMap prodMigra = _productMigrationMgr.GetSingle(new ProductMigrationMap { temp_id = parent.is_exist.ToUpper().Trim().Equals("OLD") ? parent.product_id : parent.temp_id });
                    if (prodMigra != null)
                    {
                        parent.msg = "此記錄暫時編號(temp_id)已經存在;";
                        continue;
                    }

                    Product newPro = parent.is_exist.ToUpper().Trim() == "OLD" ? _productMgr.Query(new Product { Product_Id = product_id }).FirstOrDefault() : new Product();
                    if (newPro == null)
                    {
                        parent.msg = "商品不存在";
                        continue;
                    }
                    newPro.Product_Name = parent.product_name.Trim();
                    VendorBrand brand = _vendorBrandMgr.GetProductBrand(new VendorBrand { Brand_Name = parent.brand_name.Trim() });
                    if (brand == null)
                    {
                        parent.msg = "品牌不存在";
                        continue;
                    }
                    newPro.Brand_Id = brand.Brand_Id;
                    switch (parent.combination.Trim())
                    {
                        case "2":
                            newPro.Combination = 2; break;
                        case "3":
                            newPro.Combination = 3; break;
                        case "4":
                            newPro.Combination = 4; break;
                        default: break;
                    }
                    switch (parent.status.Trim().ToUpper())
                    {
                        case "SAME":
                            break;
                        case "ON":
                            newPro.Product_Status = 5; break;
                        case "OFF":
                            newPro.Product_Status = 6; break;
                        case "NEW":
                            newPro.Product_Status = 0; break;
                        default:
                            break;
                    }
                    newPro.Price_type = 1;
                    newPro.Product_Spec = 0;
                    newPro.Spec_Title_1 = string.Empty;
                    newPro.Spec_Title_2 = string.Empty;
                    newPro.Ignore_Stock = 0;// parent.ignore_stock.ToUpper().Trim() == "Y" ? 1 : 0;
                    newPro.Shortage = 0;// parent.shortage.ToUpper().Trim() == "Y" ? 1 : 0;
                    newPro.Cate_Id = parent.cate_id.Trim();
                    var vendor = _vendorMgr.GetSingle(new Vendor { vendor_id = brand.Vendor_Id });
                    if (vendor == null)
                    {
                        parent.msg = "供應商不存在";
                        continue;
                    }
                    newPro.user_id = vendor.product_manage;
                    if (!string.IsNullOrEmpty(parent.service_fee))
                    { 
                        uint bag_check_money=0;
                        uint.TryParse(parent.service_fee,out bag_check_money);
                        newPro.Bag_Check_Money = bag_check_money;
                    }
                    #endregion

                    bool result = true;
                    ArrayList sqls = new ArrayList();
                    #region PriceMaster

                    Site site = _siteMgr.Query(new Site { Site_Name = parent.site.Trim() }).FirstOrDefault();
                    if (site == null)
                    {
                        parent.msg = "價格檔站臺不存在";
                        result = false;
                        break;
                    }
                    List<PriceMaster> priceMasters = new List<PriceMaster>();
                    //首先默認加入一筆吉甲地站臺
                    PriceMaster newPriceMaster = new PriceMaster { site_id = 1, user_level = 1, same_price = 1, accumulated_bonus = 1, default_bonus_percent = 1, bonus_percent = 1 };
                    newPriceMaster.product_name = newPro.Product_Name;
                    newPriceMaster.price_status = 1;

                    var item = _productItemMgr.Query(new ProductItem { Product_Id = newPro.Product_Id }).FirstOrDefault();
                    if (item == null)
                    {
                        parent.msg = "商品細項不存在";
                        continue;
                    }
                    #region 成本

                    int cost = 0;
                    if (!string.IsNullOrEmpty(parent.cost))
                    {
                        int.TryParse(parent.cost, out cost);
                    }
                    else
                    {
                        cost = Convert.ToInt32(item.Item_Cost);
                    }
                    newPriceMaster.cost = cost;
                    #endregion

                    #region 售價

                    int price = 0;
                    if (!string.IsNullOrEmpty(parent.price))
                    {
                        int.TryParse(parent.price, out price);
                    }
                    else
                    {
                        price = Convert.ToInt32(item.Item_Money);
                    }
                    newPriceMaster.price = price;
                    #endregion

                    newPriceMaster.event_price = Convert.ToInt32(item.Event_Item_Money);
                    newPriceMaster.event_start = item.Event_Product_Start;
                    newPriceMaster.event_end = item.Event_Product_End;
                    priceMasters.Add(newPriceMaster);

                    if (site.Site_Id != 1)
                    {
                        //如果站臺不是吉甲地就把吉甲地站臺的價格狀態改為4（下架）
                        priceMasters[0].price_status = 4;
                        //再加入非吉甲地站臺
                        newPriceMaster = new PriceMaster { site_id = site.Site_Id, user_level = 1, same_price = 1, accumulated_bonus = 1, default_bonus_percent = 1, bonus_percent = 1 };
                        newPriceMaster.product_name = newPro.Product_Name;
                        newPriceMaster.price_status = 1;
                        #region 成本

                        cost = 0;
                        if (!string.IsNullOrEmpty(parent.cost))
                        {
                            int.TryParse(parent.cost, out cost);
                        }
                        else
                        {
                            cost = Convert.ToInt32(item.Item_Cost);
                        }
                        newPriceMaster.cost = cost;
                        #endregion

                        #region 售價

                        price = 0;
                        if (!string.IsNullOrEmpty(parent.price))
                        {
                            int.TryParse(parent.price, out price);
                        }
                        else
                        {
                            price = Convert.ToInt32(item.Item_Money);
                        }
                        newPriceMaster.price = price;
                        #endregion
                        newPriceMaster.event_price = Convert.ToInt32(item.Event_Item_Money);
                        newPriceMaster.event_start = item.Event_Product_Start;
                        newPriceMaster.event_end = item.Event_Product_End;
                        priceMasters.Add(newPriceMaster);
                    }

                    #region 其他站臺價格

                    var child = source.Where(m =>
                                    m.combination.Trim() == "299"
                                    && m.formula.ToUpper().Trim() == parent.temp_id.ToUpper().Trim()
                                    && string.IsNullOrEmpty(m.msg)
                                    && m.new_old.ToUpper().Trim() == "NEW"
                                    && m.formula.ToUpper().Trim().StartsWith("P"));
                    if (child != null)
                    {
                        foreach (var c in child)
                        {
                            item = _productItemMgr.Query(new ProductItem { Product_Id = uint.Parse(c.product_id) }).FirstOrDefault();
                            if (item == null)
                            {
                                c.msg = "價格檔商品細項不存在";
                                result = false;
                                break;
                            }
                            site = _siteMgr.Query(new Site { Site_Name = c.site.Trim() }).FirstOrDefault();
                            if (site == null)
                            {
                                c.msg = "站臺不存在";
                                result = false;
                                break;
                            }
                            if (priceMasters.Exists(m => m.site_id == site.Site_Id))
                            {
                                c.msg = "站臺重複";
                                result = false;
                                break;
                            }
                            newPriceMaster = new PriceMaster { user_level = 1, same_price = 1, accumulated_bonus = 1, default_bonus_percent = 1, bonus_percent = 1 };
                            newPriceMaster.product_name = c.product_name.Trim();
                            newPriceMaster.site_id = site.Site_Id;
                            newPriceMaster.price_status = 1;
                            #region 成本

                            cost = 0;
                            if (!string.IsNullOrEmpty(c.cost))
                            {
                                int.TryParse(c.cost, out cost);
                            }
                            else
                            {
                                cost = Convert.ToInt32(item.Item_Cost);
                            }
                            newPriceMaster.cost = cost;
                            #endregion

                            #region 售價

                            price = 0;
                            if (!string.IsNullOrEmpty(c.price))
                            {
                                int.TryParse(c.price, out price);
                            }
                            else
                            {
                                price = Convert.ToInt32(item.Item_Money);
                            }
                            newPriceMaster.price = price;
                            #endregion
                            newPriceMaster.event_price = Convert.ToInt32(item.Event_Item_Money);
                            newPriceMaster.event_start = item.Event_Product_Start;
                            newPriceMaster.event_end = item.Event_Product_End;
                            priceMasters.Add(newPriceMaster);

                            if (site.Site_Name.Trim().ToLower() == "chinatrust")
                            {
                                ArrayList category_set = CategorySet(c, brand.Brand_Id);
                                if (category_set == null)
                                {
                                    result = false;
                                    break;
                                }
                                else
                                {
                                    sqls.AddRange(category_set);
                                }
                            }
                        }
                        if (!result) continue;
                    }
                    #endregion

                    #endregion

                    #region ProductCombo

                    string[] strIds = parent.formula.IndexOf(",") != -1 ? parent.formula.Trim().Split(',') : new string[] { parent.formula.Trim() };
                    string[] nums = parent.number.IndexOf(",") != -1 ? parent.number.Trim().Split(',') : new string[] { parent.number.Trim() };

                    List<Product> children = new List<Product>();
                    ProductCombo tmp;
                    for (int i = 0; i < strIds.Length; i++)
                    {
                        tmp = new ProductCombo();
                        if (parent.combination.Trim() != "2")
                        {
                            tmp.Buy_Limit = parent.buylimit.Trim().ToUpper() == "Y" ? 1 : 0;
                        }
                        else
                        {
                            tmp.S_Must_Buy = int.Parse(i >= nums.Length ? nums[nums.Length - 1] : nums[i]);
                        }
                        if (parent.combination.Trim() == "3")//為3任選時 數量為G_Must_Buy
                        {
                            tmp.G_Must_Buy = int.Parse(nums[0]);
                        }
                        switch (parent.new_old.Trim().ToUpper())
                        {
                            case "NEW":
                                var map = _productMigrationMgr.GetSingle(new ProductMigrationMap { temp_id = strIds[i] });
                                if (map == null)
                                {
                                    parent.msg = "原料編號未找到對應";
                                    break;
                                }
                                tmp.Child_Id = Convert.ToInt32(map.product_id);
                                break;
                            case "OLD":
                                tmp.Child_Id = int.Parse(strIds[i]);
                                break;
                            case "BOTH":
                                if (strIds[i].ToUpper().Trim().StartsWith("P"))
                                {
                                    var tMap = _productMigrationMgr.GetSingle(new ProductMigrationMap { temp_id = strIds[i] });
                                    if (tMap == null)
                                    {
                                        parent.msg = "原料編號未找到對應";
                                        break;
                                    }
                                    tmp.Child_Id = Convert.ToInt32(tMap.product_id);
                                }
                                else
                                {
                                    tmp.Child_Id = int.Parse(strIds[i]);
                                }
                                break;
                        }
                        if (tmp.Child_Id == 0)
                        {
                            result = false;
                            break;
                        }
                        else
                        {
                            Product tmpPro = _productMgr.Query(new Product { Product_Id = Convert.ToUInt32(tmp.Child_Id) }).FirstOrDefault();
                            if (tmpPro == null)
                            {
                                parent.msg = strIds[i] + "不存在";
                                result = false;
                                break;
                            }
                            if (tmpPro.Product_Status == 0 || tmpPro.Product_Status ==1)
                            {
                                parent.msg = strIds[i] + "商品状态不為新增、申請審核";
                                result = false;
                                break;
                            }
                            if (tmpPro.Combination != 0 && tmpPro.Combination != 1)
                            {
                                parent.msg = strIds[i] + "不是單一商品";
                                result = false;
                                break;
                            }
                            switch (newPro.Product_Freight_Set)
                            {
                                case 1:
                                case 3:
                                    if (tmpPro.Product_Freight_Set != 1 && tmpPro.Product_Freight_Set != 3) 
                                    {
                                        parent.msg = strIds[i] + "运费模式与主商品不匹配";
                                        result = false;
                                    }
                                    break;
                                case 2:
                                case 4:
                                    if (tmpPro.Product_Freight_Set != 2 && tmpPro.Product_Freight_Set != 4)
                                    {
                                        parent.msg = strIds[i] + "运费模式与主商品不匹配";
                                        result = false;
                                    }
                                    break;
                                case 5:
                                case 6:
                                    if (tmpPro.Product_Freight_Set != 5 && tmpPro.Product_Freight_Set != 6)
                                    {
                                        parent.msg = strIds[i] + "运费模式与主商品不匹配";
                                        result = false;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            if (!result) break;
                            children.Add(tmpPro);
                        }
                        sqls.Add(_productComboMgr.Save(tmp));
                    }
                    if (!result) continue;
                    #endregion

                    #region product_category_set
                    if (!string.IsNullOrEmpty(parent.display))
                    {
                        ArrayList category_set = CategorySet(parent, brand.Brand_Id);
                        if (category_set == null)
                            continue;
                        else
                        {
                            foreach (var set in category_set)
                            {
                                if (!sqls.Contains(set))
                                {
                                    sqls.Add(set);
                                }
                            }
                        }
                    }
                    else
                    {
                        sqls.Add(_productCategorySetMgr.SaveFromOtherPro(new ProductCategorySet { Product_Id = product_id }));
                    }
                    #endregion

                    #region product_migration_map

                    ProductMigrationMap pMap = new ProductMigrationMap();
                    if (parent.is_exist.ToUpper().Equals("OLD"))
                    {
                        pMap.temp_id = parent.product_id;
                    }
                    else if (parent.is_exist.ToUpper().Equals("NEW"))
                    {
                        pMap.temp_id = parent.temp_id;
                    }
                    sqls.Add(_productMigrationMgr.SaveNoPrid(pMap));

                    #endregion

                    #region notice tag picture

                    sqls.Add(_productNoticeSetMgr.SaveFromOtherPro(new ProductNoticeSet { product_id = product_id }));
                    sqls.Add(_productTagSetMgr.SaveFromOtherPro(new ProductTagSet { product_id = product_id }));
                    sqls.Add(_productPictureMgr.SaveFromOtherPro(new ProductPicture { product_id = Convert.ToInt32(product_id) }));
                    sqls.Add(_productStatusHistoryMgr.SaveNoProductId(new ProductStatusHistory { type = 7, product_status = Convert.ToInt32(newPro.Product_Status) }));
                    #endregion

                    string str = string.Empty;
                    if (_productMgr.ProductMigration(newPro, priceMasters, null, null, sqls, null))
                    {
                        str = "匯入成功";
                    }
                    else
                    {
                        str = "保存至數據庫失敗";
                    }
                    parent.msg = str;
                    source.FindAll(m =>
                                    m.combination.Trim() == "299"
                                    && m.formula.ToUpper().Trim() == parent.temp_id.ToUpper().Trim()
                                    && string.IsNullOrEmpty(m.msg)
                                    && m.new_old.ToUpper().Trim() == "NEW"
                                    && m.formula.ToUpper().Trim().StartsWith("P")).ForEach(m => m.msg = str);
                }

                #region 其他站臺價格

                var pChild = source.Where(m => m.combination.Trim() == "299" && m.new_old.ToUpper().Trim() == "OLD" && string.IsNullOrEmpty(m.msg));
                if (pChild != null)
                {
                    ProductItem item;
                    PriceMaster newPriceMaster;
                    Site site;
                    _priceMasterMgr = new PriceMasterMgr(strConn);
                    foreach (var c in pChild)
                    {
                        ProductMigrationMap prodMigra = _productMigrationMgr.GetSingle(new ProductMigrationMap { temp_id = c.formula.Trim() });
                        if (prodMigra == null)
                        {
                            c.msg = "原料編號對照不存在";
                            continue;
                        }
                        item = _productItemMgr.Query(new ProductItem { Product_Id = uint.Parse(c.product_id) }).FirstOrDefault();
                        if (item == null)
                        {
                            c.msg = "價格檔商品細項不存在";
                            continue;
                        }
                        site = _siteMgr.Query(new Site { Site_Name = c.site.Trim() }).FirstOrDefault();
                        if (site == null)
                        {
                            c.msg = "站臺不存在";
                            continue;
                        }
                        newPriceMaster = new PriceMaster { user_level = 1, same_price = 1, accumulated_bonus = 1, default_bonus_percent = 1, bonus_percent = 1 };
                        newPriceMaster.product_name = c.product_name.Trim();
                        newPriceMaster.product_id = prodMigra.product_id;
                        newPriceMaster.child_id = Convert.ToInt32(prodMigra.product_id);
                        newPriceMaster.site_id = site.Site_Id;
                        newPriceMaster.price_status = 1;
                        #region 成本

                        int cost = 0;
                        if (!string.IsNullOrEmpty(c.cost))
                        {
                            int.TryParse(c.cost, out cost);
                        }
                        else
                        {
                            cost = Convert.ToInt32(item.Item_Cost);
                        }
                        newPriceMaster.cost = cost;
                        #endregion

                        #region 售價

                        int price = 0;
                        if (!string.IsNullOrEmpty(c.price))
                        {
                            int.TryParse(c.price, out price);
                        }
                        else
                        {
                            price = Convert.ToInt32(item.Item_Money);
                        }
                        newPriceMaster.price = price;
                        #endregion
                        newPriceMaster.event_price = Convert.ToInt32(item.Event_Item_Money);
                        newPriceMaster.event_start = item.Event_Product_Start;
                        newPriceMaster.event_end = item.Event_Product_End;

                        ArrayList category_set = new ArrayList();
                        if (site.Site_Name.Trim().ToLower() == "chinatrust")
                        {
                            Product pro= _productMgr.Query(new Product { Product_Id = item.Product_Id }).FirstOrDefault();
                            if (pro == null)
                            {
                                c.msg = "商品不存在";
                                continue;
                            }
                            #region 更新該站臺商品的 product_category_set
                            
                            bool result = true;
                            System.Text.RegularExpressions.Regex regx = new System.Text.RegularExpressions.Regex("^[0-9]*$");
                            string[] cateArray = c.display.Split(',');
                            foreach (string strcate in cateArray)
                            {
                                if (!string.IsNullOrEmpty(strcate))
                                {
                                    if (regx.IsMatch(strcate))
                                    {
                                        ProductCategory query = _cateMgr.QueryAll(new ProductCategory { category_id = uint.Parse(strcate) }).FirstOrDefault();
                                        if (query == null || strcate.Equals("0"))
                                        {
                                            c.msg = "display:" + strcate + " 不存在;";
                                            result = false;
                                            break;
                                        }
                                        else
                                        {
                                            ProductCategorySet category = new ProductCategorySet { Brand_Id = pro.Brand_Id, Category_Id = uint.Parse(strcate), Product_Id = prodMigra.product_id };
                                            if (_productCategorySetMgr.Query(category).FirstOrDefault() == null)
                                            {
                                                category_set.Add(_productCategorySetMgr.Save(category));
                                            }
                                            else
                                            {
                                                c.msg = "display:" + strcate + " 已存在;";
                                                result = false;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        c.msg = "display:" + strcate + " 格式不正確 ";
                                        result = false;
                                        break;
                                    }
                                }
                            }
                            if (!result) continue;
                            #endregion
                        }

                        string str = string.Empty;
                        if (_priceMasterMgr.Save(newPriceMaster, null, category_set, ref str) > 0)
                        {
                            c.msg = "匯入成功";
                        }
                        else
                        {
                            c.msg = string.IsNullOrEmpty(str) ? "保存至數據庫失敗" : str;
                        }
                    }
                }
                #endregion
            }
        }

        private ArrayList CategorySet(CombinationExcel source, uint brand_id)
        {
            ArrayList sqls = new ArrayList();
            if (!string.IsNullOrEmpty(source.display))
            {
                bool result = true;
                _productCategorySetMgr = new ProductCategorySetMgr(strConn);

                System.Text.RegularExpressions.Regex regx = new System.Text.RegularExpressions.Regex("^[0-9]*$");
                string[] cateArray = source.display.Split(',');
                foreach (string strcate in cateArray)
                {
                    if (!string.IsNullOrEmpty(strcate))
                    {
                        if (regx.IsMatch(strcate))
                        {
                            ProductCategory query = _cateMgr.QueryAll(new ProductCategory { category_id = uint.Parse(strcate) }).FirstOrDefault();
                            if (query == null || strcate.Equals("0"))
                            {
                                source.msg = "display:" + strcate + " 不存在;";
                                result = false;
                                break;
                            }
                            else
                            {
                                sqls.Add(_productCategorySetMgr.SaveNoPrid(new ProductCategorySet { Brand_Id = brand_id, Category_Id = uint.Parse(strcate) }));
                            }
                        }
                        else
                        {
                            source.msg = "display:" + strcate + " 格式不正確 ";
                            result = false;
                            break;
                        }
                    }
                }
                return result ? sqls : null;
            }
            return sqls;
        }
    }
}
