using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using BLL.gigade.Model;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using System.Data;
using ProductMigration.Model;


namespace ProductMigration
{
    public partial class DataOperation
    {
        protected List<Model.MigrationDataSet> mds = null;
        public string connectionString { get; set; }
        public Product prod { get; set; }
        public ProductItem pItem { get; set; }
        public PriceMaster pM { get; set; }
        public ItemPrice iPrice { get; set; }
        public ProductCategorySet categorySet { get; set; }
        public ProductMigrationMap pMap { get; set; }
        public IProductImplMgr _prodMgr = null;
        public IPriceMasterImplMgr _priceMgr = null;
        public IProductItemImplMgr _prodItemMgr = null;
        public IItemPriceImplMgr _itemPriceMgr = null;
        public IVendorBrandImplMgr _vendorBrandMgr = null;
        public IVendorImplMgr _vendorMgr = null;
        public IProductMigrationImplMgr _pMap = null;
        public IProductCategorySetImplMgr _productCategorySetMgr = null;
        public IProductNoticeSetImplMgr _productNoticeSetMgr = null;
        public IProductTagSetImplMgr _productTagSetMgr = null;
        public IProductSpecImplMgr _prodSpecMgr = null;
        public IProductPictureImplMgr _productPictureMgr = null;
        public IProductStatusHistoryImplMgr _proStatusHistoryMgr = null;
        public IProductSpecImplMgr _proSpecMgr = null;
        public ISiteImplMgr _siteMgr = null;
        public MainForm form;
        public DataOperation(List<Model.MigrationDataSet> mds, MainForm form)
        {
            this.mds = mds;
            this.connectionString = System.Configuration.ConfigurationSettings.AppSettings["MySqlConnectionString"];
            prod = new Product();
            pItem = new ProductItem();
            pM = new PriceMaster();
            iPrice = new ItemPrice();
            categorySet = new ProductCategorySet();
            pMap = new ProductMigrationMap();
            _prodMgr = new ProductMgr(this.connectionString);
            _priceMgr = new PriceMasterMgr(this.connectionString);
            _prodItemMgr = new ProductItemMgr(this.connectionString);
            _itemPriceMgr = new ItemPriceMgr(this.connectionString);
            _vendorBrandMgr = new VendorBrandMgr(this.connectionString);
            _vendorMgr = new VendorMgr(this.connectionString);
            _pMap = new ProductMigrationMgr(this.connectionString);
            _productCategorySetMgr = new ProductCategorySetMgr(this.connectionString);
            _productNoticeSetMgr = new ProductNoticeSetMgr(this.connectionString);
            _productTagSetMgr = new ProductTagSetMgr(this.connectionString);
            _productPictureMgr = new ProductPictureMgr(this.connectionString);
            _proStatusHistoryMgr = new ProductStatusHistoryMgr(connectionString);
            _proSpecMgr = new ProductSpecMgr(connectionString);
            _siteMgr = new SiteMgr(connectionString);
            this.form = form;

        }
        public virtual bool Exec() { return false; }

    }

    public class operationFactory
    {
        public static DataOperation CreateOperation(string operType, List<Model.MigrationDataSet> mds, MainForm form)
        {
            DataOperation dOper = null;
            switch (operType)
            {
                case "old": dOper = new BeginUpdate(mds, form); break;
                case "new": dOper = new BeginInsert(mds, form); break;
                case "199": dOper = new PriceInsert(mds, form); break;
                default:
                    break;
            }
            return dOper;
        }
    }

    public class BeginUpdate : DataOperation
    {
        public BeginUpdate(List<Model.MigrationDataSet> mds, MainForm form) : base(mds, form) { }

        public override bool Exec()
        {
            bool result = true;
            try
            {
                foreach (MigrationDataSet item in mds)
                {
                    form.change(1);
                    if (!item.Combination.Equals("199") && !item.Product_id.Equals("0"))
                    {
                        pMap = _pMap.GetSingle(new ProductMigrationMap { temp_id = item.Product_id });
                        if (pMap != null)
                        {
                            item.OutMessage = "此記錄暫時編號(product_id)已經存在;";
                            continue;
                        }
                        else
                        {
                            pMap = new ProductMigrationMap();
                        }
                    }


                    ArrayList insertList = new ArrayList();


                    //Product
                    prod = _prodMgr.Query(new Product { Product_Id = uint.Parse(item.Product_id) }).FirstOrDefault();
                    //product.user_id：同此品牌廠商之verdor.product_manage
                    if (!string.IsNullOrEmpty(item.Brand_name))
                    {
                        VendorBrand vb = _vendorBrandMgr.GetProductBrand(new VendorBrand { Brand_Id = uint.Parse(item.Brand_name) });
                        Vendor v = _vendorMgr.GetSingle(new Vendor { vendor_id = vb.Vendor_Id });
                        prod.user_id = v.product_manage;
                    }
                    if (!string.IsNullOrEmpty(item.Brand_name)) prod.Brand_Id = uint.Parse(item.Brand_name);
                    prod.Ignore_Stock = 0;        //默認false
                    if (!string.IsNullOrEmpty(item.Cate_id)) prod.Cate_Id = item.Cate_id;
                    if (!string.IsNullOrEmpty(item.Status)) prod.Product_Status = uint.Parse(item.Status);
                    if (!string.IsNullOrEmpty(item.Freight_set)) prod.Product_Freight_Set = uint.Parse(item.Freight_set);
                    if (!string.IsNullOrEmpty(item.Product_mode)) prod.Product_Mode = uint.Parse(item.Product_mode);
                    if (!string.IsNullOrEmpty(item.Tax)) prod.Tax_Type = int.Parse(item.Tax);
                    prod.Shortage = 0;    //默認false
                    if (!string.IsNullOrEmpty(item.Product_name)) prod.Product_Name = item.Product_name;
                    prod.Product_Createdate = uint.Parse(BLL.gigade.Common.CommonFunction.GetPHPTime().ToString());
                    if (!string.IsNullOrEmpty(item.Service_Fee))
                    {
                        uint bag_check_money = 0;
                        uint.TryParse(item.Service_Fee, out bag_check_money);
                        prod.Bag_Check_Money = bag_check_money;
                    }
                    prod.Combination = 1;

                    //product_item
                    List<ProductItem> piList = _prodItemMgr.Query(new ProductItem { Product_Id = uint.Parse(item.Product_id) });
                    if (piList.Count > 0)
                    {
                        piList.ForEach(rec =>
                        {
                            if (!string.IsNullOrEmpty(item.Stock)) rec.Item_Stock = int.Parse(item.Stock);
                            if (!string.IsNullOrEmpty(item.Price)) rec.Item_Money = uint.Parse(item.Price);
                            if (!string.IsNullOrEmpty(item.Cost)) rec.Item_Cost = uint.Parse(item.Cost);
                        });
                    }

                    List<PriceMaster> pmList = new List<PriceMaster>();
                    List<PriceMaster> pmaterList = _priceMgr.PriceMasterQuery(new PriceMaster { product_id = uint.Parse(item.Product_id) });
                    pM.bonus_percent = 1;
                    pM.default_bonus_percent = 1;
                    pM.price_status = 1;
                    if (string.IsNullOrEmpty(item.Price))
                    {
                        pM.price = int.Parse(piList.Min(rec => rec.Item_Money).ToString());
                    }
                    else
                    {
                        pM.price = int.Parse(item.Price);
                    }
                    if (string.IsNullOrEmpty(item.Cost))
                    {
                        pM.cost = int.Parse(piList.Min(rec => rec.Item_Cost).ToString());
                    }
                    else
                    {
                        pM.cost = int.Parse(item.Cost);
                    }
                    pM.user_level = 1;
                    pM.user_id = 0;
                    pM.product_name = prod.Product_Name;
                    pM.same_price = piList.GroupBy(rec => rec.Item_Money).Count() <= 1 ? 1 : 0;
                    pM.child_id = 0;
                    prod.Product_Price_List = uint.Parse(pM.price.ToString());
                    if (pmaterList.Count == 0)
                    {
                        //price_master
                        if (item.Site != "1")//若不為吉甲地，則將吉甲地站臺也加入
                        {
                            //先把添加非吉甲地的站臺
                            pM.site_id = uint.Parse(item.Site);
                            pmList.Add(pM);
                            //再添加吉甲地站臺且價格狀態為4（下架）
                            PriceMaster pMgigade = pM.Clone() as PriceMaster;
                            pMgigade.site_id = 1;
                            pMgigade.price_status = 4;
                            pmList.Add(pMgigade);
                        }
                        else
                        {
                            pM.site_id = 1;
                            pmList.Add(pM);
                        }
                    }
                    else
                    {
                        #region 舊商品price_master中有值
                        PriceMaster pmaster = pmaterList.Where(rec => rec.site_id == 1).FirstOrDefault();
                        List<PriceMaster> oldPriceList = pmaterList.Where(rec => rec.site_id != 1).ToList();

                        if (item.Site == "1")
                        {
                            if (pmaster != null)//存在吉甲地站臺價格
                            {
                                //更新吉甲地站臺價格
                                if (!string.IsNullOrEmpty(item.Price)) { pmaster.price = int.Parse(item.Price); prod.Product_Price_List = uint.Parse(item.Price); }
                                if (!string.IsNullOrEmpty(item.Cost)) pmaster.cost = int.Parse(item.Cost);
                                if (!string.IsNullOrEmpty(item.Product_name)) pmaster.product_name = item.Product_name;
                                pmList.Add(pmaster);
                            }
                            else
                            {
                                pM.site_id = 1;
                                pmList.Add(pM);
                                prod.Product_Price_List = uint.Parse(pM.price.ToString());
                            }

                            //將原商品其他站臺價格複製
                            if (oldPriceList.Count > 0)
                            {
                                oldPriceList.ForEach(rec => pmList.Add(rec));
                            }
                        }
                        else//需匯入的商品價格不為吉甲地站臺價格
                        {
                            if (pmaster != null)//存在吉甲地站臺價格，則複製，否，則新建
                            {
                                pmList.Add(pmaster);
                                prod.Product_Price_List = uint.Parse(pmaster.price.ToString());
                            }
                            else
                            {
                                pM.site_id = 1;
                                pmList.Add(pM);
                                prod.Product_Price_List = uint.Parse(pM.price.ToString());
                            }

                            if (oldPriceList.Count > 0)
                            {
                                //處理當前站臺價格
                                PriceMaster nowPm = oldPriceList.Where(rec => rec.site_id == uint.Parse(item.Site)).FirstOrDefault();
                                if (nowPm != null)
                                {
                                    if (!string.IsNullOrEmpty(item.Price)) { nowPm.price = int.Parse(item.Price); prod.Product_Price_List = uint.Parse(item.Price); }
                                    if (!string.IsNullOrEmpty(item.Cost)) nowPm.cost = int.Parse(item.Cost);
                                    if (!string.IsNullOrEmpty(item.Product_name)) nowPm.product_name = item.Product_name;
                                    pmList.Add(nowPm);
                                }
                                else
                                {
                                    pM.site_id = uint.Parse(item.Site);
                                    pmList.Add(pM);
                                }
                                //處理其他站臺價格
                                List<PriceMaster> otherPriceList = oldPriceList.Where(rec => rec.site_id != uint.Parse(item.Site) && rec.site_id != 1).ToList();
                                if (otherPriceList.Count > 0) otherPriceList.ForEach(rec => pmList.Add(rec));

                            }
                            else//新建當前站臺的價格（按吉甲地站臺價格記錄取值）
                            {
                                pM.site_id = uint.Parse(item.Site);
                                pmList.Add(pM);
                            }
                        }
                        #endregion
                    }


                    #region item_price
                    List<List<ItemPrice>> ipListL = new List<List<ItemPrice>>();
                    List<ItemPrice> ipList = new List<ItemPrice>();
                    if (piList.Count > 0)
                    {
                        foreach (ProductItem proItem in piList)
                        {
                            ItemPrice iP = new ItemPrice();
                            iP.item_id = proItem.Item_Id;
                            iP.item_money = proItem.Item_Money;
                            iP.item_cost = proItem.Item_Cost;
                            ipList.Add(iP);
                        }
                        pmList.ForEach(rec => ipListL.Add(ipList));
                    }

                    #endregion

                    #region product_spec
                    List<ProductSpec> proSpecs = _proSpecMgr.Query(new ProductSpec { product_id = uint.Parse(item.Product_id) });
                    ArrayList specs = new ArrayList();
                    if (proSpecs != null)
                    {
                        StringBuilder str;
                        foreach (var specItem in proSpecs)
                        {
                            str = new StringBuilder();
                            str.Append(_proSpecMgr.SaveFromSpec(new ProductSpec { product_id = specItem.product_id, spec_id = specItem.spec_id }));
                            str.Append(_prodItemMgr.UpdateCopySpecId(new ProductItem { Spec_Id_1 = specItem.spec_id, Spec_Id_2 = specItem.spec_id }));
                            specs.Add(str.ToString());
                        }
                    }
                    #endregion

                    #region product_category_set
                    if (!string.IsNullOrEmpty(item.Display))
                    {
                        string[] cateArray = item.Display.Split(',');
                        foreach (string strcate in cateArray)
                        {
                            categorySet.Brand_Id = uint.Parse(item.Brand_name);
                            categorySet.Category_Id = uint.Parse(strcate);
                            insertList.Add(_productCategorySetMgr.SaveNoPrid(categorySet));
                        }
                    }
                    else
                    {
                        insertList.Add(_productCategorySetMgr.SaveFromOtherPro(new ProductCategorySet { Product_Id = uint.Parse(item.Product_id) }));
                    }

                    #endregion

                    #region product_migration_map
                    pMap.temp_id = item.Product_id;
                    insertList.Add(_pMap.SaveNoPrid(pMap));
                    #endregion

                    //tag notice picture
                    insertList.Add(_productNoticeSetMgr.SaveFromOtherPro(new ProductNoticeSet { product_id = uint.Parse(item.Product_id) }));
                    insertList.Add(_productTagSetMgr.SaveFromOtherPro(new ProductTagSet { product_id = uint.Parse(item.Product_id) }));
                    insertList.Add(_productPictureMgr.SaveFromOtherPro(new ProductPicture { product_id = int.Parse(item.Product_id) }));
                    insertList.Add(_proStatusHistoryMgr.SaveNoProductId(new ProductStatusHistory { product_status = int.Parse(item.Status), type = 7, user_id = 0 }));     //商品歷史記錄
                    result = _prodMgr.ProductMigration(prod, pmList, piList, ipListL, insertList, specs);

                    #region 原update邏輯代碼
                    /*ArrayList saveArray = new ArrayList();
                    saveArray.Add(_prodMgr.Update(prod));

                    PriceMaster queryMaster = _priceMgr.QueryByUserId(new PriceMaster { product_id = uint.Parse(item.Product_id) }).FirstOrDefault();
                    if (queryMaster == null)            //新增
                    {
                        //price_master
                        pM.product_id = uint.Parse(item.Product_id);
                        pM.bonus_percent = 1;
                        pM.default_bonus_percent = 1;
                        pM.price_status = 1;
                        pM.cost = int.Parse(item.Cost);
                        pM.price = int.Parse(item.Price);
                        pM.user_level = 1;
                        pM.site_id = uint.Parse(item.Site);
                        pM.user_id = 0;
                        pM.product_name = item.Product_name;
                        pM.same_price = 1;
                        pM.child_id = 0;

                        //item_price
                        List<ItemPrice> ipList = new List<ItemPrice>();
                        List<ProductItem> queryList = _prodItemMgr.Query(new ProductItem { Product_Id = uint.Parse(item.Product_id) });
                        foreach (ProductItem proItem in queryList)
                        {
                            iPrice.item_id = proItem.Item_Id;
                            iPrice.item_money = uint.Parse(item.Price);
                            iPrice.item_cost = uint.Parse(item.Cost);
                            ipList.Add(iPrice);
                        }

                        //保存
                        string strError = "";
                        if (_priceMgr.Save(pM, ipList, saveArray, ref strError) <= 0)
                        {
                            result = false;
                        }
                    }
                    else      //更新
                    {
                        //price_master
                        queryMaster.cost = int.Parse(item.Cost);
                        queryMaster.price = int.Parse(item.Price);
                        queryMaster.user_level = 1;
                        queryMaster.site_id = uint.Parse(item.Site);
                        queryMaster.user_id = 0;
                        queryMaster.same_price = 1;
                        queryMaster.child_id = 0;
                        saveArray.Add(_priceMgr.Update(queryMaster));

                        //item_price
                        List<ItemPrice> itemPriceList = _itemPriceMgr.itemPriceQuery(new ItemPrice { price_master_id = queryMaster.price_master_id });
                        if (itemPriceList != null && itemPriceList.Count() > 0)     //存在item_price则更新，否则新增item_price
                        {
                            foreach (ItemPrice priceItem in itemPriceList)
                            {
                                priceItem.item_cost = uint.Parse(item.Cost);
                                priceItem.item_money = uint.Parse(item.Price);
                                saveArray.Add(_itemPriceMgr.Update(priceItem));
                            }
                        }
                        else
                        {
                            List<ProductItem> proItemList = _prodItemMgr.Query(new ProductItem { Product_Id = uint.Parse(item.Product_id) });
                            foreach (ProductItem proitemItem in proItemList)
                            {
                                iPrice.price_master_id = queryMaster.price_master_id;
                                iPrice.item_id = proitemItem.Item_Id;
                                iPrice.item_money = uint.Parse(item.Price);
                                iPrice.item_cost = uint.Parse(item.Cost);
                                saveArray.Add(_itemPriceMgr.Save(iPrice));
                            }
                        }

                        BLL.gigade.Dao.MySqlDao _mySqlDao = new BLL.gigade.Dao.MySqlDao(connectionString);
                        result = _mySqlDao.ExcuteSqls(saveArray);
                    }*/

                    #endregion

                }

            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }
    }

    public class BeginInsert : DataOperation
    {
        public BeginInsert(List<Model.MigrationDataSet> mds, MainForm form) : base(mds, form) { }

        public override bool Exec()
        {
            bool result = false;
            try
            {
                foreach (MigrationDataSet item in mds)
                {
                    form.change(1);
                    ArrayList insertList = new ArrayList();
                    List<ProductItem> piList = new List<ProductItem>();
                    List<PriceMaster> pmList = new List<PriceMaster>();
                    List<ItemPrice> ipList = new List<ItemPrice>();
                    List<List<ItemPrice>> ipListL = new List<List<ItemPrice>>();
                    //product.user_id：同此品牌廠商之verdor.product_manage
                    VendorBrand vb = _vendorBrandMgr.GetProductBrand(new VendorBrand { Brand_Id = uint.Parse(item.Brand_name) });
                    Vendor v = _vendorMgr.GetSingle(new Vendor { vendor_id = vb.Vendor_Id });

                    //product
                    prod.Product_Name = item.Product_name;
                    prod.Brand_Id = uint.Parse(item.Brand_name);
                    prod.Combination = 1;
                    prod.Ignore_Stock = 0;      //默認false
                    prod.Cate_Id = item.Cate_id;
                    prod.Product_Price_List = uint.Parse(item.Price);
                    prod.Product_Status = uint.Parse(item.Status);
                    prod.Product_Freight_Set = uint.Parse(item.Freight_set);
                    prod.Product_Mode = uint.Parse(item.Product_mode);
                    prod.Tax_Type = int.Parse(item.Tax);
                    prod.user_id = v.product_manage;
                    prod.Shortage = 0;          //默認false
                    prod.Product_Createdate = uint.Parse(BLL.gigade.Common.CommonFunction.GetPHPTime().ToString());
                    if (!string.IsNullOrEmpty(item.Service_Fee))
                    {
                        uint bag_check_money = 0;
                        uint.TryParse(item.Service_Fee, out bag_check_money);
                        prod.Bag_Check_Money = bag_check_money;
                    }

                    //product_item
                    pItem.Item_Stock = int.Parse(item.Stock);
                    pItem.Spec_Id_1 = 0;
                    pItem.Spec_Id_2 = 0;
                    pItem.Item_Money = uint.Parse(item.Price);
                    pItem.Item_Cost = uint.Parse(item.Cost);
                    piList.Add(pItem);

                    //price_master
                    pM.cost = int.Parse(item.Cost);
                    pM.price = int.Parse(item.Price);
                    pM.user_level = 1;
                    pM.accumulated_bonus = 1;
                    //pM.user_id = v.product_manage;
                    pM.product_name = item.Product_name;
                    pM.same_price = 1;
                    pM.price = int.Parse(item.Price);
                    pM.child_id = 0;
                    pM.bonus_percent = 1;
                    pM.default_bonus_percent = 1;
                    pM.price_status = 1;
                    if (item.Site != "1")//不為吉甲地
                    {
                        //先添加非吉甲地的站臺
                        pM.site_id = uint.Parse(item.Site);
                        pmList.Add(pM);
                        PriceMaster pMgiagde = pM.Clone() as PriceMaster;
                        //再默認添加一筆吉甲地的站臺且價格狀態為4(下架)
                        pMgiagde.site_id = 1;
                        pMgiagde.price_status = 4;
                        pmList.Add(pMgiagde);
                    }
                    else
                    {
                        pM.site_id = 1;
                        pmList.Add(pM);
                    }

                    //item_price
                    iPrice.item_money = uint.Parse(item.Price);
                    iPrice.item_cost = uint.Parse(item.Cost);
                    ipList.Add(iPrice);
                    pmList.ForEach(rec => ipListL.Add(ipList));


                    #region product_category_set
                    if (!string.IsNullOrEmpty(item.Display))
                    {
                        string[] cateArray = item.Display.Split(',');
                        foreach (string strcate in cateArray)
                        {
                            categorySet.Brand_Id = uint.Parse(item.Brand_name);
                            categorySet.Category_Id = uint.Parse(strcate);
                            insertList.Add(_productCategorySetMgr.SaveNoPrid(categorySet));
                        }
                    }
                    #endregion


                    //product_migration_map
                    pMap.temp_id = item.Temp_id;
                    insertList.Add(_pMap.SaveNoPrid(pMap));

                    //商品歷史記錄
                    insertList.Add(_proStatusHistoryMgr.SaveNoProductId(new ProductStatusHistory { product_status = int.Parse(item.Status), type = 7, user_id = 0 }));

                    result = _prodMgr.ProductMigration(prod, pmList, piList, ipListL, insertList, null);


                }
            }
            catch (Exception)
            {

            }
            return result;
        }
    }

    public class PriceInsert : DataOperation
    {
        public PriceInsert(List<Model.MigrationDataSet> mds, MainForm form) : base(mds, form) { }

        public override bool Exec()
        {
            bool result = true;
            PriceMaster priceMaster = new PriceMaster();
            foreach (MigrationDataSet item in mds)
            {
                result = true;
                form.change(1);
                //如果价格档原料编号新旧为new,则从对照表取出formula对应的product_id.
                //if (item.New_old.Equals("new"))
                //{
                ProductMigrationMap mapQuery = _pMap.GetSingle(new ProductMigrationMap { temp_id = item.Formula });
                if (mapQuery != null)
                {
                    item.Formula = mapQuery.product_id.ToString();
                }
                else
                {
                    item.OutMessage = "formula對照不存在";
                    result = false;
                    continue;
                }

                //}

                #region 原逻辑
                //如果价格档中无价格,则取formula对应PriceMaster中站台为gigade的价格.
                //if (string.IsNullOrEmpty(item.Price) || string.IsNullOrEmpty(item.Cost))
                //{

                //    PriceMaster priceQuery = _priceMgr.PriceMasterQuery(new PriceMaster { product_id = uint.Parse(item.Formula), site_id = 1 }).FirstOrDefault();
                //    if (priceQuery != null)
                //    {
                //        item.Cost = priceQuery.cost.ToString();
                //        item.Price = priceQuery.price.ToString();
                //    }
                //    else
                //    {
                //        item.OutMessage = "price,cost不存在";
                //        result = false;
                //        continue;
                //    }
                //} 
                #endregion

                //获取product_id对应product_item中的价格
                if (!item.Product_id.Equals("0"))
                {
                    ProductItem itemQuery = _prodItemMgr.Query(new ProductItem { Product_Id = uint.Parse(item.Product_id) }).FirstOrDefault();
                    if (itemQuery != null)
                    {
                        item.Cost = itemQuery.Item_Cost.ToString();
                        item.Price = itemQuery.Item_Money.ToString();
                    }
                    else
                    {
                        item.OutMessage = "product_item中无product_id信息;";
                        result = false;
                        continue;
                    }
                }
                else
                {
                    item.OutMessage = "product_id不存在;";
                    result = false;
                    continue;
                }



                //price_master
                priceMaster.product_id = uint.Parse(item.Formula);
                priceMaster.cost = int.Parse(item.Cost);
                priceMaster.price = int.Parse(item.Price);
                priceMaster.price_status = 1;
                priceMaster.user_level = 1;
                priceMaster.default_bonus_percent = 1;
                priceMaster.bonus_percent = 1;
                priceMaster.site_id = uint.Parse(item.Site);
                priceMaster.user_id = 0;
                priceMaster.product_name = item.Product_name;
                priceMaster.same_price = 1;
                priceMaster.child_id = 0;

                //item_price
                List<ItemPrice> ipList = new List<ItemPrice>();
                List<ProductItem> queryList = _prodItemMgr.Query(new ProductItem { Product_Id = uint.Parse(item.Formula) });
                foreach (ProductItem proItem in queryList)
                {
                    iPrice = new ItemPrice();
                    iPrice.item_id = proItem.Item_Id;
                    iPrice.item_money = uint.Parse(item.Price);
                    iPrice.item_cost = uint.Parse(item.Cost);
                    ipList.Add(iPrice);
                }
                ArrayList others = new ArrayList();
                Site site = _siteMgr.Query(new Site { Site_Id = uint.Parse(item.Site) }).FirstOrDefault();
                if (site.Site_Name.Trim().ToLower() == "chinatrust")
                {
                    Product pro = _prodMgr.Query(new Product { Product_Id = mapQuery.product_id }).FirstOrDefault();
                    if (pro == null)
                    {
                        item.OutMessage = "商品不存在";
                        continue;
                    }
                    #region 更新該站臺商品的 product_category_set

                    string[] cateArray = item.Display.Split(',');
                    foreach (string strcate in cateArray)
                    {
                        ProductCategorySet category = new ProductCategorySet { Brand_Id = pro.Brand_Id, Category_Id = uint.Parse(strcate), Product_Id = mapQuery.product_id };
                        if (_productCategorySetMgr.Query(category).FirstOrDefault() == null)
                        {
                            others.Add(_productCategorySetMgr.Save(category));
                        }
                        else
                        {
                            item.OutMessage = "display:" + strcate + " 已存在;";
                            result = false;
                            break;
                        }
                    }
                    if (!result) continue;
                }
                    #endregion

                //保存
                string strError = "";
                if (_priceMgr.Save(priceMaster, ipList, others, ref strError) <= 0)
                {
                    result = false;
                    item.OutMessage = strError;
                    if (strError == "")
                    {
                        item.OutMessage = "匯入失敗";
                    }
                }
                else
                {
                    item.OutMessage = "匯入成功";
                }

            }
            return result;
        }

    }

}
