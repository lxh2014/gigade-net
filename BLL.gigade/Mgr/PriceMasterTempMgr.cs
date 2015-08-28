using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Mgr.Impl;
using System.Collections;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class PriceMasterTempMgr : IPriceMasterTempImplMgr
    {
        private IPriceMasterTempImplDao _priceMasterTempDao;
        string connctionString = string.Empty;
        public PriceMasterTempMgr(string connectionStr)
        {
            _priceMasterTempDao = new PriceMasterTempDao(connectionStr);
            this.connctionString = connectionStr;
        }

        public bool Save(List<Model.PriceMasterTemp> priceMasterTempList, List<List<ItemPrice>> itemPrices, ArrayList others)
        {
            if (priceMasterTempList == null)
            {
                return false;
            }
            if (itemPrices != null && itemPrices.Count > 0 && priceMasterTempList.Count != itemPrices.Count)
            {
                return false;
            }
            ArrayList sqls = new ArrayList();
            ArrayList prices = new ArrayList();
            ItemPriceTempMgr itemPriceTempMgr = new ItemPriceTempMgr("");
            foreach (var item in priceMasterTempList)
            {
                sqls.Add(_priceMasterTempDao.Save(item));

                if (itemPrices != null && itemPrices.Count > 0)
                {
                    ArrayList price = new ArrayList();
                    List<ItemPrice> items = itemPrices[priceMasterTempList.IndexOf(item)];
                    if (items != null)
                    {
                        foreach (var tp in items)
                        {
                            price.Add(itemPriceTempMgr.Save(tp));
                        }
                    }
                    prices.Add(price);
                }
            }
            return _priceMasterTempDao.Save(sqls, prices, others);
        }

        public bool Update(List<Model.PriceMasterTemp> priceMasterTempList, List<ItemPrice> itemPrices)
        {
            ArrayList sqls = new ArrayList();
            if (priceMasterTempList != null)
            {
                foreach (var item in priceMasterTempList)
                {
                    sqls.Add(_priceMasterTempDao.Update(item));
                }
            }
            if (itemPrices != null)
            {
                ItemPriceTempMgr itemPriceTempMgr = new ItemPriceTempMgr("");
                foreach (var item in itemPrices)
                {
                    sqls.Add(itemPriceTempMgr.Update(item));
                }
            }
            MySqlDao mySqlDao = new MySqlDao(connctionString);
            return mySqlDao.ExcuteSqls(sqls);
        }

        public Model.Custom.PriceMasterProductCustom Query(Model.PriceMasterTemp priceMasterTemp)
        {
            Model.Custom.PriceMasterProductCustom pmpc = _priceMasterTempDao.Query(priceMasterTemp);
            if(pmpc != null)
                pmpc.product_name = Model.PriceMaster.Product_Name_Op(pmpc.product_name);
            return pmpc;
        }
        public string Delete(Model.PriceMasterTemp priceMasterTemp)
        {
            return _priceMasterTempDao.Delete(priceMasterTemp);
        }

        public string ChildDelete(PriceMasterTemp priceMasterTemp)
        {
            return _priceMasterTempDao.ChildDelete(priceMasterTemp);
        }

        public List<PriceMasterTemp> queryChild(PriceMasterTemp query)
        {
            return _priceMasterTempDao.queryChild(query);
        }

        public string SelectChild(Model.PriceMasterTemp priceMasterTemp)
        {
            return _priceMasterTempDao.SelectChild(priceMasterTemp);
        }

        public string Move2PriceMaster(Model.PriceMasterTemp priceMasterTemp)
        {
            return _priceMasterTempDao.Move2PriceMaster(priceMasterTemp);
        }

        public string Move2PriceMasterByMasterId()
        {
            return _priceMasterTempDao.Move2PriceMasterByMasterId();
        }

        public string SaveFromPriceMaster(Model.PriceMasterTemp priceMasterTemp)
        {
            return _priceMasterTempDao.SaveFromPriceMaster(priceMasterTemp);
        }
        public string SaveFromPriceMasterByMasterId(PriceMasterTemp priceMasterTemp)
        {
            return _priceMasterTempDao.SaveFromPriceMasterByMasterId(priceMasterTemp);
        }




        #region 供應商商品處理
        public string DeleteByVendor(PriceMasterTemp priceMasterTemp)
        {
            return _priceMasterTempDao.DeleteByVendor(priceMasterTemp);
        }

        #region 根據model 查詢供應商數據信息生成PriceMasterProductCustomTemp對象 +PriceMasterProductCustomTemp QueryByVendor(PriceMasterTemp priceMasterTemp)
        public Model.Custom.PriceMasterProductCustomTemp QueryByVendor(PriceMasterTemp priceMasterTemp)
        {
            try
            {
                return _priceMasterTempDao.QueryByVendor(priceMasterTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempMgr-->QueryByVendor-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 供應商商品價格插入

        public bool SaveByVendor(List<Model.PriceMasterTemp> priceMasterTempList, List<List<ItemPrice>> itemPrices, ArrayList others)
        {
            if (priceMasterTempList == null)
            {
                return false;
            }
            if (itemPrices != null && itemPrices.Count > 0 && priceMasterTempList.Count != itemPrices.Count)
            {
                return false;
            }
            ArrayList sqls = new ArrayList();
            ArrayList prices = new ArrayList();
            try
            {
                ItemPriceTempMgr itemPriceTempMgr = new ItemPriceTempMgr("");
                foreach (var item in priceMasterTempList)
                {
                    sqls.Add(_priceMasterTempDao.SaveByVendor(item));

                    //if (itemPrices != null && itemPrices.Count > 0)
                    //{
                    //    ArrayList price = new ArrayList();
                    //    List<ItemPrice> items = itemPrices[priceMasterTempList.IndexOf(item)];
                    //    if (items != null)
                    //    {
                    //        foreach (var tp in items)
                    //        {
                    //            price.Add(itemPriceTempMgr.Save(tp));
                    //        }
                    //    }
                    //    prices.Add(price);
                    //}
                }
                return _priceMasterTempDao.Save(sqls, prices, others);
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempMgr-->SaveByVendor-->" + ex.Message + sqls.ToString(), ex);
            }
        }
        #endregion
        #region 供應商商品價格更新
        public string UpdateTs(Model.Custom.PriceMasterCustom pM)
        {
            try
            {
                return _priceMasterTempDao.UpdateTs(pM);
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempMgr-->UpdateTs-->" + ex.Message , ex);
            }

        }
        public bool UpdateByVendor(List<Model.PriceMasterTemp> priceMasterTempList, List<ItemPrice> itemPrices)
        {
            ArrayList sqls = new ArrayList();
            try
            {
                if (priceMasterTempList != null)
                {
                    foreach (var item in priceMasterTempList)
                    {
                        sqls.Add(_priceMasterTempDao.UpdateByVendor(item));
                    }
                }
                //if (itemPrices != null)
                //{
                //    ItemPriceTempMgr itemPriceTempMgr = new ItemPriceTempMgr("");
                //    foreach (var item in itemPrices)
                //    {
                //        sqls.Add(itemPriceTempMgr.Update(item));
                //    }
                //}
                MySqlDao mySqlDao = new MySqlDao(connctionString);
                return mySqlDao.ExcuteSqls(sqls);
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempMgr-->UpdateByVendor-->" + ex.Message + sqls.ToString(), ex);
            }
        }
        #endregion
        #region 查詢組合商品中臨時表中單一商品信息
        public List<Model.Custom.SingleProductPriceTemp> SingleProductPriceQueryByVendor(string product_id, int pile_id)
        {
            try
            {
                return _priceMasterTempDao.SingleProductPriceQueryByVendor(product_id, pile_id);
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempMgr-->SingleProductPriceQueryByVendor-->" + ex.Message, ex);
            }
        }
        #endregion
        public string VendorSaveFromPriceMaster(Model.PriceMasterTemp priceMasterTemp, string old_id)
        {//
            return _priceMasterTempDao.VendorSaveFromPriceMaster(priceMasterTemp, old_id);
        }

        public List<Model.Custom.PriceMasterCustom> QueryProdSiteByVendor(PriceMasterTemp priceMasterTemp)
        {
            try
            {
                return _priceMasterTempDao.QueryProdSiteByVendor(priceMasterTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempMgr-->QueryProdSite-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 管理員核可供應商建立的商品時將商品價格信息由臨時表移動到正式表
        /// </summary>
        /// <param name="priceMasterTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_Delete(PriceMasterTemp priceMasterTemp)
        {
            try
            {
                return _priceMasterTempDao.Vendor_Delete(priceMasterTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempMgr-->Vendor_Delete-->" + ex.Message, ex);
            }
        }
        public string VendorMove2PriceMaster(PriceMasterTemp priceMasterTemp)
        {
            try
            {
                return _priceMasterTempDao.VendorMove2PriceMaster(priceMasterTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("PriceMasterTempMgr-->VendorMove2PriceMaster-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}
