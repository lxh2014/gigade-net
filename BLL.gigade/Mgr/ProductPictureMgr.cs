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
    public  class ProductPictureMgr : IProductPictureImplMgr
    {
        private IProductPictureImplDao _productPicDao;
        private string connStr;
        public ProductPictureMgr(string connectionStr)
        {
            _productPicDao = new ProductPictureDao(connectionStr);
            this.connStr = connectionStr;
        }

        public bool Save(List<ProductPicture> PicList,int type=1)
        {
            try
            {
                ArrayList arrList = new ArrayList();
                //先刪除商品說明圖記錄
                arrList.Add(_productPicDao.Delete(PicList[0].product_id, type));
                foreach (var item in PicList)
                {
                    arrList.Add(_productPicDao.Save(item, type));
                }
                MySqlDao mySqlDao = new MySqlDao(connStr);
                return mySqlDao.ExcuteSqls(arrList);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductPictureMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }

        public List<ProductPicture> Query(int product_id,int type=1)
        {
            try
            {
                return _productPicDao.Query(product_id, type);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductPictureMgr-->Query-->" + ex.Message, ex);
            }
        }

        public string Delete(int product_id,int type=1)
        {
            try
            {
                return _productPicDao.Delete(product_id,type);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductPictureMgr-->Delete-->" + ex.Message, ex);
            }
        }

        public string SaveFromOtherPro(ProductPicture productPicture)
        {
            return _productPicDao.SaveFromOtherPro(productPicture);
        }
    }
}
