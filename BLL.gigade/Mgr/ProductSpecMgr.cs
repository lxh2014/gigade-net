using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using System.Collections;
using BLL.gigade.Dao;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class ProductSpecMgr : Impl.IProductSpecImplMgr
    {
        private IProductSpecImplDao _specDao;
        string connectionStr;
        private IProductImplMgr _proMgr;
        private BLL.gigade.Dao.MySqlDao _sqlDao; 
        public ProductSpecMgr(string connectionStr)
        {
            _specDao = new BLL.gigade.Dao.ProductSpecDao(connectionStr);
            this.connectionStr = connectionStr;
            _proMgr = new BLL.gigade.Mgr.ProductMgr(connectionStr);
            _sqlDao = new Dao.MySqlDao(connectionStr);
        }

        public List<ProductSpec> query(int product_id, string type)
        {
            try
            {
                return _specDao.query(product_id, type);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecMgr-->query(int product_id,string type)-->" + ex.Message, ex);
            }
        }

        public ProductSpec query(int spec_id)
        {
            try
            {
                return _specDao.query(spec_id);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecMgr-->query(int spec_id)-->" + ex.Message, ex);
            }
        }

        public List<ProductSpec> Query(ProductSpec query)
        {
            try
            {
                return _specDao.Query(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecMgr-->Query(ProductSpec query)-->" + ex.Message, ex);
            }
        }

        public string Update(ProductSpec specModel)
        {
            try
            {
                return _specDao.Update(specModel);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecMgr-->Update-->" + ex.Message, ex);
            }
        }
        public int UpdateSingle(ProductSpec uSpec)
        {
            try
            {
                return _specDao.UpdateSingle(uSpec);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecMgr-->UpdateSingle-->" + ex.Message, ex);
            }
        }

        public string Delete(uint product_Id)
        {
            try
            {
                return _specDao.Delete(product_Id);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecMgr-->Delete-->" + ex.Message, ex);
            }
        }


        public string SaveFromSpec(ProductSpec proSpec)
        {
            return _specDao.SaveFromSpec(proSpec);
        }

        public string UpdateCopySpecId(ProductSpec proSpec)
        {
            return _specDao.UpdateCopySpecId(proSpec);
        }

        public bool Save(List<ProductSpec> saveList)
        {
            try
            {
                bool result = true;
                try
                {
                    foreach (ProductSpec item in saveList)
                    {
                        if (_specDao.Save(item) <= 0)
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
                throw new Exception("ProductSpecMgr-->Save-->" + ex.Message, ex);
            }
        }

        public int Updspecstatus(ProductStockQuery m)
        {
            try
            {
                return _specDao.Updspecstatus(m);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSpecMgr-->Updspecstatus-->" + ex.Message, ex);
            }
        }

    }
}
