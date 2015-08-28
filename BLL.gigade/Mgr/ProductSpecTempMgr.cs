using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using System.Collections;
using BLL.gigade.Dao;
namespace BLL.gigade.Mgr
{
    public class ProductSpecTempMgr : IProductSpecTempImplMgr
    {
        private IProductSpecTempImplDao _tempDao;
        private string connStr;
        public ProductSpecTempMgr(string conStr)
        {
            _tempDao = new BLL.gigade.Dao.ProductSpecTempDao(conStr);
            this.connStr = conStr;
        }
        public bool Save(List<ProductSpecTemp> saveTempList)
        {
            try
            {
                bool result = true;
                try
                {
                    foreach (ProductSpecTemp item in saveTempList)
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
                    throw ex;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecTempMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }

        public List<ProductSpecTemp> Query(ProductSpecTemp queryTemp)
        {
            try
            {
                return _tempDao.Query(queryTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecTempMgr-->Query-->" + ex.Message, ex);
            }
        }

        public bool Delete(ProductSpecTemp delTemp)
        {
            try
            {
                return _tempDao.Delete(delTemp) <= 0 ? false : true;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecTempMgr-->Delete-->" + ex.Message, ex);
            }
        }

        public string QuerySpecOne(ProductSpecTemp psTemp)
        {
            try
            {
                string json = string.Empty;
                json += "{success:true,items:[";
                List<ProductSpecTemp> results = _tempDao.Query(psTemp);
                foreach (var item in results)
                {
                    json += "{\"spec_name\":\"" + item.spec_name + "\"}";
                }
                json += "]}";
                json = json.Replace("}{", "},{");
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecTempMgr-->QuerySpecOne-->" + ex.Message, ex);
            }
        }

        public bool Update(List<ProductSpecTemp> pSpecList, string updateType)
        {
            try
            {
                ArrayList sqls = new ArrayList();
                foreach (var item in pSpecList)
                {
                    sqls.Add(_tempDao.Update(item, updateType));
                }
                MySqlDao mySqlDao = new MySqlDao(connStr);
                return mySqlDao.ExcuteSqls(sqls);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecTempMgr-->Update-->" + ex.Message, ex);
            }
        }

        public string TempMoveSpec(ProductSpecTemp proSpecTemp)
        {
            try
            {
                return _tempDao.TempMoveSpec(proSpecTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecTempMgr-->TempMoveSpec-->" + ex.Message, ex);
            }
        }
        public string TempDelete(ProductSpecTemp proSpecTemp)
        {
            try
            {
                return _tempDao.TempDelete(proSpecTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecTempMgr-->TempDelete-->" + ex.Message, ex);
            }
        }

        public string SaveFromSpec(ProductSpecTemp proSpecTemp)
        {
            try
            {
                return _tempDao.SaveFromSpec(proSpecTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecTempMgr-->SaveFromSpec-->" + ex.Message, ex);
            }
        }

        public string UpdateCopySpecId(ProductSpecTemp proSpecTemp)
        {
            try
            {
                return _tempDao.UpdateCopySpecId(proSpecTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecTempMgr-->UpdateCopySpecId-->" + ex.Message, ex);
            }
        }


        #region 供應商修改
        public string TempDeleteByVendor(ProductSpecTemp proSpecTemp)
        {
            try
            {
                return _tempDao.TempDeleteByVendor(proSpecTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecTempMgr-->TempDeleteByVendor-->" + ex.Message, ex);
            }
        }

        public bool SaveByVendor(List<ProductSpecTemp> saveTempList)
        {
            try
            {
                bool result = true;
                try
                {
                    foreach (ProductSpecTemp item in saveTempList)
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
                    throw ex;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecTempMgr-->SaveByVendor-->" + ex.Message, ex);
            }
        }

        public bool DeleteByVendor(ProductSpecTemp delTemp)
        {
            try
            {
                return _tempDao.DeleteByVendor(delTemp) <= 0 ? false : true;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecTempMgr-->DeleteByVendor-->" + ex.Message, ex);
            }
        }
        public string VendorSaveFromSpec(ProductSpecTemp proSpecTemp, string old_id)
        {
            try
            {
                return _tempDao.VendorSaveFromSpec(proSpecTemp, old_id);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecTempMgr-->VendorSaveFromSpec-->" + ex.Message, ex);
            }
        }
  
        public string VendorTempMoveSpec(ProductSpecTemp proSpecTemp)
        {
            try
            {
                return _tempDao.VendorTempMoveSpec(proSpecTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecTempMgr-->VendorTempMoveSpec-->" + ex.Message, ex);
            }
        }
        public string VendorTempDelete(ProductSpecTemp proSpecTemp)
        {
            try
            {
                return _tempDao.VendorTempDelete(proSpecTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecTempMgr-->VendorTempDelete-->" + ex.Message, ex);
            }
        }
        public List<ProductSpecTemp> VendorQuery(ProductSpecTemp queryTemp)
        {
            try
            {
                return _tempDao.VendorQuery(queryTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecTempMgr-->VendorQuery-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}
