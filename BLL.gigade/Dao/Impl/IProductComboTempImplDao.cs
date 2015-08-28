using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao.Impl
{
    interface IProductComboTempImplDao
    {
        int Save(ProductComboTemp saveTemp);
        int Delete(ProductComboTemp delTemp);
        List<ProductComboCustom> combQuery(ProductComboCustom query);
        List<ProductComboTemp> groupNumQuery(ProductComboTemp query);
        string TempMoveCombo(ProductComboTemp query);
        string TempDelete(ProductComboTemp query);
        string SaveFromCombo(ProductComboTemp proComboTemp);
        List<MakePriceCustom> differentSpecQuery(ProductComboCustom query);
        /// <summary>
        /// 查詢組合商品規格對應的價格
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<ProductComboCustom> comboPriceQuery(ProductComboCustom query);
        /// <summary>
        /// 查詢組合商品價格對應規格的pile_id
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<ProductComboCustom> priceComboQuery(ProductComboCustom query);

        string TempDeleteByVendor(ProductComboTemp query);
        List<ProductComboCustomVendor> combQueryByVendor(ProductComboCustomVendor query);
        int DeleteByVendor(ProductComboTemp delTemp);
        int SaveByVendor(ProductComboTemp saveTemp);
        List<ProductComboTemp> groupNumQueryByVendor(ProductComboTemp query);
        string VendorSaveFromCombo(ProductComboTemp proComboTemp, string old_id);
        List<ProductTemp> QueryChildStatusVendor(ProductComboTemp query);

        #region 與供應商商品相關
        string Vendor_TempMoveCombo(ProductComboTemp proComboTemp);
        #endregion
    }
}
