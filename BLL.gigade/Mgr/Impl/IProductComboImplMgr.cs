using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductComboImplMgr
    {
        List<ProductComboCustom> combQuery(ProductComboCustom query);
        List<ProductComboCustom> combNoPriceQuery(ProductComboCustom query);
        List<ProductCombo> groupNumQuery(ProductCombo query);
        string Save(ProductCombo combo);
        string Delete(int parent_Id);
        List<ProductComboCustom> sameSpecQuery(ProductComboCustom query);
        List<MakePriceCustom> differentSpecQuery(ProductComboCustom query);
        List<MakePriceCustom> differentNoPriceSpecQuery(ProductComboCustom query);
        List<ProductComboCustom> getChildren(ProductComboCustom query);
        /// <summary>
        /// 獲得子商品是child_id的組合商品字符串
        /// </summary>
        /// <param name="child_Id">單一商品id</param>
        string GetParentList(int child_Id);
    }
}
