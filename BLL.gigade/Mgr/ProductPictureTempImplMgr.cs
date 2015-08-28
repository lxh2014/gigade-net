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
    public class ProductPictureTempImplMgr:IProductPictureTempImplMgr
    {
        private IProductPictureTempImplDao _productPicTempDao;
        private string connStr;
        public ProductPictureTempImplMgr(string connectionStr)
        {
            _productPicTempDao = new ProductPictureTempDao(connectionStr);
            this.connStr = connectionStr;
        }

        public bool Save(List<ProductPictureTemp> PicList, ProductPictureTemp prPictureTemp,int type)
        {
            try
            {
                ArrayList arrList = new ArrayList();
                //先刪除商品說明圖記錄

                arrList.Add(_productPicTempDao.Delete(prPictureTemp, type));

                foreach (var item in PicList)
                {
                    arrList.Add(_productPicTempDao.Save(item, type));
                }

                MySqlDao mySqlDao = new MySqlDao(connStr);
                return mySqlDao.ExcuteSqls(arrList);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductPictureTempImplMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }

        public List<ProductPictureTemp> Query(ProductPictureTemp proPictureTemp, int type)
        {
            try
            {
                return _productPicTempDao.Query(proPictureTemp, type);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductPictureTempImplMgr-->Query-->" + ex.Message, ex);
            }
        }

        public string Delete(ProductPictureTemp proPictureTemp,int type)
        {
            try
            {
                return _productPicTempDao.Delete(proPictureTemp, type);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductPictureTempImplMgr-->Delete-->" + ex.Message, ex);
            }
        }

        public string MoveToProductPicture(ProductPictureTemp proPictureTemp,int type)
        {
            try
            {
                return _productPicTempDao.MoveToProductPicture(proPictureTemp, type);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductPictureTempImplMgr-->MoveToProductPicture-->" + ex.Message, ex);
            }
        }

        public string SaveFromProPicture(ProductPictureTemp proPictureTemp,int type)
        {
            try
            {
                return _productPicTempDao.SaveFromProPicture(proPictureTemp, type);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductPictureTempImplMgr-->SaveFromProPicture-->" + ex.Message, ex);
            }
        }

        public string DeleteByVendor(ProductPictureTemp proPictureTemp)
        {
            try
            {
                return _productPicTempDao.DeleteByVendor(proPictureTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductPictureTempImplMgr-->DeleteByVendor-->" + ex.Message, ex);
            }
        }

       
        public List<ProductPictureTemp> VendorQuery(ProductPictureTemp proPictureTemp)
        {
            try
            {
                return _productPicTempDao.VendorQuery(proPictureTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductPictureTempImplMgr-->VendorQuery-->" + ex.Message, ex);
            }
        }
        public string VendorSaveFromProPicture(ProductPictureTemp proPictureTemp, string old_product_Id)
        {
            try
            {
                return _productPicTempDao.VendorSaveFromProPicture(proPictureTemp, old_product_Id);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductPictureTempImplMgr-->VendorSaveFromProPicture-->" + ex.Message, ex);
            }
        }


        /// <summary>
        /// 管理員核可供應商建立的商品時將商品圖檔信息由臨時表移動到正式表
        /// </summary>
        /// <param name="proPictureTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_MoveToProductPicture(ProductPictureTemp proPictureTemp)
        {
            try
            {
                return _productPicTempDao.Vendor_MoveToProductPicture(proPictureTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductPictureTempImplMgr-->Vendor_MoveToProductPicture-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 管理員核可供應商建立的商品時將商品圖檔信息由臨時表移除
        /// </summary>
        /// <param name="proPictureTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_Delete(ProductPictureTemp proPictureTemp)
        {
            try
            {
                return _productPicTempDao.Vendor_Delete(proPictureTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductPictureTempImplMgr-->Vendor_Delete-->" + ex.Message, ex);
            }
        }
    }
}
