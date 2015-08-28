using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Mgr.Impl
{
   public  interface IVendorBrandSetImplMgr
    {
        List<VendorBrandSet> Query(VendorBrandSet vbs);
        List<VendorBrandSet> GetClassId(VendorBrandSet vbs);

        List<VendorBrandSetQuery> GetVendorBrandList(VendorBrandSetQuery store, out int totalCount);
        DataTable GetShop(string id);
        string GetShopName(uint id);
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
