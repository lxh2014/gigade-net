/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ItemPriceTempMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/3/11 11:47:38 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using System.Collections;


namespace BLL.gigade.Mgr
{
    public class ItemPriceTempMgr : IItemPriceTempImplMgr
    {
        private IItemPriceTempImplDao _itemPriceTempDao;
        private string connStr;
        public ItemPriceTempMgr(string connectionStr)
        {
            _itemPriceTempDao = new ItemPriceTempDao(connectionStr);
            this.connStr = connectionStr;
        }

        public List<Model.ItemPrice> itemPriceQuery(Model.ItemPrice query)
        {
            return _itemPriceTempDao.itemPriceQuery(query);
        }
        public string Save(Model.ItemPrice itemPrice)
        {
            return _itemPriceTempDao.Save(itemPrice);
        }
        public string Move2ItemPrice()
        {
            return _itemPriceTempDao.Move2ItemPrice();
        }

        public string SaveFromItemPriceByMasterId()
        {
            return _itemPriceTempDao.SaveFromItemPriceByMasterId();
        }

        public string Update(Model.ItemPrice itemPrice)
        {
            return _itemPriceTempDao.Update(itemPrice);
        }
        public string Delete(string product_Id, int combo_type, int writer_id)
        {
            return _itemPriceTempDao.Delete(product_Id, combo_type, writer_id);
        }

        public string ChildDelete(string product_id, int combo_type, int writer_id)
        {
            return _itemPriceTempDao.ChildDelete(product_id, combo_type, writer_id);
        }

        public string DeleteByVendor(string product_Id, int combo_type, int writer_id)
        {
            return _itemPriceTempDao.DeleteByVendor(product_Id, combo_type, writer_id);
        }

        public List<Model.Custom.ItemPriceCustom> QueryByVendor(Model.ItemPrice itemPrice)
        {
            try
            {
                return _itemPriceTempDao.QueryByVendor(itemPrice);
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceTempMgr.QueryByVendor-->" + ex.Message, ex);
            }
        }

        public string UpdateTs(Model.ItemPrice itemPrice)
        {
            try
            {
                return _itemPriceTempDao.UpdateTs(itemPrice);
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceTempMgr.UpdateTs-->" + ex.Message, ex);
            }
        }
        #region + 獲得供應商新添加的一個商品細項List<Model.Custom.ItemPriceCustom> QueryNewAdd(Model.ItemPrice itemPrice)
        public List<Model.Custom.ItemPriceCustom> QueryNewAdd(Model.ItemPrice itemPrice)
        {
            try
            {
                return _itemPriceTempDao.QueryNewAdd(itemPrice);

            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceTempMgr.QueryNewAdd-->" + ex.Message, ex);
            }
        }
        #endregion
        public bool UpdateByVendor(ArrayList exculsql)
        {
            try
            {
                MySqlDao mySqlDao = new MySqlDao(connStr);
                return mySqlDao.ExcuteSqls(exculsql);
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceTempMgr.UpdateByVendor-->" + ex.Message, ex);
            }

        }

        #region 與供應商商品相關
        /// <summary>
        /// 管理員核可供應商建立的商品時將商品價格信息由臨時表移除
        /// </summary>
        /// <param name="product_Id">商品編號</param>
        /// <param name="combo_type">商品類型</param>
        /// <param name="writerId">操作人</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_Delete(string product_Id, int combo_type, int writerId)
        {
            try
            {
                return _itemPriceTempDao.Vendor_Delete(product_Id, combo_type, writerId);
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceTempMgr-->Vendor_Delete-->" + ex.Message, ex);
            }
        }
        public string VendorMove2ItemPrice()
        {
            return _itemPriceTempDao.VendorMove2ItemPrice();
        }
        #endregion
    }
}
