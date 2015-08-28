using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model;
namespace BLL.gigade.Dao.Impl
{
    interface IProductComboImplDao
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
        List<ProductCombo> GetParentList(int product_id);///查詢組合商品中子商品為product_id的組合商品 add by wwei2016w 2015/7/06
    }
}
