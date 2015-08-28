using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System.Collections;
using BLL.gigade.Dao;
namespace BLL.gigade.Mgr
{
    public class ProductComboTempMgr : BLL.gigade.Mgr.Impl.IProductComboTempImplMgr
    {
        private IProductComboTempImplDao _tempDao;
        private MySqlDao _mySql;
        public ProductComboTempMgr(string connectionString)
        {
            _tempDao = new BLL.gigade.Dao.ProductComboTempDao(connectionString);
            _mySql = new MySqlDao(connectionString);
        }

        public bool Save(List<ProductComboTemp> saveTempList)
        {
            bool result = true;
            try
            {
                _tempDao.Delete(saveTempList[0]);
                foreach (ProductComboTemp item in saveTempList)
                {
                    if (_tempDao.Save(item) <= 0)
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                _tempDao.Delete(saveTempList[0]);
                throw new Exception("ProductComboTempMgr.SingleCompareSave-->" + ex.Message, ex);
            }

            try
            {
                if (!result)
                {
                    _tempDao.Delete(saveTempList[0]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.SingleCompareSave-->" + ex.Message, ex);
            }

            return result;

        }

        /// <summary>
        /// 刪除組合商品的規格和價格數據
        /// </summary>
        /// <param name="delTemp"></param>
        /// <returns></returns>
        public bool Delete(ProductComboTemp delTemp)
        {

            try
            {
                ArrayList sqls = new ArrayList();
                sqls.Add(_tempDao.TempDelete(delTemp));

                //刪除item_price_temp中的數據
                ItemPriceTempMgr itemPriceMgr = new ItemPriceTempMgr("");
                sqls.Add(itemPriceMgr.Delete(delTemp.Parent_Id, delTemp.Combo_Type, delTemp.Writer_Id));

                //刪除price_master_temp中的數據
                PriceMasterTempMgr priceMasterTempMgr = new PriceMasterTempMgr("");
                sqls.Add(priceMasterTempMgr.Delete(new PriceMasterTemp { writer_Id = delTemp.Writer_Id, product_id = delTemp.Parent_Id, combo_type = delTemp.Combo_Type }));

                return _mySql.ExcuteSqls(sqls);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.Delete-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 刪除組合商品的所有價格數據
        /// </summary>
        /// <param name="delTemp"></param>
        /// <returns></returns>
        public bool comboPriceDelete(ProductComboTemp delTemp)
        {
            try
            {
                ArrayList sqls = new ArrayList();

                //刪除item_price_temp中的數據
                ItemPriceTempMgr itemPriceMgr = new ItemPriceTempMgr("");
                sqls.Add(itemPriceMgr.Delete(delTemp.Parent_Id, delTemp.Combo_Type, delTemp.Writer_Id));

                //刪除price_master_temp中的數據
                PriceMasterTempMgr priceMasterTempMgr = new PriceMasterTempMgr("");
                sqls.Add(priceMasterTempMgr.Delete(new PriceMasterTemp { writer_Id = delTemp.Writer_Id, product_id = delTemp.Parent_Id, combo_type = delTemp.Combo_Type }));

                return _mySql.ExcuteSqls(sqls);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.Delete-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 刪除子商品對應價格數據
        /// </summary>
        /// <returns></returns>
        public bool comboChildPriceDel(ProductComboTemp delTemp)
        {
            ArrayList sqls = new ArrayList();
            ItemPriceTempMgr itemPriceMgr = new ItemPriceTempMgr("");
            sqls.Add(itemPriceMgr.ChildDelete(delTemp.Parent_Id, delTemp.Combo_Type, delTemp.Writer_Id));

            PriceMasterTempMgr priceMasterMgr = new PriceMasterTempMgr("");
            sqls.Add(priceMasterMgr.ChildDelete(new PriceMasterTemp { writer_Id = delTemp.Writer_Id, product_id = delTemp.Parent_Id.ToString(), combo_type = delTemp.Combo_Type }));
            return _mySql.ExcuteSqls(sqls);

        }

        public List<ProductComboCustom> combQuery(ProductComboCustom query)
        {
            try
            {
                return _tempDao.combQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.combQuery-->" + ex.Message, ex);
            }
        }

        public List<ProductComboCustom> comboPriceQuery(ProductComboCustom query)
        {
            try
            {
                return _tempDao.comboPriceQuery(query);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductComboTempMgr.comboPriceQuery-->" + ex.Message, ex);
            }
        }

        public List<ProductComboCustom> priceComboQuery(ProductComboCustom query)
        {
            try
            {
                return _tempDao.priceComboQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.priceComboQuery-->" + ex.Message, ex);
            }
        }

        public List<MakePriceCustom> differentSpecQuery(ProductComboCustom query)
        {
            try
            {
                return _tempDao.differentSpecQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.differentSpecQuery-->" + ex.Message, ex);
            }
        }

        public List<ProductComboTemp> groupNumQuery(ProductComboTemp query)
        {

            try
            {
                return _tempDao.groupNumQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.groupNumQuery-->" + ex.Message, ex);
            }
        }

        public string TempMoveCombo(ProductComboTemp query)
        {

            try
            {
                return _tempDao.TempMoveCombo(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.TempMoveCombo-->" + ex.Message, ex);
            }
        }

        public string TempDelete(ProductComboTemp query)
        {
            try
            {
                return _tempDao.TempDelete(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.TempDelete-->" + ex.Message, ex);
            }
        }

        public string SaveFromCombo(ProductComboTemp proComboTemp)
        {

            try
            {
                return _tempDao.SaveFromCombo(proComboTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.SaveFromCombo-->" + ex.Message, ex);
            }
        }


        #region 供應商商品處理

        public string TempDeleteByVendor(ProductComboTemp query)
        {

            try
            {
                return _tempDao.TempDeleteByVendor(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.TempDeleteByVendor-->" + ex.Message, ex);
            }
        }

        #region 刪除組合商品的規格和價格數據 product_combo，item_price_master price_master_temp +bool DeleteByVendor(ProductComboTemp delTemp)
        /// <summary>
        /// 刪除組合商品的規格和價格數據
        /// </summary>
        /// <param name="delTemp"></param>
        /// <returns></returns>
        public bool DeleteByVendor(ProductComboTemp delTemp)
        {
            ArrayList sqls = new ArrayList();
            try
            {
                sqls.Add(_tempDao.TempDeleteByVendor(delTemp));

                //刪除item_price_temp中的數據
                ItemPriceTempMgr itemPriceMgr = new ItemPriceTempMgr("");
                sqls.Add(itemPriceMgr.DeleteByVendor(delTemp.Parent_Id, delTemp.Combo_Type, delTemp.Writer_Id));

                //刪除price_master_temp中的數據
                PriceMasterTempMgr priceMasterTempMgr = new PriceMasterTempMgr("");
                sqls.Add(priceMasterTempMgr.DeleteByVendor(new PriceMasterTemp { writer_Id = delTemp.Writer_Id, product_id = delTemp.Parent_Id, combo_type = delTemp.Combo_Type }));

                return _mySql.ExcuteSqls(sqls);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.DeleteByVendor-->" + ex.Message + sqls.ToString(), ex);
            }
        }
        #endregion

        public List<ProductComboCustomVendor> combQueryByVendor(ProductComboCustomVendor query)
        {
            try
            {
                return _tempDao.combQueryByVendor(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.combQueryByVendor-->" + ex.Message, ex);
            }
        }

        #region 處理組合商品保存 product_combo_temp +bool SaveByVendor(List<ProductComboTemp> saveTempList)
        public bool SaveByVendor(List<ProductComboTemp> saveTempList)
        {
            bool result = true;
            try
            {
                _tempDao.DeleteByVendor(saveTempList[0]);
                foreach (ProductComboTemp item in saveTempList)
                {
                    if (_tempDao.SaveByVendor(item) <= 0)
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                _tempDao.DeleteByVendor(saveTempList[0]);
                throw new Exception("ProductComboTempMgr.SaveByVendor-->" + ex.Message, ex);
            }

            try
            {
                if (!result)
                {
                    _tempDao.DeleteByVendor(saveTempList[0]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.SaveByVendor-->" + ex.Message, ex);
            }

            return result;

        }
        #endregion

        #region 獲取群組組合商品信息 +List<ProductComboTemp> groupNumQueryByVendor(ProductComboTemp query)
        public List<ProductComboTemp> groupNumQueryByVendor(ProductComboTemp query)
        {

            try
            {
                return _tempDao.groupNumQueryByVendor(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.groupNumQueryByVendor-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 刪除組合商品的父商品和子商品價格數據 item_price_master price_master_temp +bool comboPriceDeleteByVendor(ProductComboTemp delTemp)
        /// <summary>
        /// 刪除組合商品的父商品和子商品價格數據
        /// </summary>
        /// <param name="delTemp"></param>
        /// <returns></returns>
        public bool comboPriceDeleteByVendor(ProductComboTemp delTemp)
        {
            ArrayList sqls = new ArrayList();
            try
            {
                //刪除item_price_temp子商品的數據中的數據
                ItemPriceTempMgr itemPriceMgr = new ItemPriceTempMgr("");
                sqls.Add(itemPriceMgr.DeleteByVendor(delTemp.Parent_Id, delTemp.Combo_Type, delTemp.Writer_Id));

                //刪除price_master_temp中的數據
                PriceMasterTempMgr priceMasterTempMgr = new PriceMasterTempMgr("");
                sqls.Add(priceMasterTempMgr.DeleteByVendor(new PriceMasterTemp { writer_Id = delTemp.Writer_Id, product_id = delTemp.Parent_Id, combo_type = delTemp.Combo_Type }));

                return _mySql.ExcuteSqls(sqls);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.comboPriceDeleteByVendor-->" + ex.Message + sqls.ToString(), ex);
            }
        }
        #endregion

        public string VendorSaveFromCombo(ProductComboTemp proComboTemp, string old_id)
        {//20140905 複製供應商

            try
            {
                return _tempDao.VendorSaveFromCombo(proComboTemp, old_id);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.VendorSaveFromCombo-->" + ex.Message, ex);
            }
        }

        public List<ProductTemp> QueryChildStatusVendor(ProductComboTemp query)
        {
            try
            {
                return _tempDao.QueryChildStatusVendor(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.QueryChildStatusVendor-->" + ex.Message, ex);
            }
        }

        #region 供應商商品審核時將商品臨時表中的組合商品數據添加到琥正式表中
        /// <summary>
        /// 供應商商品審核時將商品臨時表中的組合商品數據添加到琥正式表中
        /// </summary>
        /// <param name="proComboTemp">組合商品對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_TempMoveCombo(ProductComboTemp proComboTemp)
        {
            try
            {
                return _tempDao.Vendor_TempMoveCombo(proComboTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboTempMgr.Vendor_TempMoveCombo-->" + ex.Message, ex);
            }
        }
        #endregion
        #endregion
    }
}
