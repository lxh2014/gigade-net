using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IPriceMasterTempImplMgr
    {
        bool Save(List<Model.PriceMasterTemp> priceMasterTempList, List<List<ItemPrice>> itemPrices, ArrayList others);
        bool Update(List<Model.PriceMasterTemp> priceMasterTempList, List<ItemPrice> itemPrices);
        Model.Custom.PriceMasterProductCustom Query(Model.PriceMasterTemp priceMasterTemp);
        string Delete(Model.PriceMasterTemp priceMasterTemp);
        string SelectChild(Model.PriceMasterTemp priceMasterTemp);
        List<PriceMasterTemp> queryChild(Model.PriceMasterTemp query);
        string Move2PriceMaster(Model.PriceMasterTemp priceMasterTemp);
        string Move2PriceMasterByMasterId();
        string SaveFromPriceMaster(Model.PriceMasterTemp priceMasterTemp);
        string SaveFromPriceMasterByMasterId(PriceMasterTemp priceMasterTemp);
        /// <summary>
        /// 刪除子商品的價格數據
        /// </summary>
        /// <param name="priceMasterTemp"></param>
        /// <returns></returns>
        string ChildDelete(PriceMasterTemp priceMasterTemp);


        string DeleteByVendor(PriceMasterTemp priceMasterTemp);
        Model.Custom.PriceMasterProductCustomTemp QueryByVendor(PriceMasterTemp priceMasterTemp);
        bool SaveByVendor(List<Model.PriceMasterTemp> priceMasterTempList, List<List<ItemPrice>> itemPrices, ArrayList others);
        bool UpdateByVendor(List<Model.PriceMasterTemp> priceMasterTempList, List<ItemPrice> itemPrices);
        List<Model.Custom.SingleProductPriceTemp> SingleProductPriceQueryByVendor(string product_id, int pile_id);
        List<Model.Custom.PriceMasterCustom> QueryProdSiteByVendor(PriceMasterTemp priceMasterTemp);
        string UpdateTs(Model.Custom.PriceMasterCustom pM);
        string VendorSaveFromPriceMaster(Model.PriceMasterTemp priceMasterTemp, string old_id);
        string VendorMove2PriceMaster(PriceMasterTemp priceMasterTemp);
    }
}
