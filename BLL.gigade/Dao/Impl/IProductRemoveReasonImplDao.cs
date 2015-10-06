using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IProductRemoveReasonImplDao
    {
        //獲取到上架商品庫存<=0,並且排除掉庫存為0可販賣機暫停售賣的商品
        DataTable GetStockLessThanZero();
        string InsertProductRemoveReason(ProductRemoveReason prr);
        DataTable GetStockMsg();
        string DeleteProductRemoveReason(ProductRemoveReason prr);
        string UpdateProductStatus(Product pt);
        string InsertIntoProductStatusHistory(ProductStatusHistory psh);
        DataTable GetOutofStockMsg();
        DataTable GetProductRemoveReasonList();
        int ProductRemoveReasonTransact(string str);
        DataTable GetDeleteProductRemoveReasonList();//获取要删除的临时表中的数据
    }
}
