using BLL.gigade.Model.Temp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductOrderTempImplMgr
    {
        /// <summary>
        /// 查詢單一商品賣出記錄
        /// </summary>
        /// <param name="pot">一個ProductOrderTemp類型的條件</param>
        /// <returns>符合條件的List集合</returns>
        List<ProductOrderTemp> QuerySingle(ProductOrderTemp pot);

        /// <summary>
        /// 查詢組合商品父商品記錄
        /// </summary>
        /// <param name="pot">一個ProductOrderTemp類型的條件</param>
        /// <returns>符合條件的List集合</returns>
        List<ProductOrderTemp> QueryParent(ProductOrderTemp pot);

        /// <summary>
        /// 查詢組合商品子商品
        /// </summary>
        /// <param name="dt">時間條件</param>
        /// <param name="parentIds">父項productId</param>
        /// <returns></returns>
        List<ProductOrderTemp> QueryChild(DateTime dt, string parentIds);

        /// <summary>
        /// 將報表信息流返回
        /// </summary>
        /// <param name="fileName">要讀取的xml列</param>
        /// <param name="pot">條件</param>
        /// <param name="type">1:單一商品 2：組合商品</param>
        /// <returns>一個報表信息流</returns>
        MemoryStream OutToExcel(string fileName, ProductOrderTemp pot);
    }
}
