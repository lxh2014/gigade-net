using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
     interface IVendorBrandSetImplDao
    {
         List<VendorBrandSet> Query(VendorBrandSet vbs);
         List<VendorBrandSet> GetClassId(VendorBrandSet vbs);
         DataTable GetShop(string id);
         string GetShopName(uint id);
         List<VendorBrandSetQuery> GetVendorBrandList(VendorBrandSetQuery store, out int totalCount);
         int Save(VendorBrandSetQuery model);
         int Update(VendorBrandSetQuery model);
         int GetBrandId();
         VendorBrandSetQuery GetModelById(int id);
         VendorBrandSetQuery GetSingleImage(string name);
         int DeleteImage(string imageName);
         int UpdateImage(VendorBrandSetQuery query);
         void SaveBrandStory(VendorBrandSetQuery query);
         List<VendorBrandSetQuery> GetImageInfo(VendorBrandSetQuery store);
         bool GetSortIsRepeat(VendorBrandSetQuery query);
    }
}
