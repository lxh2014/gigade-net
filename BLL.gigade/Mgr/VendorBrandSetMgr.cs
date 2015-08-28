using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Dao;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class VendorBrandSetMgr : IVendorBrandSetImplMgr
    {
        private Dao.Impl.IVendorBrandSetImplDao _vendorBrandSetDao;

        public VendorBrandSetMgr(string connectionString)
        {
            _vendorBrandSetDao = new VendorBrandSetDao(connectionString);
        }

        public List<Model.VendorBrandSet> Query(VendorBrandSet vbs)
        {
            return _vendorBrandSetDao.Query(vbs);
        }
        public List<VendorBrandSet> GetClassId(VendorBrandSet vbs)
        {
           return _vendorBrandSetDao.GetClassId(vbs);
        }
        public List<VendorBrandSetQuery> GetVendorBrandList(VendorBrandSetQuery store, out int totalCount)
        {
            return _vendorBrandSetDao.GetVendorBrandList(store, out totalCount);
        }
        public DataTable GetShop(string id)/*返回品牌*/
        {
            return _vendorBrandSetDao.GetShop(id);
        }
        public string GetShopName(uint id)
        {
            return _vendorBrandSetDao.GetShopName(id);
        }
        public int Save(VendorBrandSetQuery model)
        {
            return _vendorBrandSetDao.Save(model);
        }
        public int Update(VendorBrandSetQuery model)
        {
            return _vendorBrandSetDao.Update(model);
        }
        public int GetBrandId()
        {
            return _vendorBrandSetDao.GetBrandId();
        }
        public VendorBrandSetQuery GetModelById(int id)
        {
            return _vendorBrandSetDao.GetModelById(id);
        }



        public VendorBrandSetQuery GetSingleImage(string name)
        {
            try
            {
                return _vendorBrandSetDao.GetSingleImage(name);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandSetMgr-->GetSingleImage-->" + ex.Message, ex);
            }
        }


        public int DeleteImage(string imageName)
        {
            try
            {
                return _vendorBrandSetDao.DeleteImage(imageName);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandSetMgr-->DeleteImage-->" + ex.Message, ex);
            }
        }

        public int UpdateImage(VendorBrandSetQuery query)
        {
            try
            {
                return _vendorBrandSetDao.UpdateImage(query);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandSetMgr-->UpdateImage-->"+ex.Message,ex);
            }
        }

        public void SaveBrandStory(VendorBrandSetQuery query)
        {
            try
            {
                _vendorBrandSetDao.SaveBrandStory(query);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandSetMgr-->SaveBrandStory-->" + ex.Message, ex);
            }
        }

        public List<VendorBrandSetQuery> GetImageInfo(VendorBrandSetQuery store)
        {
            try
            {
                return _vendorBrandSetDao.GetImageInfo(store);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandSetMgr-->GetImageInfo-->" + ex.Message, ex);
            }
        }
        public bool GetSortIsRepeat(VendorBrandSetQuery query)
        {
            try
            {
                return _vendorBrandSetDao.GetSortIsRepeat(query);
            }
            catch (Exception ex)
            {
                throw new Exception("VendorBrandSetMgr-->GetSortIsRepeat-->" + ex.Message, ex);
            }
        }
    }
}
