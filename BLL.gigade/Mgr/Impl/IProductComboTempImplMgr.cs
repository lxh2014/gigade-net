using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductComboTempImplMgr
    {
        bool Save(List<ProductComboTemp> saveTempList);
        bool Delete(ProductComboTemp delTemp);
        bool comboPriceDelete(ProductComboTemp delTemp);
        List<ProductComboCustom> combQuery(ProductComboCustom query);
        List<ProductComboTemp> groupNumQuery(ProductComboTemp query);
        string TempMoveCombo(ProductComboTemp query);
        string TempDelete(ProductComboTemp query);
        string SaveFromCombo(ProductComboTemp proComboTemp);
        List<MakePriceCustom> differentSpecQuery(ProductComboCustom query);
        List<ProductComboCustom> comboPriceQuery(ProductComboCustom query);
        List<ProductComboCustom> priceComboQuery(ProductComboCustom query);

        string TempDeleteByVendor(ProductComboTemp query);
        bool DeleteByVendor(ProductComboTemp delTemp);
        List<ProductComboCustomVendor> combQueryByVendor(ProductComboCustomVendor query);
        bool SaveByVendor(List<ProductComboTemp> saveTempList);
        List<ProductComboTemp> groupNumQueryByVendor(ProductComboTemp query);
        bool comboPriceDeleteByVendor(ProductComboTemp delTemp);
        string VendorSaveFromCombo(ProductComboTemp proComboTemp, string old_id);
        List<ProductTemp> QueryChildStatusVendor(ProductComboTemp query);
    }
}
