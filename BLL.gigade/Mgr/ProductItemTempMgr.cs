/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductItemTempMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/28 11:30:30 
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
using BLL.gigade.Model;
namespace BLL.gigade.Mgr
{
    public class ProductItemTempMgr : IProductItemTempImplMgr
    {
        private IProductItemTempImplDao _productItemTempDao;
        private string connStr;
        public ProductItemTempMgr(string connectionStr)
        {
            _productItemTempDao = new ProductItemTempDao(connectionStr);
            this.connStr = connectionStr;
        }

        #region IProductItemTempImplMgr 成员

        public bool Save(List<ProductItemTemp> saveTempList)
        {
            try
            {
                bool result = true;
                if (saveTempList.Count > 0)
                {
                    result = _productItemTempDao.Save(saveTempList) > 0;
                }
                return result;

                //_productItemTempDao.Delete(new ProductItemTemp { Writer_Id = saveTempList[0].Writer_Id });
                //foreach (ProductItemTemp item in saveTempList)
                //{
                //    if (_productItemTempDao.Save(item) <= 0)
                //    {
                //        result = false;
                //    }
                //}

            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }


        public List<Model.ProductItemTemp> Query(Model.ProductItemTemp proItemTemp)
        {
            try
            {
                return _productItemTempDao.Query(proItemTemp);;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->Query-->" + ex.Message, ex);
            }
        }

        public bool UpdateCostMoney(List<Model.ProductItemTemp> productItemTemp)
        {
            try
            {
                ArrayList sqls = new ArrayList();
                foreach (var item in productItemTemp)
                {
                    sqls.Add(_productItemTempDao.UpdateCostMoney(item));
                }
                MySqlDao mySqlDao = new MySqlDao(connStr);
                return mySqlDao.ExcuteSqls(sqls);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr.UpdateCostMoney-->" + ex.Message, ex);
            }
        }

        public bool UpdateStockAlarm(List<Model.ProductItemTemp> productItemTemp)
        {
            try
            {
                ArrayList sqls = new ArrayList();
                foreach (var item in productItemTemp)
                {
                    sqls.Add(_productItemTempDao.UpdateStockAlarm(item));
                }
                MySqlDao mySqlDao = new MySqlDao(connStr);
                return mySqlDao.ExcuteSqls(sqls);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr.UpdateStockAlarm-->" + ex.Message, ex);
            }
        }

        public bool UpdateItemStock(Model.ProductItemTemp productItemTemp)
        {
            try
            {
                return _productItemTempDao.UpdateItemStock(productItemTemp) >= 0;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr.UpdateItemStock-->" + ex.Message, ex);
            }
        }


        public string QueryStock(Model.ProductItemTemp proItemTemp)
        {
            try
            {
                StringBuilder stb = new StringBuilder();
                List<Model.ProductItemTemp> result = _productItemTempDao.Query(proItemTemp);
                if (result.Count == 0)
                {
                    return "{success:true,items:[]}";
                }
                else
                {
                    stb.Append("{success:true,items:[");
                    int defaultArriveDays = new ProductTempMgr(connStr).GetDefaultArriveDays(new ProductTemp { Writer_Id = proItemTemp.Writer_Id, Product_Id = proItemTemp.Product_Id });
                    foreach (var item in result)
                    {
                        item.Arrive_Days += defaultArriveDays;
                        stb.Append("{");
                        stb.AppendFormat("\"spec_title_1\":\"{0}\",\"spec_title_2\":\"{1}\",\"item_stock\":\"{2}\",\"item_alarm\":\"{3}\",\"barcode\":\"{4}\",\"spec_id_1\":\"{5}\",\"spec_id_2\":\"{6}\",\"item_id\":\"{7}\",\"erp_id\":\"{8}\",\"remark\":\"{9}\",\"arrive_days\":\"{10}\",\"default_arrive_days\":\"{11}\"", item.Spec_Name_1, item.Spec_Name_2, item.Item_Stock, item.Item_Alarm, item.Barcode, item.Spec_Id_1, item.Spec_Id_2, item.Item_Id, item.Erp_Id, item.Remark, item.Arrive_Days, defaultArriveDays);//edit by xiangwang0413w 2014/06/18 (增加ERP廠商編號erp_id)  // add by zhuoqin0830w 2014/02/05 增加備註  //add by zhuoqin0830w 2014/03/20 增加運達天數
                        stb.Append("}");
                    }
                    stb.Append("]}");
                    return stb.ToString().Replace("}{", "},{");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->QueryStock-->" + ex.Message, ex);
            }
        }

        public bool Delete(Model.ProductItemTemp delTemp)
        {
            try
            {
                return _productItemTempDao.Delete(delTemp) <= 0 ? false : true;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->Delete-->" + ex.Message, ex);
            }
        }

        public string MoveProductItem(Model.ProductItemTemp proItemTemp)
        {
            try
            {
                return _productItemTempDao.MoveProductItem(proItemTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->MoveProductItem-->" + ex.Message, ex);
            }
        }

        public string DeleteSql(Model.ProductItemTemp proItemTemp)
        {
            try
            {
                return _productItemTempDao.DeleteSql(proItemTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->DeleteSql-->" + ex.Message, ex);
            }
        }
        public string DeleteVendorSql(Model.ProductItemTemp proItemTemp)
        {
            try
            {
                return _productItemTempDao.DeleteVendorSql(proItemTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->DeleteVendorSql-->" + ex.Message, ex);
            }

        }
        public string QuerySql(Model.ProductItemTemp proItemTemp)
        {
            try
            {
                return _productItemTempDao.QuerySql(proItemTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->QuerySql-->" + ex.Message, ex);
            }
        }

        public string SaveFromProItem(Model.ProductItemTemp proItemTemp)
        {
            try
            {
                return _productItemTempDao.SaveFromProItem(proItemTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->SaveFromProItem-->" + ex.Message, ex);
            }
        }

        public string UpdateCopySpecId(ProductItemTemp proItemTemp)
        {
            try
            {
                return _productItemTempDao.UpdateCopySpecId(proItemTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->UpdateCopySpecId-->" + ex.Message, ex);
            }
        }

        #region 供應商修改
        public List<Model.ProductItemTemp> QueryByVendor(Model.ProductItemTemp proItemTemp)
        {
            try
            {
                return _productItemTempDao.QueryByVendor(proItemTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->QueryByVendor-->" + ex.Message, ex);
            }
        }
        public bool SaveByVendor(List<Model.ProductItemTemp> saveTempList)
        {
            try
            {
                bool result = true;
                try
                {
                    foreach (ProductItemTemp item in saveTempList)
                    {
                        if (_productItemTempDao.SaveByVendor(item) <= 0)
                        {
                            result = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                    throw ex;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->SaveByVendor-->" + ex.Message, ex);
            }
        }

        public string UpdateByVendor(Model.ProductItemTemp item)
        {
            try
            {
                return _productItemTempDao.UpdateByVendor(item) ;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->UpdateByVendor-->" + ex.Message, ex);
            }
        }

        public bool DeleteByVendor(Model.ProductItemTemp delTemp)
        {
            try
            {
                return _productItemTempDao.DeleteByVendor(delTemp) <= 0 ? false : true;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->DeleteByVendor-->" + ex.Message, ex);
            }
        }

        public string QueryStockByVendor(Model.ProductItemTemp proItemTemp)
        {
            try
            {
                StringBuilder stb = new StringBuilder();
                List<Model.ProductItemTemp> result = _productItemTempDao.QueryByVendor(proItemTemp);
                if (result.Count == 0)
                {
                    return "{success:true,items:[]}";
                }
                else
                {
                    stb.Append("{success:true,items:[");
                    foreach (var item in result)
                    {
                        stb.Append("{");
                        stb.AppendFormat("\"spec_title_1\":\"{0}\",\"spec_title_2\":\"{1}\",\"item_stock\":\"{2}\",\"item_alarm\":\"{3}\",\"barcode\":\"{4}\",\"spec_id_1\":\"{5}\",\"spec_id_2\":\"{6}\",\"item_id\":\"{7}\",\"erp_id\":\"{8}\"", item.Spec_Name_1, item.Spec_Name_2, item.Item_Stock, item.Item_Alarm, item.Barcode, item.Spec_Id_1, item.Spec_Id_2, item.Item_Id, item.Erp_Id);//edit by xiangwang0413w 2014/6/18(增加ERP廠商編號erp_id)
                        stb.Append("}");
                    }
                    stb.Append("]}");
                    return stb.ToString().Replace("}{", "},{");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->QueryStockByVendor-->" + ex.Message, ex);
            }
        }
        public bool UpdateStockAlarmByVendor(List<Model.ProductItemTemp> productItemTemp)
        {
            try
            {
                ArrayList sqls = new ArrayList();
                foreach (var item in productItemTemp)
                {
                    sqls.Add(_productItemTempDao.UpdateStockAlarmByVendor(item));
                }
                MySqlDao mySqlDao = new MySqlDao(connStr);
                return mySqlDao.ExcuteSqls(sqls);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr.UpdateStockAlarmByVendor-->" + ex.Message, ex);
            }
        }
        public string VendorSaveFromProItem(Model.ProductItemTemp proItemTemp,string old_id)
        {
            try
            {
                return _productItemTempDao.VendorSaveFromProItem(proItemTemp,old_id);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->VendorSaveFromProItem-->" + ex.Message, ex);
            }
        }
        public string VendorUpdateCopySpecId(ProductItemTemp proItemTemp)
        {
            try
            {
                return _productItemTempDao.VendorUpdateCopySpecId(proItemTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->VendorUpdateCopySpecId-->" + ex.Message, ex);
            }
        }
        public string VendorQuerySql(Model.ProductItemTemp proItemTemp)
        {
            try
            {
                return _productItemTempDao.VendorQuerySql(proItemTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->VendorQuerySql-->" + ex.Message, ex);
            }
        }
        public string VendorMoveProductItem(Model.ProductItemTemp proItemTemp)
        {
            try
            {
                return _productItemTempDao.VendorMoveProductItem(proItemTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemTempMgr-->VendorMoveProductItem-->" + ex.Message, ex);
            }
        }
        #endregion
        #endregion
    }
}
