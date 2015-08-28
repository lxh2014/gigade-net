using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade;
using System.Collections;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface IPriceMasterTempImplDao
    {
        string Save(Model.PriceMasterTemp priceMaster);
        string Update(Model.PriceMasterTemp priceMaster);
        Model.Custom.PriceMasterProductCustom Query(Model.PriceMasterTemp priceMasterTemp);
        string Delete(Model.PriceMasterTemp priceMasterTemp);
        string Move2PriceMaster(Model.PriceMasterTemp priceMasterTemp);
        string Move2PriceMasterByMasterId();
        string SelectChild(Model.PriceMasterTemp priceMasterTemp);
        List<Model.PriceMasterTemp> queryChild(Model.PriceMasterTemp query);
        string SaveFromPriceMasterByMasterId(Model.PriceMasterTemp priceMasterTemp);
        string SaveFromPriceMaster(Model.PriceMasterTemp priceMasterTemp);

        bool Save(ArrayList priceMasters, ArrayList itemPriceSqls, ArrayList otherSqls);
        /// <summary>
        /// 刪除子商品的價格數據
        /// </summary>
        /// <param name="priceMasterTemp"></param>
        /// <returns></returns>
        string ChildDelete(Model.PriceMasterTemp priceMasterTemp);

        string DeleteByVendor(PriceMasterTemp priceMasterTemp); Model.Custom.PriceMasterProductCustomTemp QueryByVendor(PriceMasterTemp priceMasterTemp);
        string SaveByVendor(PriceMasterTemp priceMaster);
        string UpdateByVendor(PriceMasterTemp priceMaster);
        List<Model.Custom.SingleProductPriceTemp> SingleProductPriceQueryByVendor(string product_id, int pile_id);
        string VendorSaveFromPriceMaster(PriceMasterTemp priceMasterTemp, string old_product_id);
        List<Model.Custom.PriceMasterCustom> QueryProdSiteByVendor(PriceMasterTemp priceMasterTemp);
        string UpdateTs(Model.Custom.PriceMasterCustom pM);


        #region 與供應商商品相關
        string Vendor_Delete(PriceMasterTemp priceMasterTemp); 
        string VendorMove2PriceMaster(PriceMasterTemp priceMasterTemp);
        #endregion
    }
}
