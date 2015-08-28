using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductDeliverySetTempImplMgr
    {
        bool Save(List<ProductDeliverySetTemp> proDeliSets,int delProdId, int comboType, int writerId);
        List<ProductDeliverySetTemp> QueryByProductId(ProductDeliverySetTemp query);
        string MoveProductDeliverySet(ProductDeliverySetTemp proDelSetTemp);
        string SaveFromProDeliverySet(ProductDeliverySetTemp proDelSetTemp);
        string Delete(ProductDeliverySetTemp proDelSetTemp);
    }
}
