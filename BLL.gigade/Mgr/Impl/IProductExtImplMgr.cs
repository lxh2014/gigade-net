using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductExtImplMgr
    {
        /// <summary>
        /// 查詢product_ext
        /// </summary>
        /// <param name="pe">查詢條件</param>
        /// <returns>符合條件的集合</returns>
        List<ProductExtCustom> Query(ProductExtCustom.Condition condition,params int[] ids);

        /// <summary>
        /// 操作商品細項
        /// </summary>
        /// <param name="pe">需要操作的商品條件</param>
        /// <returns>操作是否成功</returns>
        //string UpdateProductExt(ProductExtCustom pe);
        bool UpdateProductExt(List<ProductExtCustom> lists, Caller _caller, string controlId);
        /// <summary>
        /// 刪除結果
        /// </summary>
        /// <param name="pe">刪除條件</param>
        /// <returns>受影響的行數</returns>
        int DeleteProductExtByCondition(ProductExtCustom pe);
        bool UpdatePendDel(uint proudctId, bool penDel);

        List<ProductExtCustom> QueryHistoryInfo(Int64 Update_start, Int64 Update_end, int Brand_id = 0, string Item_ids = null, string Product_ids = null);
        /// <summary>
        ///歷史記錄excel匯出方法
        /// </summary>
        MemoryStream OutToExcel(string fileName, Int64 Update_start, Int64 Update_end, int Brand_id = 0, string Item_ids = null, string Product_ids = null);
    }
}
