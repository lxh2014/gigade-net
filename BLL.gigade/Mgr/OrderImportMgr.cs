/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：OrderImportMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/22 14:11:22 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using System.Collections;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;
using BLL.gigade.Mgr.Impl;
using System.Web;
using BLL.gigade.Dao;
namespace BLL.gigade.Mgr
{
    public class OrderImportMgr
    {
        private IDBAccess _dbAccess;
        //private FileManagement fileManagement;
        private NPOI4ExcelHelper excelHelper;
        private IProductItemMapImplMgr productItemMapMgr;
        private IProductImplMgr productMgr;
        private IProductItemImplMgr productItemMgr;
        private IVendorBrandImplMgr vendorBrandMgr;
        private IDeliveryFreightSetMappingImplMgr deliveryFreightSetMappingMgr;
        private IChannelOrderImplMgr channelOrderMgr;
        private IProductSpecImplMgr specMgr;
        private IOrderMasterImplMgr orderMasterMgr;
        private IOrderMasterPatternImplMgr _orderMasterPatternMgr;
        private IOrderSlaveImplMgr orderSlaveMgr;
        private IOrderDetailImplMgr orderDetailMgr;
        private IPriceMasterImplMgr priceMasterMgr;
        private IItemPriceImplMgr itemPriceMgr;
        private IProductMapSetImplMgr productMapSetMgr;
        private IOrderPaymentHncbImplMgr _orderPaymentHncbMgr;
        private IBonusMasterImplMgr _bonusMasterMgr;
        private IBonusRecordImplMgr _bonusRecordMgr;

        private string MySqlConnStr;
        private int ChannelID;

        public string SlaveNote { get; set; }
        public OrderImportMgr(string mySqlConnStr, int channelId)
        {
            this.MySqlConnStr = mySqlConnStr;
            this.ChannelID = channelId;

            SlaveNote = string.Empty;
        }

        #region 讀取整個excel文件數據
        /// <summary>
        /// 讀取整個excel文件數據
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public List<T> ReadExcel<T>(string filePath)
        {
            List<T> orders = new List<T>();
            try
            {
                excelHelper = new NPOI4ExcelHelper(filePath);
                _dbAccess = DBFactory.getDBAccess(DBType.MySql, MySqlConnStr);

                string extension = System.IO.Path.GetExtension(filePath).ToLower().ToString();
                System.Data.DataTable _dt = excelHelper.SheetData();
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    orders = _dbAccess.getObjByTable<T>(_dt);
                    orders = orders.Skip(1).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("OrderImportMgr-->ReadExcel<T>-->" + ex.Message, ex);
            }
            return orders;
        }
        #endregion

        #region 讀取整個excel文件數據  判斷上傳的 模板 是否 符合條件
        /// <summary>
        /// 讀取整個excel文件數據  add by zhuoqin0830w 2015/06/09
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="template"></param>
        /// <param name="model_in"></param>
        /// <returns></returns>
        public List<T> ReadExcelMatch<T>(string filePath, string template, string model_in)
        {
            List<T> orders = new List<T>();
            try
            {
                string extension = System.IO.Path.GetExtension(filePath).ToLower().ToString();
                _dbAccess = DBFactory.getDBAccess(DBType.MySql, MySqlConnStr);
                //上傳的Excel
                excelHelper = new NPOI4ExcelHelper(filePath);
                System.Data.DataTable _dt = excelHelper.SheetData();
                //模板Excel
                excelHelper = new NPOI4ExcelHelper(template);
                System.Data.DataTable _dt1 = null;
                ArrayList num = excelHelper.SheetNameForExcel();
                bool check = true;
                // 根據 model_in 判斷 是 GIGADE 訂單 或 PayEasy 訂單 或 其他訂單
                switch (model_in)
                {
                    case "1": // 其他 模板
                        for (int i = 0; i < num.Count; i++)
                        {
                            //查找 不是 GIGADE 和 PayEasy 模板的 Dt 數據 
                            if (num[i].ToString() == "yahoo_home" || num[i].ToString() == "yahoo_store" || num[i].ToString() == "udn")
                            {
                                _dt1 = excelHelper.SheetData(i);
                                // 判斷 上傳的 Excel 和 模板 Excel 的 第一個欄位 是否 符合 
                                if (_dt.Rows[0][0].ToString() == _dt1.Rows[0][0].ToString())
                                {
                                    for (int j = 0; j < _dt.Columns.Count; j++)
                                    {
                                        //判斷 符合的 數據中  其他欄位是否 相同
                                        if (_dt.Rows[0][j].ToString() != _dt1.Rows[0][j].ToString())
                                        { check = false; break; }
                                    }
                                    // 如果 相同 則跳出循環 
                                    if (check) { break; }
                                }
                                else { check = false; }
                            }
                        }
                        break;
                    case "2": // GIGADE 模板
                        for (int i = 0; i < num.Count; i++)
                        {
                            if (num[i].ToString() == "gigade")
                            {
                                _dt1 = excelHelper.SheetData(i);
                            }
                        }
                        break;
                    case "3": // PayEasy 模板
                        for (int i = 0; i < num.Count; i++)
                        {
                            if (num[i].ToString() == "payeasy")
                            {
                                _dt1 = excelHelper.SheetData(i);
                            }
                        }
                        break;
                }
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    // 排除 其他 的模板
                    if (model_in != "1")
                    {
                        // 判斷 GIGADE 和 PayEasy 模板 是否 為空 
                        if (_dt1 != null)
                        {
                            for (int i = 0; i < _dt.Columns.Count; i++)
                            {
                                if (_dt.Rows[0][i].ToString() != _dt1.Rows[0][i].ToString())
                                {
                                    check = false;
                                    break;
                                }
                            }
                        }
                        else { check = false; }
                    }

                    //如果 Check 為 true 表示 上傳的 Excel 與 模板Excel 匹配成功
                    if (check)
                    {
                        orders = _dbAccess.getObjByTable<T>(_dt);
                        orders = orders.Skip(1).ToList();
                    }
                    else { orders = null; }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("OrderImportMgr-->ReadExcel<T>-->" + ex.Message, ex);
            }
            return orders;
        }
        #endregion

        #region 選中數據得到OrdersImport
        /// <summary>
        /// 選中數據得到OrdersImport
        /// </summary>
        /// <param name="chkRecord"></param>
        /// <returns></returns>
        public List<OrdersImport> SplitChkData(string chkRecord)
        {
            List<OrdersImport> chks = null;
            try
            {
                if (!string.IsNullOrEmpty(chkRecord) && chkRecord.LastIndexOf("|") != -1)
                {
                    string[] orders = chkRecord.Split('|');
                    chks = new List<OrdersImport>();
                    foreach (string str in orders)
                    {
                        if (!string.IsNullOrEmpty(str) && str.LastIndexOf("+") != -1)
                        {
                            chks.Add(new OrdersImport { dmtshxuid = str.Split('+')[0], chlitpdno = str.Split('+')[1] });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("OrderImportMgr-->SplitChkData-->" + ex.Message, ex);
            }
            return chks;
        }
        #endregion

        #region 生成單筆訂單的OrderSlave,OrderDetail  合作賣場
        /// <summary>
        /// 生成單筆訂單的OrderSlave,OrderDetail
        /// </summary>
        /// <param name="product"></param>
        /// <param name="all"></param>
        /// <param name="stockEnough"></param>
        /// <returns></returns>
        public List<OrderSlave> GetSlave(List<OrdersImport> product, List<OrdersImport> all)
        {
            List<OrderSlave> slaves = new List<OrderSlave>();
            try
            {
                product = product.Where(m => !m.dsr.Contains("物流服務費")).ToList();//去掉運費記錄
                ProductItemMap pItemQuery = new ProductItemMap { channel_id = Convert.ToUInt32(ChannelID) };
                bool result = true;
                uint pack_id = 0;

                Dictionary<int, int> buyTotal = new Dictionary<int, int>();
                List<ProductItemMap> allMap = new List<ProductItemMap>();
                List<Product> allPro = new List<Product>();
                List<ProductItem> allItem = new List<ProductItem>();
                List<ProductMapSet> allMapSet = new List<ProductMapSet>();

                productItemMapMgr = new ProductItemMapMgr(MySqlConnStr);
                productMgr = new ProductMgr(MySqlConnStr);
                productItemMgr = new ProductItemMgr(MySqlConnStr);
                priceMasterMgr = new PriceMasterMgr(MySqlConnStr);
                itemPriceMgr = new ItemPriceMgr(MySqlConnStr);

                #region 驗證數據，準備數據
                foreach (OrdersImport p in product)
                {
                    if (string.IsNullOrEmpty(p.qty.Trim()) || p.qty.Trim() == "0") { continue; };
                    if (string.IsNullOrEmpty(p.sumup.Trim())) { p.sumup = "0"; };

                    #region ProductItemMap

                    pItemQuery.channel_detail_id = p.chlitpdno;
                    ProductItemMap itemMap = productItemMapMgr.QueryAll(pItemQuery).FirstOrDefault();//查詢賣場商品映射商品信息
                    if (itemMap == null)
                    {
                        all.FindAll(m => m.dmtshxuid == p.dmtshxuid && m.chlitpdno == p.chlitpdno).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("PRODUCT_MAP_NOT_EXISTS"));
                        result = false;
                        continue;
                    }
                    if (!allMap.Contains(itemMap))
                    {
                        allMap.Add(itemMap);
                    }
                    #endregion

                    #region Product

                    Product pr;
                    ProductItem tmpItem = null;
                    if (itemMap.item_id == 0)
                    {
                        pr = productMgr.Query(new Product { Product_Id = itemMap.product_id }).FirstOrDefault();
                    }
                    else
                    {
                        //單一商品，此items即購買的商品
                        #region ProductItem

                        tmpItem = productItemMgr.Query(new ProductItem { Item_Id = Convert.ToUInt32(itemMap.item_id) }).FirstOrDefault();
                        if (tmpItem == null)
                        {
                            all.FindAll(m => m.dmtshxuid == p.dmtshxuid && m.chlitpdno == p.chlitpdno).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("PRODUCT_NOT_EXISTS"));
                            result = false;
                            continue;
                        }
                        if (!allItem.Contains(tmpItem))
                        {
                            allItem.Add(tmpItem);
                        }
                        #endregion

                        #region 計算購買數量

                        int item_id = Convert.ToInt32(tmpItem.Item_Id);
                        int buy = Convert.ToInt32(p.qty);
                        if (buyTotal.ContainsKey(item_id))
                        {
                            buy += buyTotal[item_id];
                            buyTotal.Remove(item_id);
                        }
                        buyTotal.Add(item_id, buy);
                        #endregion

                        pr = productMgr.Query(new Product { Product_Id = tmpItem.Product_Id }).FirstOrDefault();
                    }
                    if (pr == null)
                    {
                        all.FindAll(m => m.dmtshxuid == p.dmtshxuid && m.chlitpdno == p.chlitpdno).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("PRODUCT_NOT_EXISTS"));
                        result = false;
                        continue;
                    }
                    //補貨中停止販售 1:是 0:否
                    if (pr.Shortage == 1)
                    {
                        all.FindAll(m => m.dmtshxuid == p.dmtshxuid && m.chlitpdno == p.chlitpdno).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("SHORT_AGE"));
                        result = false;
                        break;
                    }
                    if (!allPro.Contains(pr))
                    {
                        allPro.Add(pr);
                    }
                    #endregion

                    #region 組合商品，取出組成商品

                    if (pr.Combination != 0 && pr.Combination != 1)
                    {
                        #region ProductMapSet

                        productMapSetMgr = new ProductMapSetMgr(MySqlConnStr);
                        List<ProductMapSet> proMapSets = productMapSetMgr.Query(new ProductMapSet { map_rid = itemMap.rid });
                        if (proMapSets == null || proMapSets.Count == 0)
                        {
                            all.FindAll(m => m.dmtshxuid == p.dmtshxuid && m.chlitpdno == p.chlitpdno).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("PRODUCT_MAP_NOT_EXISTS"));
                            result = false;
                            continue;
                        }
                        #endregion

                        #region ProductItem

                        foreach (var set in proMapSets)
                        {
                            #region 計算購買數量

                            int item_id = Convert.ToInt32(set.item_id);
                            int buy = Convert.ToInt32(set.set_num * Convert.ToInt32(p.qty));
                            if (buyTotal.ContainsKey(item_id))
                            {
                                buy += buyTotal[item_id];
                                buyTotal.Remove(item_id);
                            }
                            buyTotal.Add(item_id, buy);
                            #endregion
                            //ProductItem tmpItem = productItemMgr.Query(new ProductItem { Item_Id = set.item_id }).FirstOrDefault();
                            tmpItem = productItemMgr.Query(new ProductItem { Item_Id = set.item_id }).FirstOrDefault();
                            if (tmpItem == null)
                            {
                                all.FindAll(m => m.dmtshxuid == p.dmtshxuid && m.chlitpdno == p.chlitpdno).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("PRODUCT_NOT_EXISTS"));
                                result = false;
                                break;
                            }
                            pr = productMgr.Query(new Product { Product_Id = tmpItem.Product_Id }).FirstOrDefault();
                            if (pr == null)
                            {
                                all.FindAll(m => m.dmtshxuid == p.dmtshxuid && m.chlitpdno == p.chlitpdno).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("PRODUCT_NOT_EXISTS"));
                                result = false;
                                break;
                            }
                            //補貨中停止販售 1:是 0:否
                            if (pr.Shortage == 1)
                            {
                                all.FindAll(m => m.dmtshxuid == p.dmtshxuid && m.chlitpdno == p.chlitpdno).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("SHORT_AGE"));
                                result = false;
                                break;
                            }
                            if (!allMapSet.Contains(set))
                            {
                                allMapSet.Add(set);
                            }
                            if (!allItem.Contains(tmpItem))
                            {
                                allItem.Add(tmpItem);
                            }
                            if (!allPro.Contains(pr))
                            {
                                allPro.Add(pr);
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion

                #region 庫存判斷

                foreach (var item in buyTotal)
                {
                    //庫存為0時是否可以販賣
                    if (allPro.Where(m => m.Product_Id == allItem.Where(n => n.Item_Id == item.Key).FirstOrDefault().Product_Id).FirstOrDefault().Ignore_Stock == 0)
                    {
                        if (allItem.Where(m => m.Item_Id == item.Key).FirstOrDefault().Item_Stock < item.Value)//item_stock去和需要夠買的數量進行對比
                        {
                            List<ProductItemMap> pTm = productItemMapMgr.QueryChannel_detail_id(allItem.Where(e => e.Item_Id == item.Key).FirstOrDefault().Item_Id.ToString());
                            foreach (var pitemmap in pTm)
                            {
                                all.FindAll(m => product.FirstOrDefault().dmtshxuid == m.dmtshxuid && m.chlitpdno == pitemmap.channel_detail_id && string.IsNullOrEmpty(m.Msg)).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("EMPTY_STOCK"));
                            }
                            all.FindAll(m => product.FirstOrDefault().dmtshxuid == m.dmtshxuid && string.IsNullOrEmpty(m.Msg)).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("PRODUCT_WARNING"));//"存在庫存不足的商品"
                            result = false;
                            break;
                        }
                    }
                }
                if (!result)
                {
                    return null;
                }
                #endregion

                #region 計算價格 生成slave

                foreach (OrdersImport p in product)
                {
                    if (string.IsNullOrEmpty(p.qty.Trim()) || p.qty.Trim() == "0") { continue; };
                    if (string.IsNullOrEmpty(p.sumup.Trim())) { p.sumup = "0"; };

                    ProductItemMap itemMap = allMap.Where(m => m.channel_detail_id.ToLower().Trim() == p.chlitpdno.ToLower().Trim()).FirstOrDefault();//查詢賣場商品映射商品信息
                    Product pr;
                    List<ProductItem> items = new List<ProductItem>();
                    PriceMaster price;

                    #region 取出商品

                    if (itemMap.item_id == 0)
                    {
                        pr = allPro.Where(m => m.Product_Id == itemMap.product_id).FirstOrDefault();
                    }
                    else
                    {
                        //單一商品，此items即購買的商品
                        items = allItem.Where(m => m.Item_Id == Convert.ToUInt32(itemMap.item_id)).ToList();
                        pr = allPro.Where(m => m.Product_Id == items.FirstOrDefault().Product_Id).FirstOrDefault();
                    }
                    #endregion

                    if (pr.Combination != 0 && pr.Combination != 1)
                    {
                        #region 商品價格信息
                        //edit by hufeng0813w 2014-07-01 外站的成本可以設定,不再默認取吉甲地站臺的
                        //原先邏輯
                        //price = priceMasterMgr.Query(new PriceMaster { product_id = pr.Product_Id, site_id = 1, user_level = 1, user_id = 0, child_id = Convert.ToInt32(pr.Product_Id) }).FirstOrDefault();
                        price = priceMasterMgr.Query(new PriceMaster { product_id = pr.Product_Id, price_master_id = itemMap.price_master_id }).FirstOrDefault();
                        if (price == null)
                        {
                            all.FindAll(m => m.dmtshxuid == p.dmtshxuid && m.chlitpdno == p.chlitpdno).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("PRODUCT_PRICE_NOT_EXISTS"));
                            result = false;
                            continue;
                        }
                        #endregion

                        List<Product> pros = new List<Product>();
                        List<PriceMaster> priceMasters = new List<PriceMaster>();

                        #region 從對照取出每項子商品，及價格

                        List<ProductMapSet> proMapSets = allMapSet.Where(m => m.map_rid == itemMap.rid).ToList();
                        foreach (var set in proMapSets)
                        {
                            //ProductItem
                            ProductItem tmpItem = allItem.Where(m => m.Item_Id == Convert.ToUInt32(set.item_id)).FirstOrDefault();
                            items.Add(tmpItem);
                            //Product
                            pros.Add(allPro.Where(m => m.Product_Id == tmpItem.Product_Id).FirstOrDefault());

                            #region 子商品PriceMaster

                            PriceMaster query = new PriceMaster { site_id = 1, user_level = 1, user_id = 0 };
                            //edit by xiangwang0413w 2014-07-08 外站的成本可以設定,不再默認取吉甲地站臺的
                            //PriceMaster query = new PriceMaster() { price_master_id = itemMap.price_master_id };
                            if (pr.Price_type == 1)
                            {
                                query.product_id = tmpItem.Product_Id;
                            }
                            else if (pr.Price_type == 2)
                            {
                                //各自定價時 查詢子商品價格為child_id=子商品ID的價格檔
                                query.product_id = pr.Product_Id;
                                query.child_id = Convert.ToInt32(tmpItem.Product_Id);
                            }
                            PriceMaster tmpPrice = priceMasterMgr.Query(query).FirstOrDefault();
                            if (tmpPrice == null)
                            {
                                all.FindAll(m => m.dmtshxuid == p.dmtshxuid && m.chlitpdno == p.chlitpdno).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("PRODUCT_PRICE_NOT_EXISTS"));
                                result = false;
                                break;
                            }
                            if (tmpPrice.same_price != 1)
                            {
                                ItemPrice tmp = itemPriceMgr.Query(new ItemPrice { price_master_id = tmpPrice.price_master_id, item_id = set.item_id, IsPage = false }).FirstOrDefault();
                                tmpPrice.price = Convert.ToInt32(tmp.item_money);
                                tmpPrice.cost = Convert.ToInt32(tmp.item_cost);
                                tmpPrice.event_cost = Convert.ToInt32(tmp.event_cost);
                            }
                            priceMasters.Add(tmpPrice);
                            #endregion
                        }
                        if (!result) continue;
                        #endregion

                        #region 計算單個商品價格

                        pack_id++;
                        long sumPrice = priceMasters.Sum(m => proMapSets[priceMasters.IndexOf(m)].set_num * m.price);//組合商品內商品售價總和

                        long MainCost = price.cost;//組合商品本身的成本
                        long sumCost = priceMasters.Sum(m => proMapSets[priceMasters.IndexOf(m)].set_num * m.cost);//組合商品內商品成本總和
                        long sumEventCost = priceMasters.Sum(m => proMapSets[priceMasters.IndexOf(m)].set_num * m.event_cost);//組合商品內商品活動成本總和

                        int sumUp = Convert.ToInt32(p.sumup) / Convert.ToInt32(p.qty);//算一個組合的售價
                        int childAmout = Convert.ToInt32(sumPrice);
                        pros.ForEach(m => m.Bag_Check_Money = 0);//寄倉費，若是組合，寄倉費只看父項商品的設定，不管子項商品上有無設定寄倉費 edit by xiangwang0413w 2014/08/20
                        for (int i = 0; i < items.Count; i++)
                        {
                            #region 計算單個商品售價

                            if (proMapSets[i].set_num == 0)
                            {
                                pros[i].Product_Price_List = 0;
                            }
                            else
                            {
                                if (pr.Price_type == 1)
                                {
                                    #region 售價按比例拆分
                                    //計算單品售價
                                    //判斷一個組合商品的售價是否為 0 如果 不為 0 則 進行 計算
                                    pros[i].Product_Price_List = sumUp == 0 ? 0 : Convert.ToUInt32(Math.Round(sumUp * ((Convert.ToDouble(priceMasters[i].price) * proMapSets[i].set_num) / childAmout)));//售價乘以購買數量
                                    sumUp -= Convert.ToInt32(pros[i].Product_Price_List);// * proMapSets[i].set_num);
                                    childAmout -= Convert.ToInt32(priceMasters[i].price * proMapSets[i].set_num);
                                    //計算單品成本 add by hufeng0813w 2014/06/05
                                    long Mcost = (priceMasters[i].cost * proMapSets[i].set_num);//組合商品下子商品的總成本
                                    priceMasters[i].cost = Convert.ToInt32(Math.Round(MainCost * (sumCost == 0 ? 0 : ((Convert.ToDouble(priceMasters[i].cost) * proMapSets[i].set_num) / sumCost))));//組合商品的成本*子商品所占之比例; 子商品所占之比例=((子商品成本*子商品在組合中的數量)/所有子商品的成本總和)
                                    MainCost -= Convert.ToInt32(priceMasters[i].cost);//組合本身售價-算過的子商品的總成本
                                    sumCost -= Mcost;//組合商品總成本-算過的子商品的總成本
                                    #endregion
                                }
                                else if (pr.Price_type == 2)
                                {
                                    #region 各自定價

                                    pros[i].Product_Price_List = Convert.ToUInt32(priceMasters[i].price);
                                    #endregion
                                }
                            }
                            #endregion
                            //buyNum 乘以了組合商品的購買數量 edit by hufeng0813w 2014/06/06 一個組合的訂單中的子商品buy_num字段只寫自身在組合中的組成數量不需要乘以組合的購買數量
                            uint buyNum = proMapSets[i].set_num;// uint.Parse(p.qty) *
                            priceMasters[i].price = int.Parse((priceMasters[i].price * buyNum).ToString());//組合中子商品的signe_price乘以本身在組合中的所組成數量 add by hufeng0813w 2014/06/06
                            FillSlave(slaves, pros[i], items[i], priceMasters[i], pros[i].Product_Price_List * buyNum, buyNum, priceMasters[i].product_name, p.chlitpdno, p.subsn, pack_id, pr, uint.Parse(p.qty));
                        }
                        #endregion

                        #region 組合商品自身添加至detail

                        //取得組合商品的成本和活動成本
                        PriceMaster pMaster = new PriceMaster { cost = price.cost, event_cost = price.event_cost, price_master_id = price.price_master_id };//{ cost = Convert.ToInt32(sumCost), event_cost = Convert.ToInt32(sumEventCost), price_master_id = price.price_master_id };
                        if (pr.Price_type == 1)
                        {
                            pMaster.price = price.price;
                        }
                        else if (pr.Price_type == 2)
                        {
                            if (price.same_price != 0)//不同價時 需計算價格
                            {
                                pMaster.price = Convert.ToInt32(sumPrice);//組合商品內商品售價總和
                            }
                            else
                            {
                                pMaster.price = price.price;
                            }
                        }
                        ProductItem pItem = new ProductItem { Item_Id = items.FirstOrDefault().Item_Id };

                        FillSlave(slaves, pr, pItem, pMaster, uint.Parse(p.sumup), uint.Parse(p.qty), p.dsr, p.chlitpdno, p.subsn, pack_id, pr, uint.Parse(p.qty));
                        #endregion
                    }
                    else
                    {
                        #region 商品價格信息

                        //edit by hufeng0813w 2014-07-01 外站的成本可以設定,不再默認取吉甲地站臺的
                        //原先邏輯
                        //price = priceMasterMgr.Query(new PriceMaster { product_id = pr.Product_Id, site_id = 1, user_level = 1, user_id = 0 }).FirstOrDefault();
                        price = priceMasterMgr.Query(new PriceMaster { product_id = pr.Product_Id, price_master_id = itemMap.price_master_id }).FirstOrDefault();
                        if (price == null)
                        {
                            all.FindAll(m => m.dmtshxuid == p.dmtshxuid && m.chlitpdno == p.chlitpdno).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("PRODUCT_PRICE_NOT_EXISTS"));
                            result = false;
                            continue;
                        }
                        if (price.same_price != 1)
                        {
                            ItemPrice tmp = itemPriceMgr.Query(new ItemPrice { price_master_id = price.price_master_id, item_id = items.FirstOrDefault().Item_Id, IsPage = false }).FirstOrDefault();
                            price.cost = Convert.ToInt32(tmp.item_cost);
                            price.event_cost = Convert.ToInt32(tmp.event_cost);
                            price.price = Convert.ToInt32(tmp.item_money);
                        }
                        #endregion

                        FillSlave(slaves, pr, items.FirstOrDefault(), price, uint.Parse(p.sumup), uint.Parse(p.qty), p.dsr, p.chlitpdno, p.subsn, 0, null, 0);
                    }
                }
                if (!result)
                {
                    slaves = null;
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception("OrderImportMgr.GetSlave(List<OrdersImport> product, List<OrdersImport> all)-->" + ex.Message, ex);
            }
            return slaves;
        }
        #endregion

        #region 生成單筆訂單的OrderSlave,OrderDetail  gigade賣場
        /// <summary>
        /// 生成單筆訂單的OrderSlave,OrderDetail（訂單匯入  gigade）
        /// </summary>
        /// <param name="product"></param>
        /// <param name="all"></param>
        /// <param name="stockEnough"></param>
        /// <returns></returns>
        public List<OrderSlave> FillSlaveForG(List<OrdersImport> product, List<OrdersImport> all)
        {
            List<OrderSlave> slaves = new List<OrderSlave>();
            try
            {
                product = product.Where(m => m.dsr != "物流服務費").ToList();//去掉運費記錄
                bool result = true;
                foreach (OrdersImport p in product)
                {
                    if (string.IsNullOrEmpty(p.qty.Trim()) || p.qty.Trim() == "0") { continue; };
                    if (string.IsNullOrEmpty(p.sumup.Trim())) { p.sumup = "0"; };

                    //取得外站商品在數據庫具體商品信息
                    productMgr = new ProductMgr(MySqlConnStr);
                    productItemMgr = new ProductItemMgr(MySqlConnStr);

                    ProductItem prItem = productItemMgr.Query(new ProductItem { Item_Id = Convert.ToUInt32(p.chlitpdno) }).FirstOrDefault();

                    if (prItem == null)
                    {
                        all.FindAll(m => m.dmtshxuid == p.dmtshxuid && m.chlitpdno == p.chlitpdno).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("PRODUCT_NOT_EXISTS"));
                        result = false;
                        continue;
                    }
                    Product pr = productMgr.Query(new Product { Product_Id = prItem.Product_Id }).FirstOrDefault();
                    if (pr == null)
                    {
                        all.FindAll(m => m.dmtshxuid == p.dmtshxuid && m.chlitpdno == p.chlitpdno).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("PRODUCT_NOT_EXISTS"));
                        result = false;
                        continue;
                    }
                    //補貨中停止販售 1:是 0:否
                    if (pr.Shortage == 1)
                    {
                        all.FindAll(m => m.dmtshxuid == p.dmtshxuid && m.chlitpdno == p.chlitpdno).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("SHORT_AGE"));
                        result = false;
                        continue;
                    }
                    //庫存為0時是否還能販售 1:是 0:否
                    if (pr.Ignore_Stock == 0)//不存不足部能販賣就去看數量和庫存否則就不需要
                    {
                        if (prItem.Item_Stock < UInt32.Parse(p.qty))
                        {
                            all.FindAll(m => m.dmtshxuid == p.dmtshxuid && m.chlitpdno == p.chlitpdno).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("EMPTY_STOCK"));
                            result = false;
                            continue;
                        }
                    }

                    vendorBrandMgr = new VendorBrandMgr(MySqlConnStr);
                    deliveryFreightSetMappingMgr = new DeliveryFreightSetMappingMgr(MySqlConnStr);
                    VendorBrand vb = vendorBrandMgr.GetProductBrand(new VendorBrand { Brand_Id = pr.Brand_Id });
                    DeliveryFreightSetMapping df = deliveryFreightSetMappingMgr.GetDeliveryFreightSetMapping(new DeliveryFreightSetMapping { Product_Freight_Set = Convert.ToInt32(pr.Product_Freight_Set) });

                    #region OrderSlave

                    uint vendorId = pr.Product_Mode != 2 ? vb.Vendor_Id : Convert.ToUInt32((df.Delivery_Freight_Set == 1 ? 2 : 92));
                    OrderSlave slave = slaves.Where(m => m.Vendor_Id == vendorId).FirstOrDefault();
                    if (slave == null)
                    {
                        slave = new OrderSlave();
                        slave.Vendor_Id = vendorId;
                        slave.Slave_Status = 2;
                        slave.Slave_Product_Subtotal = uint.Parse(p.sumup);
                    }
                    else
                    {
                        slaves.Remove(slave);
                        slave.Slave_Product_Subtotal += uint.Parse(p.sumup);
                    }
                    slave.Slave_Amount = slave.Slave_Product_Subtotal;

                    #endregion

                    #region OrderDetail
                    OrderDetail newDetail = slave.Details.Where(m => m.Item_Id == prItem.Item_Id).FirstOrDefault();
                    if (newDetail == null)
                    {
                        newDetail = new OrderDetail();
                        newDetail.Item_Id = Convert.ToUInt32(prItem.Item_Id);
                        //edit by zhuoqin0830w 2015/11/02
                        //判斷商品 是否 買斷 商品  如果是  則 供應商 為 gigade 如果不是 則表示 使用原有的供應商
                        newDetail.Item_Vendor_Id = pr.Prepaid == 0 ? vb.Vendor_Id : Convert.ToUInt32((df.Delivery_Freight_Set == 1 ? 2 : 92));
                        newDetail.Detail_Status = 2;
                        newDetail.Buy_Num = uint.Parse(p.qty);
                        newDetail.Product_Freight_Set = pr.Product_Freight_Set;
                        newDetail.Product_Mode = pr.Product_Mode;
                        newDetail.Product_Name = p.dsr;
                        newDetail.Product_Spec_Name = prItem.GetSpecName();
                        newDetail.Single_Cost = prItem.Item_Cost;
                        newDetail.Single_Price = uint.Parse(p.sumup) / uint.Parse(p.qty);
                        newDetail.Single_Money = newDetail.Single_Price;
                        newDetail.Channel_Detail_Id = p.chlitpdno;
                        newDetail.Sub_Order_Id = p.subsn;
                        newDetail.Prepaid = pr.Prepaid;//edit by xiangwang0413w 2014/09/15 將product.prepaid寫入訂單
                    }
                    else
                    {
                        slave.Details.Remove(newDetail);
                        newDetail.Buy_Num += uint.Parse(p.qty);
                    }
                    slave.Details.Add(newDetail);
                    #endregion

                    slaves.Add(slave);
                }
                if (!result)
                {
                    slaves = null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("OrderImportMgr.FillSlaveForG-->" + ex.Message, ex);
            }
            return slaves;
        }
        #endregion

        #region Gigade 後台新增訂單－生成訂單的OrderSlave,OrderDetail
        /// <summary>
        ///Gigade 後台新增訂單－生成OrderSlave,OrderDetail（內部訂單輸入）
        /// </summary>
        /// <param name="odc"></param>
        /// <param name="errorOrder"></param>
        /// <returns></returns>
        public List<OrderSlave> FillSlave(List<OrderAddCustom> odc, List<OrderAddCustom> errorOrder, uint orderStatus, string txtOrderId)
        {
            List<OrderSlave> slaves = new List<OrderSlave>();
            try
            {
                bool result = true;
                uint pack_id = 0;
                string parent_name = "";
                //uint buyNum = 0;
                int parent_id = 0;
                int combined = 0;
                foreach (OrderAddCustom p in odc)
                {
                    List<OrderAddCustom> errorList = errorOrder.Where(rec => rec.Product_Id == p.Product_Id).ToList();
                    if (errorList.Count() > 0) { continue; }
                    //uint itemTotalBuyNum = 0;
                    productMgr = new ProductMgr(MySqlConnStr);
                    productItemMgr = new ProductItemMgr(MySqlConnStr);
                    specMgr = new ProductSpecMgr(MySqlConnStr);
                    Product pr = new Product();

                    //價格類型
                    int price_type = 0;
                    if (p.parent_id == 0)
                    {
                        price_type = p.price_type;
                    }
                    else
                    {
                        Product pResult = productMgr.Query(new Product { Product_Id = uint.Parse(p.parent_id.ToString()) }).FirstOrDefault();
                        price_type = pResult.Price_type;
                    }


                    IPriceMasterImplMgr priceMgr = new PriceMasterMgr(MySqlConnStr);

                    PriceMaster pM = null;
                    if (price_type == 2)        //各自定價時取價格
                    {
                        pM = priceMgr.QueryPriceMaster(new PriceMaster
                       {
                           product_id = p.parent_id == 0 ? p.Product_Id : uint.Parse(p.parent_id.ToString()),
                           user_id = 0,
                           user_level = 1,
                           site_id = 1,
                           child_id = int.Parse(p.Product_Id.ToString())
                       });
                    }
                    else
                    {
                        pM = priceMgr.QueryPriceMaster(new PriceMaster
                        {
                            product_id = p.Product_Id,
                            user_id = 0,
                            user_level = 1,
                            site_id = 1,
                            child_id = (p.parent_id == 0 && p.Item_Id == 0) ? int.Parse(p.Product_Id.ToString()) : 0
                        });
                    }


                    ProductItem prItem = new ProductItem();
                    if (p.Item_Id != 0)
                    {
                        prItem = productItemMgr.Query(new ProductItem { Item_Id = Convert.ToUInt32(p.Item_Id) }).FirstOrDefault();
                        pr = productMgr.Query(new Product { Product_Id = prItem.Product_Id }).FirstOrDefault();
                    }
                    else
                    {
                        pr = productMgr.Query(new Product { Product_Id = p.Product_Id }).FirstOrDefault();
                    }
                    if (pr != null && prItem != null)
                    {

                        #region 判斷庫存
                        /*
                        if (!(p.parent_id == 0 && p.Item_Id == 0))
                        {
                            foreach (var item in odc)
                            {
                                if (item.parent_id == 0 && item.Item_Id == 0 && item.Product_Id == p.parent_id)
                                {
                                    buyNum = item.buynum;
                                    continue;
                                }

                                if (item.parent_id == 0 && item.Item_Id != 0 && item.Product_Id == p.Product_Id)
                                {
                                    itemTotalBuyNum += item.buynum;
                                    continue;
                                }
                                if (item.Item_Id == p.Item_Id)
                                {
                                    uint sum = uint.Parse((item.s_must_buy * buyNum).ToString());
                                    itemTotalBuyNum += sum;
                                }

                            }


                            if (prItem.Item_Stock < itemTotalBuyNum)
                            {
                                p.msg = Resource.CoreMessage.GetResource("EMPTY_STOCK");
                                errorOrder.Add(p);
                                result = false;
                                continue;
                            }
                        }*/
                        #endregion

                        vendorBrandMgr = new VendorBrandMgr(MySqlConnStr);
                        deliveryFreightSetMappingMgr = new DeliveryFreightSetMappingMgr(MySqlConnStr);
                        VendorBrand vb = vendorBrandMgr.GetProductBrand(new VendorBrand { Brand_Id = pr.Brand_Id });
                        DeliveryFreightSetMapping df = deliveryFreightSetMappingMgr.GetDeliveryFreightSetMapping(new DeliveryFreightSetMapping { Product_Freight_Set = Convert.ToInt32(pr.Product_Freight_Set) });

                        #region OrderDetail
                        //OrderDetail newDetail = slave.Details.Where(m => m.Item_Id == p.Item_Id).FirstOrDefault();
                        OrderDetail newDetail = new OrderDetail();
                        //if (newDetail == null)
                        //{
                        //寄倉費
                        productMgr = new ProductMgr(MySqlConnStr);
                        Product prod = productMgr.Query(new Product { Product_Id = p.Product_Id }).FirstOrDefault();
                        if (prod != null)
                        {
                            newDetail.Bag_Check_Money = prod.Bag_Check_Money;
                            newDetail.Combined_Mode = int.Parse(prod.Combination.ToString());
                        }
                        if (p.Item_Id == 0)
                        {
                            pack_id++;
                            parent_name = p.product_name;
                            parent_id = int.Parse(p.Product_Id.ToString());
                            newDetail.item_mode = 1;//父商品
                            combined = newDetail.Combined_Mode;
                        }
                        //else
                        // {
                        #region OrderSlave

                        uint vendorId = pr.Product_Mode != 2 ? vb.Vendor_Id : Convert.ToUInt32((df.Delivery_Freight_Set == 1 ? 2 : 92));
                        OrderSlave slave = slaves.Where(m => m.Vendor_Id == vendorId).FirstOrDefault();
                        if (slave == null)
                        {
                            slave = new OrderSlave();
                            slave.Vendor_Id = vendorId;
                            slave.Slave_Note = txtOrderId;
                            slave.Slave_Product_Subtotal = p.Item_Id == 0 ? 0 : p.sumprice;
                            slave.Slave_Status = orderStatus;
                            slave.Slave_Updatedate = Convert.ToUInt32(CommonFunction.GetPHPTime());
                            slave.Slave_Ipfrom = CommonFunction.GetClientIP();
                        }
                        else
                        {
                            slaves.Remove(slave);
                            if (p.Item_Id != 0)
                            {
                                slave.Slave_Product_Subtotal += p.sumprice;
                            }
                        }
                        slave.Slave_Amount = slave.Slave_Product_Subtotal;

                        #endregion

                        if (p.parent_id != 0 && p.Item_Id != 0)
                        {
                            newDetail.Parent_Id = parent_id;
                            newDetail.item_mode = 2;//子商品
                            newDetail.Combined_Mode = combined;
                        }
                        //edit by xxl reason:父商品也需要pack_id;
                        newDetail.parent_name = parent_name;
                        newDetail.pack_id = pack_id;
                        //end by xxl  
                        //判斷商品 是否 買斷 商品  如果是  則 供應商 為 gigade 如果不是 則表示 使用原有的供應商  edit by zhuoqin0830w 2015/10/07
                        newDetail.Item_Vendor_Id = pr.Prepaid == 0 ? vb.Vendor_Id : Convert.ToUInt32((df.Delivery_Freight_Set == 1 ? 2 : 92));
                        //單一商品
                        if (p.parent_id == 0 && p.Item_Id != 0)
                        {
                            newDetail.item_mode = 0;
                            newDetail.Parent_Id = int.Parse(p.Product_Id.ToString());
                            newDetail.Prepaid = pr.Prepaid;//edit by xiangwang0413w 2014/09/15 將product.prepaid寫入訂
                        }
                        newDetail.Detail_Status = orderStatus;
                        newDetail.Product_Freight_Set = pr.Product_Freight_Set;
                        newDetail.Product_Mode = pr.Product_Mode;
                        newDetail.Product_Name = HttpUtility.HtmlEncode(pM.product_name);
                        newDetail.Product_Spec_Name = prItem.GetSpecName();
                        newDetail.Single_Cost = p.Item_Cost;  // 成本
                        //add by zhuoqin0830w  2015/04/29  內部訂單輸入功能新增 購物金金額、抵用卷金額、活動成本金額
                        newDetail.Event_Cost = p.Event_Item_Cost;// 活動成本// edit by zhuoqin0830w  添加 活動成本參數  2015/03/24  Ahon林志鴻 說需要更改
                        newDetail.Deduct_Bonus = p.deduct_bonus;
                        newDetail.Deduct_Welfare = p.deduct_welfare;
                        //add by zhuoqin0830w  2015/07/03  增加Site_Id
                        newDetail.Site_Id = (int)p.Site_Id;
                        //返還的購物金  add by zhuoqin0830w  2015/07/31
                        newDetail.Accumulated_Bonus = (int)p.accumulated_bonus;

                        long nowTime = BLL.gigade.Common.CommonFunction.GetPHPTime();
                        //若規格同價則單一商品的價格和成本從price_master中取，否則從item_price中取
                        if (p.price_master_id != 0)
                        {
                            PriceMaster priceM = priceMgr.Query(new PriceMaster { price_master_id = p.price_master_id }).FirstOrDefault();
                            newDetail.Single_Cost = uint.Parse(priceM.cost.ToString());
                            if (priceM.same_price == 1)//規格同價
                            {
                                //當前商品為組合商品的父商品時
                                if (p.parent_id == 0 && p.Item_Id == 0)
                                {
                                    newDetail.Single_Price = uint.Parse(priceM.price.ToString());  //原價
                                }
                                else
                                {
                                    newDetail.Single_Price = uint.Parse((priceM.price * p.s_must_buy).ToString());
                                }
                            }
                            else//不同價
                            {
                                if (p.parent_id == 0 && p.Item_Id == 0)
                                {
                                    newDetail.Single_Price = uint.Parse(odc.Where(rec => rec.parent_id != 0 && rec.Item_Id != 0 && rec.group_id == p.group_id).Sum(rec => rec.Item_Money * rec.s_must_buy).ToString());
                                }
                                else
                                {
                                    itemPriceMgr = new ItemPriceMgr(MySqlConnStr);
                                    ItemPrice it = itemPriceMgr.Query(new ItemPrice
                                    {
                                        item_id = p.Item_Id,
                                        price_master_id = priceM.price_master_id
                                    }).FirstOrDefault();
                                    if (it != null)
                                    {
                                        newDetail.Single_Price = uint.Parse((it.item_money * p.s_must_buy).ToString());
                                    }
                                }
                            }
                        }
                        newDetail.Single_Money = p.product_cost; // 定價
                        newDetail.price_master_id = p.price_master_id;
                        if (p.parent_id == 0 && p.Item_Id == 0)
                        {
                            //查找子商品item_id
                            OrderAddCustom od = odc.Where(rec => rec.group_id == p.group_id && rec.Item_Id != 0).FirstOrDefault();
                            newDetail.Item_Id = od.Item_Id;
                            newDetail.Parent_Id = int.Parse(p.Product_Id.ToString());//父商品的parent_id為其product_id
                            newDetail.parent_num = p.buynum;
                            newDetail.Buy_Num = p.buynum;

                            newDetail.Single_Price = (uint)pM.price;
                        }
                        else if (p.parent_id == 0 && p.Item_Id != 0)
                        {
                            newDetail.Item_Id = p.Item_Id;
                            newDetail.Buy_Num = p.buynum;
                            newDetail.parent_num = p.buynum;
                            //單一商品會進入此方法  所以在 價格方面 不必乘以 必買數量  edit by zhuoqin0830w  2015/06/23
                            newDetail.Single_Price = (uint)pM.price;
                            newDetail.Single_Money = (uint)p.product_cost;
                            newDetail.Single_Cost = (uint)p.Item_Cost;
                            newDetail.Event_Cost = (uint)p.Event_Item_Cost;
                        }
                        else
                        {
                            newDetail.Item_Id = p.Item_Id;
                            newDetail.parent_num = uint.Parse((p.buynum / p.s_must_buy).ToString());
                            newDetail.Buy_Num = uint.Parse(p.s_must_buy.ToString());
                            newDetail.Single_Price = (uint)(prItem.Item_Money * p.s_must_buy);
                            newDetail.Single_Money = (uint)(p.product_cost * p.s_must_buy);
                            newDetail.Single_Cost = (uint)(p.Item_Cost * p.s_must_buy);
                            newDetail.Event_Cost = (uint)(p.Event_Item_Cost * p.s_must_buy);
                        }
                        if (nowTime > prItem.Event_Product_Start && nowTime < prItem.Event_Product_End)
                        {
                            newDetail.Single_Cost = p.Event_Item_Cost;
                        }
                        slave.Details.Add(newDetail);
                        slaves.Add(slave);
                        // }
                        //}
                        //else
                        //{
                        //    slave.Details.Remove(newDetail);
                        //    newDetail.Buy_Num += p.buynum;
                        //}
                        #endregion
                    }
                    else
                    {
                        p.msg = Resource.CoreMessage.GetResource("PRODUCT_NOT_EXISTS");
                        errorOrder.Add(p);
                        result = false;
                        continue;
                    }
                }
                if (!result)
                {
                    slaves = null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("OrderImportMgr-->FillSlave(List<OrderAddCustom> odc,List<OrderAddCustom> errorOrder,uint orderStatus,string txtOrderId)-->" + ex.Message, ex);
            }
            return slaves;
        }
        #endregion

        #region 合作賣場 後台新增訂單－生成訂單的OrderSlave,OrderDetail
        /// <summary>
        ///合作外站 後台新增訂單－生成OrderSlave,OrderDetail （訂單輸入 合作）
        /// </summary>
        /// <param name="odc"></param>
        /// <param name="errorOrder"></param>
        /// <returns></returns>
        public List<OrderSlave> FillSlaveCooperator(List<CooperatorOrderCustom> odc, List<CooperatorOrderCustom> errorOrder, uint channelId, uint orderStatus, string txtOrderId)
        {
            List<OrderSlave> slaves = new List<OrderSlave>();
            //查詢外站組合商品子商品組成數量(product_map_set.set_num) edit by xiangwang0413w 2014/08/21 
            var parentProduct = odc.Find(m => m.Item_Id == 0);
            List<ProductMapSet> proMapSets = parentProduct != null ? new ProductMapSetMgr(MySqlConnStr).Query(new ProductItemMap
            {
                channel_id = channelId,
                channel_detail_id = parentProduct.coop_product_id,
                price_master_id = parentProduct.price_master_id
            }) : null;

            try
            {
                bool result = true;
                uint pack_id = 0;
                string parent_name = "";
                //uint buyNum = 0;
                string channel_detail_id = "";
                uint combined = 0;
                uint parent_id = 0;
                foreach (CooperatorOrderCustom p in odc)
                {
                    //uint itemTotalBuyNum = 0;
                    IPriceMasterImplMgr priceMgr = new PriceMasterMgr(MySqlConnStr);
                    productMgr = new ProductMgr(MySqlConnStr);
                    productItemMgr = new ProductItemMgr(MySqlConnStr);
                    productItemMapMgr = new ProductItemMapMgr(MySqlConnStr);
                    specMgr = new ProductSpecMgr(MySqlConnStr);
                    ProductItem prItem = new ProductItem();
                    Product pr = new Product();
                    ProductItemMap pMap = productItemMapMgr.QueryAll(new ProductItemMap { channel_id = channelId, channel_detail_id = p.parent_id == "0" ? p.coop_product_id : p.parent_id }).FirstOrDefault();

                    if (p.Item_Id != 0)
                    {
                        prItem = productItemMgr.Query(new ProductItem { Item_Id = p.Item_Id }).FirstOrDefault();
                        pr = productMgr.Query(new Product { Product_Id = prItem.Product_Id }).FirstOrDefault();
                        if (p.parent_id == "0")
                        {
                            //p.price_master_id = priceMgr.Query(new PriceMaster
                            //{
                            //    product_id = prItem.Product_Id,
                            //    user_id = 0,
                            //    user_level = 1,
                            //    site_id = 1,
                            //    child_id = 0
                            //}).FirstOrDefault().price_master_id;
                            //edit by xiangwang0413w 2014/07/09 外站的成本可以設定,不再默認取吉甲地站臺的
                            p.price_master_id = pMap.price_master_id;
                        }
                    }
                    else
                    {
                        pr = productMgr.Query(new Product { Product_Id = pMap.product_id }).FirstOrDefault();
                    }

                    if (prItem != null)//add by jiajun 2014/08/25
                    {
                        #region 判斷庫存
                        if (p.Item_Id != 0)
                        {
                            if (p.ignore_stock == 0 && prItem.Item_Stock < p.buynum)//庫存不足時提示
                            {
                                Resource.CoreMessage.GetResource("EMPTY_STOCK");
                                errorOrder.Add(p);
                                result = false;
                                continue;
                            }
                        }

                        //foreach (var item in odc)
                        //{
                        //    if (item.parent_id == "0" && item.Item_Id == 0)
                        //    {
                        //        buyNum = item.buynum;
                        //        continue;
                        //    }
                        //    else if (item.Item_Id != 0 && item.parent_id == "0" && item.Item_Id == p.Item_Id)
                        //    {
                        //        itemTotalBuyNum += item.buynum;
                        //        continue;
                        //    }
                        //    if (item.Item_Id == p.Item_Id)
                        //    {
                        //        uint sum = uint.Parse((item.s_must_buy * buyNum).ToString());
                        //        itemTotalBuyNum += sum;
                        //    }

                        //}

                        //if (!(p.Item_Id == 0 && p.parent_id == "0"))
                        //{
                        //    if (prItem.Item_Stock < itemTotalBuyNum)
                        //    {
                        //        p.msg = Resource.CoreMessage.GetResource("EMPTY_STOCK");
                        //        errorOrder.Add(p);
                        //        result = false;
                        //        continue;
                        //    }
                        //}

                        #endregion

                        vendorBrandMgr = new VendorBrandMgr(MySqlConnStr);
                        deliveryFreightSetMappingMgr = new DeliveryFreightSetMappingMgr(MySqlConnStr);
                        VendorBrand vb = vendorBrandMgr.GetProductBrand(new VendorBrand { Brand_Id = pr.Brand_Id });
                        DeliveryFreightSetMapping df = deliveryFreightSetMappingMgr.GetDeliveryFreightSetMapping(new DeliveryFreightSetMapping { Product_Freight_Set = Convert.ToInt32(pr.Product_Freight_Set) });

                        #region OrderDetail
                        OrderDetail newDetail = new OrderDetail();

                        if (p.Item_Id == 0)
                        {
                            pack_id++;
                            channel_detail_id = p.coop_product_id;
                            parent_name = p.product_name;
                            pr = productMgr.Query(new Product { Product_Id = pMap.product_id }).FirstOrDefault();
                            newDetail.item_mode = 1;//父商品
                            combined = pr.Combination;
                            newDetail.Combined_Mode = int.Parse(combined.ToString());
                            parent_id = pr.Product_Id;
                            newDetail.Bag_Check_Money = pr.Bag_Check_Money;
                        }
                        else
                        {
                            newDetail.Bag_Check_Money = pr.Bag_Check_Money;
                            newDetail.Combined_Mode = int.Parse(pr.Combination.ToString());
                        }
                        #region OrderSlave

                        uint vendorId = pr.Product_Mode != 2 ? vb.Vendor_Id : Convert.ToUInt32((df.Delivery_Freight_Set == 1 ? 2 : 92));
                        OrderSlave slave = slaves.Where(m => m.Vendor_Id == vendorId).FirstOrDefault();
                        if (slave == null)
                        {
                            slave = new OrderSlave();
                            slave.Vendor_Id = vendorId;
                            slave.Slave_Note = txtOrderId;
                            slave.Slave_Product_Subtotal = p.sumprice;
                            slave.Slave_Status = orderStatus;
                            slave.Slave_Updatedate = Convert.ToUInt32(CommonFunction.GetPHPTime());
                            slave.Slave_Ipfrom = CommonFunction.GetClientIP();
                        }
                        else
                        {
                            slaves.Remove(slave);
                            if (p.Item_Id != 0)
                            {
                                slave.Slave_Product_Subtotal += p.sumprice;
                            }
                        }
                        slave.Slave_Amount = slave.Slave_Product_Subtotal;

                        #endregion
                        if (p.parent_id != "0" && p.Item_Id != 0)
                        {
                            newDetail.Parent_Id = int.Parse(parent_id.ToString());
                            newDetail.item_mode = 2;
                            newDetail.Combined_Mode = int.Parse(combined.ToString());
                        }
                        //edit by xxl reason:父商品也需要pack_id,parent_name
                        newDetail.pack_id = pack_id;
                        newDetail.parent_name = parent_name;
                        //end by xxl
                        newDetail.Item_Vendor_Id = vb.Vendor_Id;
                        //單一商品
                        if (p.parent_id == "0" && p.Item_Id != 0)
                        {
                            channel_detail_id = p.coop_product_id;
                            newDetail.item_mode = 0;
                            newDetail.Parent_Id = int.Parse(pr.Product_Id.ToString());
                            newDetail.Prepaid = pr.Prepaid;//edit by xiangwang0413w 2014/09/15 將product.prepaid寫入訂單
                        }
                        newDetail.Detail_Status = orderStatus;
                        newDetail.Product_Freight_Set = pr.Product_Freight_Set;
                        newDetail.Product_Mode = pr.Product_Mode;
                        newDetail.Product_Name = HttpUtility.HtmlEncode(p.product_name);
                        newDetail.Product_Spec_Name = prItem.GetSpecName();
                        newDetail.Event_Cost = p.Event_Item_Cost;
                        newDetail.Channel_Detail_Id = channel_detail_id;
                        newDetail.price_master_id = p.price_master_id;

                        if (p.price_master_id != 0)
                        {
                            if (p.parent_id == "0" && p.Item_Id != 0)//單一商品
                            {
                                itemPriceMgr = new ItemPriceMgr(MySqlConnStr);
                                ItemPrice it = itemPriceMgr.Query(new ItemPrice
                                {
                                    item_id = p.Item_Id,
                                    price_master_id = p.price_master_id
                                }).FirstOrDefault();
                                newDetail.Single_Price = it.item_money;
                                newDetail.Single_Cost = it.item_cost;

                                newDetail.Item_Id = p.Item_Id;
                                newDetail.Buy_Num = p.buynum;
                                newDetail.parent_num = p.buynum;
                                newDetail.Single_Money = p.product_cost;
                            }
                            else if (p.parent_id == "0" && p.Item_Id == 0)//當前商品為組合商品的父商品時
                            {
                                PriceMaster priceM = priceMgr.Query(new PriceMaster { price_master_id = p.price_master_id }).FirstOrDefault();
                                newDetail.Single_Price = (uint)priceM.price;
                                newDetail.Single_Cost = (uint)priceM.cost;

                                ////查找子商品item_id
                                CooperatorOrderCustom od = odc.Where(rec => rec.group_id == p.group_id && rec.Item_Id != 0).FirstOrDefault();
                                newDetail.Item_Id = od.Item_Id;
                                newDetail.Parent_Id = int.Parse(pr.Product_Id.ToString());//父商品的parent_id為其product_id
                                newDetail.parent_num = p.buynum;
                                newDetail.Buy_Num = p.buynum;
                                newDetail.Single_Money = p.product_cost;
                            }
                            else//組合商品子商品
                            {
                                itemPriceMgr = new ItemPriceMgr(MySqlConnStr);
                                ItemPrice it = itemPriceMgr.Query(new ItemPrice
                                {
                                    item_id = p.Item_Id,
                                    price_master_id = p.price_master_id
                                }).FirstOrDefault();

                                uint set_num = proMapSets.Find(m => m.item_id == p.Item_Id).set_num;//子商品對應組成商品數量
                                newDetail.Single_Price = it.item_money * set_num;
                                newDetail.Single_Cost = it.item_cost * set_num;

                                newDetail.Item_Id = p.Item_Id;
                                newDetail.Buy_Num = p.buynum;
                                newDetail.parent_num = p.buynum / set_num;
                                newDetail.Single_Money = p.product_cost * set_num;
                            }
                        }
                        long nowTime = BLL.gigade.Common.CommonFunction.GetPHPTime();
                        if (nowTime > prItem.Event_Product_Start && nowTime < prItem.Event_Product_End)
                        {
                            // newDetail.Single_Cost = p.Event_Item_Cost;
                        }
                        slave.Details.Add(newDetail);
                        slaves.Add(slave);
                        #endregion
                    }
                    else
                    {
                        p.msg = Resource.CoreMessage.GetResource("PRODUCT_NOT_EXISTS");
                        errorOrder.Add(p);
                        result = false;
                        break;
                    }
                }
                if (!result)
                {
                    slaves = null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("OrderImportMgr.FillSlaveCooperator-->" + ex.Message, ex);
            }
            return slaves;
        }
        #endregion

        #region 單個商品填充orderslave
        /// <summary>
        /// 單個商品填充orderslave  (匯入訂單時 組合商品和單一商品都會進入此方法 插入 order_slave 和 order_detail 表的 數據)
        /// </summary>
        /// <param name="slaves">需填充的orderslave集合</param>
        /// <param name="pro">商品信息</param>
        /// <param name="proItem">商品細項信息</param>
        /// <param name="price">商品價格信息</param>
        /// <param name="sumPrice">總價</param>
        /// <param name="buyCount">購買數量</param>
        /// <param name="buyName">購買時名稱</param>
        /// <param name="channel_Detail_Id">賣場商品編號</param>
        /// <param name="sub_SN">賣場訂單編號</param>
        /// <param name="pack_Id">項目編號，同一組合商品內具體商品相同</param>
        public void FillSlave(List<OrderSlave> slaves, Product pro, ProductItem proItem, PriceMaster price, uint sumPrice, uint buyCount, string buyName, string channel_Detail_Id, string sub_SN, uint pack_Id, Product parentPro, uint parentNum)
        {
            vendorBrandMgr = new VendorBrandMgr(MySqlConnStr);
            deliveryFreightSetMappingMgr = new DeliveryFreightSetMappingMgr(MySqlConnStr);
            VendorBrand vb = vendorBrandMgr.GetProductBrand(new VendorBrand { Brand_Id = pro.Brand_Id });
            DeliveryFreightSetMapping df = deliveryFreightSetMappingMgr.GetDeliveryFreightSetMapping(new DeliveryFreightSetMapping { Product_Freight_Set = Convert.ToInt32(pro.Product_Freight_Set) });

            #region OrderSlave
            uint vendorId = pro.Product_Mode != 2 ? vb.Vendor_Id : Convert.ToUInt32((df.Delivery_Freight_Set == 1 ? 2 : 92));
            OrderSlave slave = slaves.Where(m => m.Vendor_Id == vendorId).FirstOrDefault();
            if (slave == null)
            {
                slave = new OrderSlave();
                slave.Vendor_Id = vendorId;
                slave.Slave_Status = 2;
                slave.Slave_Product_Subtotal = sumPrice;
            }
            else
            {
                slaves.Remove(slave);
                if (parentPro != pro)
                {
                    slave.Slave_Product_Subtotal += sumPrice;
                }
            }
            slave.Slave_Amount = slave.Slave_Product_Subtotal;

            #endregion

            #region OrderDetail
            OrderDetail newDetail = new OrderDetail();
            newDetail.Item_Id = Convert.ToUInt32(proItem.Item_Id);
            //edit by zhuoqin0830w 2015/08/21
            //判斷商品 是否 買斷 商品  如果是  則 供應商 為 gigade 如果不是 則表示 使用原有的供應商  
            newDetail.Item_Vendor_Id = pro.Prepaid == 0 ? vb.Vendor_Id : Convert.ToUInt32((df.Delivery_Freight_Set == 1 ? 2 : 92));
            newDetail.Detail_Status = 2;
            newDetail.Buy_Num = buyCount;
            newDetail.Product_Freight_Set = pro.Product_Freight_Set;
            newDetail.Product_Mode = pro.Product_Mode;
            newDetail.Product_Name = buyName;
            newDetail.Product_Spec_Name = proItem.GetSpecName();
            newDetail.Single_Cost = Convert.ToUInt32(price.cost); //price.same_price == 1 ? Convert.ToUInt32(price.cost) : proItem.Item_Cost;//售價  成本

            //判斷特價活動成本是否過期，如果過期就 賦值 為 0 如果沒有則使用 特價 活動成本  eidt by zhuoqin0830w  2015/10/15  通過溝通 ahon 決定更改 因為前台代碼有判斷 成本取值是活動成本還是原成本
            uint nowTime = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
            if (nowTime > price.event_start && nowTime < price.event_end)
            { newDetail.Event_Cost = 0; }
            else { newDetail.Event_Cost = Convert.ToUInt32(price.event_cost); }

            newDetail.Single_Price = Convert.ToUInt32(price.price);
            newDetail.Single_Money = buyCount != 0 ? sumPrice / buyCount : 0;
            newDetail.Channel_Detail_Id = channel_Detail_Id;
            newDetail.Sub_Order_Id = sub_SN;
            newDetail.price_master_id = price.price_master_id;
            newDetail.Bag_Check_Money = pro.Bag_Check_Money;
            newDetail.parent_num = parentNum;//edit by hufeng0813w 2014/06/06 子商品也需要寫入組合商品的購買數量
            newDetail.Prepaid = pro.Prepaid;//edit by xiangwang0413w 2014/09/15 將product.prepaid寫入訂單
            if (parentPro != null && parentPro.Combination != 0 && parentPro.Combination != 1)
            {
                newDetail.Combined_Mode = int.Parse(parentPro.Combination.ToString());
                newDetail.Parent_Id = int.Parse(parentPro.Product_Id.ToString());
                newDetail.pack_id = pack_Id;
                newDetail.parent_name = parentPro.Product_Name;
                newDetail.item_mode = Convert.ToUInt32(parentPro == pro ? 1 : 2);
            }
            else
            {
                newDetail.Combined_Mode = int.Parse(pro.Combination.ToString());
                newDetail.Parent_Id = int.Parse(pro.Product_Id.ToString());
                newDetail.item_mode = 0;
            }
            slave.Details.Add(newDetail);

            #endregion

            slaves.Add(slave);
        }
        #endregion

        #region 訂單保存至數據庫
        /// <summary>
        /// 訂單保存至數據庫
        /// </summary>
        /// <param name="master">order_master</param>
        /// <param name="slaves">order_slaves</param>
        /// <param name="cOrders"></param>
        /// <param name="all"></param>
        /// <param name="op">order_master_pattern</param>
        /// <param name="bm">bonus_master</param>
        /// <param name="br">bonus_record</param>
        /// <returns></returns>
        public bool Save2DB(OrderMaster master, List<OrderSlave> slaves, List<ChannelOrder> cOrders, List<OrdersImport> all, OrderMasterPattern op = null, BonusMaster bm = null, BonusRecord brBonus = null, BonusRecord brWelfare = null)
        {
            try
            {
                orderMasterMgr = new OrderMasterMgr(MySqlConnStr);
                string orderMasterSql = orderMasterMgr.Save(master);
                //add hxw 2015/05/27
                _orderMasterPatternMgr = new OrderMasterPatternMgr("");
                string orderMasterPatternSql = op == null ? string.Empty : _orderMasterPatternMgr.Save(op);

                // 添加  華南賬戶（虛擬帳號） orderPayment  add by zhuoqin0830w  2015/05/13
                string orderPayment = string.Empty;
                OrderPaymentHncb orderPaymentHncb = new OrderPaymentHncb();
                if (master.Order_Payment == 2 && master.Order_Status == 0)
                {
                    _orderPaymentHncbMgr = new OrderPaymentHncbMgr(MySqlConnStr);
                    orderPayment = _orderPaymentHncbMgr.AddPaymentHncb(orderPaymentHncb);
                }

                #region 使用 購物金 和 抵用卷   add by zhuoqin0830w 2015/08/24
                //添加 bonus_master 的數據新增  add by zhuoqin0830w 2015/08/24
                SerialDao _serialDao = new SerialDao(MySqlConnStr);
                _bonusMasterMgr = new BonusMasterMgr(MySqlConnStr);
                //使用 Serial 表中的 流水賬號
                if (bm != null)
                {
                    Serial ser = _serialDao.GetSerialById(27);
                    ser.Serial_Value = ser.Serial_Value + 1;
                    _serialDao.Update(ser);
                    bm.master_id = Convert.ToUInt32(ser.Serial_Value);
                }
                string bonusMaster = bm == null ? string.Empty : _bonusMasterMgr.AddBonusMaster(bm);
                //對 bonus_record 的數據新增  和 對 bonus_master 數據 修改  add by zhuoqin0830w 2015/08/25
                _bonusRecordMgr = new BonusRecordMgr(MySqlConnStr);
                string bonusRecord = string.Empty;
                //購物金
                if (brBonus != null)
                {
                    List<BonusMaster> queryBonusMaster = _bonusMasterMgr.GetBonusByEndTime(brBonus);
                    //將前臺的到的購物金額存儲
                    int useBonus = (int)brBonus.record_use;
                    foreach (BonusMaster bonus in queryBonusMaster)
                    {
                        //判斷 從 bonus_master 裱中查詢出來的 master_balance 是否 大於用戶 使用金額  如果大於 就 使用 用戶金額  如果小於  則使用 master_balance 金額 
                        int decuteBonusNum = bonus.master_balance > useBonus ? useBonus : bonus.master_balance;
                        if (decuteBonusNum > 0)
                        {
                            //使用 Serial 表中的 流水賬號
                            Serial ser = _serialDao.GetSerialById(28);
                            ser.Serial_Value = ser.Serial_Value + 1;
                            _serialDao.Update(ser);
                            brBonus.record_id = Convert.ToUInt32(ser.Serial_Value);
                            brBonus.master_id = bonus.master_id;
                            brBonus.type_id = bonus.type_id;
                            brBonus.record_use = Convert.ToUInt32(decuteBonusNum);
                            brBonus.record_note = bonus.master_note;
                            brBonus.record_writer = bonus.master_writer;
                            bonusRecord += _bonusRecordMgr.InsertBonusRecord(brBonus);
                            //使 bonus_master 裱中的 購物金 減去 使用的購物金
                            bonus.master_balance = bonus.master_balance - decuteBonusNum;
                            bonus.master_updatedate = Convert.ToUInt32(BLL.gigade.Common.CommonFunction.GetPHPTime());
                            bonusMaster += _bonusMasterMgr.UpdateBonusMasterBalance(bonus);
                            //將 前臺傳來的總金額 減去使用金額
                            useBonus -= decuteBonusNum;
                            //如果useBonus == 0 為 true  則表示 使用金已經用完 可以不用在循環到下一步
                            if (useBonus == 0) { break; }
                        }
                    }
                }
                //抵用卷
                if (brWelfare != null)
                {
                    List<BonusMaster> queryBonusMaster = _bonusMasterMgr.GetWelfareByEndTime(brWelfare);
                    //將前臺的到的抵用券金額存儲
                    int useWelfare = (int)brWelfare.record_use;
                    foreach (BonusMaster bonus in queryBonusMaster)
                    {
                        //判斷 從 bonus_master 裱中查詢出來的 master_balance 是否 大於用戶 使用金額  如果大於 就 使用 用戶金額  如果小於  則使用 master_balance 金額 
                        int decuteWelfareNum = bonus.master_balance > useWelfare ? useWelfare : bonus.master_balance;
                        if (decuteWelfareNum > 0)
                        {
                            //使用 Serial 表中的 流水賬號
                            Serial ser = _serialDao.GetSerialById(28);
                            ser.Serial_Value = ser.Serial_Value + 1;
                            _serialDao.Update(ser);
                            brWelfare.record_id = Convert.ToUInt32(ser.Serial_Value);
                            brWelfare.master_id = bonus.master_id;
                            brWelfare.type_id = bonus.type_id;
                            brWelfare.record_use = Convert.ToUInt32(decuteWelfareNum);
                            brWelfare.record_note = bonus.master_note;
                            brWelfare.record_writer = bonus.master_writer;
                            bonusRecord += _bonusRecordMgr.InsertBonusRecord(brWelfare);
                            //使 bonus_master 裱中的 購物金 減去 使用的購物金
                            bonus.master_balance = bonus.master_balance - decuteWelfareNum;
                            bonus.master_updatedate = Convert.ToUInt32(BLL.gigade.Common.CommonFunction.GetPHPTime());
                            bonusMaster += _bonusMasterMgr.UpdateBonusMasterBalance(bonus);
                            //將 前臺傳來的總金額 減去使用金額
                            useWelfare -= decuteWelfareNum;
                            //如果useBonus == 0 為 true  則表示 使用金已經用完 可以不用在循環到下一步
                            if (useWelfare == 0) { break; }
                        }
                    }
                }
                #endregion

                ArrayList orderSlavesSql = new ArrayList();
                ArrayList orderDetailsSql = new ArrayList();
                ArrayList otherSql = new ArrayList();

                orderSlaveMgr = new OrderSlaveMgr("");
                orderDetailMgr = new OrderDetailMgr("");
                productItemMgr = new ProductItemMgr(MySqlConnStr);
                ProductItem pro;
                foreach (var slave in slaves)
                {
                    //記錄OrderSlave
                    orderSlavesSql.Add(orderSlaveMgr.Save(slave));
                    ArrayList detailSql = new ArrayList();
                    //記錄OrderDetail 子商品先存入數據庫
                    //foreach (var detail in slave.Details)
                    //{
                    //    detailSql.Add(orderDetailMgr.Save(detail));
                    //    if (detail.item_mode != 1)
                    //    {
                    //        pro = productItemMgr.Query(new ProductItem { Item_Id = detail.Item_Id }).FirstOrDefault();
                    //        pro.Item_Stock = Convert.ToInt32(detail.Buy_Num);

                    //        otherSql.Add(productItemMgr.UpdateStock(pro));
                    //    }
                    //}
                    //add by hufeng0813w 2014/06/09 reason:父商品先存入數據庫
                    foreach (var item in slave.Details.OrderBy(m => m.item_mode))
                    {
                        detailSql.Add(orderDetailMgr.Save(item));
                        if (item.item_mode != 1)
                        {
                            pro = productItemMgr.Query(new ProductItem { Item_Id = item.Item_Id }).FirstOrDefault();
                            pro.Item_Stock = Convert.ToInt32(item.Buy_Num);

                            otherSql.Add(productItemMgr.UpdateStock(pro));
                        }
                    }
                    orderDetailsSql.Add(detailSql);
                }

                //記錄ChannelOrder
                channelOrderMgr = new ChannelOrderMgr("");
                cOrders.ForEach(m => otherSql.Add(channelOrderMgr.Save(m)));
                // 添加  華南賬戶（虛擬帳號） orderPayment  add by zhuoqin0830w  2015/05/13 // 添加 bonusMaster 和 bonusRecord 數據 add by zhuoqin0830w  2015/08/24
                return orderMasterMgr.SaveOrder(orderMasterSql, orderMasterPatternSql, orderPayment, orderSlavesSql, orderDetailsSql, otherSql, bonusMaster, bonusRecord);
            }
            catch (Exception ex)
            {
                if (all != null)
                {
                    all.FindAll(m => m.dmtshxuid == master.Channel_Order_Id && m.IsSel).ForEach(m => m.Msg = Resource.CoreMessage.GetResource("SAVE_TO_DB_FAILURE"));
                }
                throw new Exception("OrderImportMgr.Save2DB(OrderMaster master, List<OrderSlave> slaves, List<ChannelOrder> cOrders, List<OrdersImport> all)-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 查询商品對照

        public ProductItemMap QueryProductMapping(OrdersImport pro)
        {
            if (pro.dsr == "物流服務費")
            {
                return null;
            }
            else
            {
                try
                {
                    ProductItemMap query = new ProductItemMap { channel_id = Convert.ToUInt32(ChannelID), channel_detail_id = pro.chlitpdno };
                    productItemMapMgr = new ProductItemMapMgr(MySqlConnStr);
                    return productItemMapMgr.QueryAll(query).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw new Exception("OrderImportMgr-->QueryProductMapping-->" + ex.Message, ex);
                }
            }
        }
        #endregion

        #region 訂單是否存在

        public bool IsExistsOrder(OrdersImport pro)
        {
            try
            {
                channelOrderMgr = new ChannelOrderMgr(MySqlConnStr);
                var t = channelOrderMgr.Query(new ChannelOrder { Order_Id = pro.dmtshxuid, Channel_Id = ChannelID });
                return t.Count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderImportMgr-->IsExistsOrder-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 查询商品细项

        public ProductItem QueryProductItem(uint productItemId)
        {
            try
            {
                productItemMgr = new ProductItemMgr(MySqlConnStr);
                return productItemMgr.Query(new ProductItem { Item_Id = productItemId }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderImportMgr-->QueryProductItem-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 查询商品
        public Product QueryProduct(uint produtId)
        {
            try
            {
                productMgr = new ProductMgr(MySqlConnStr);
                return productMgr.Query(new Product { Product_Id = produtId }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderImportMgr-->QueryProduct-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}