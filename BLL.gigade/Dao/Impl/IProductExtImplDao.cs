using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao.Impl
{
    public interface IProductExtImplDao
    {
        /// <summary>
        /// 查詢product_ext
        /// </summary>
        /// <param name="pe">查詢條件</param>
        /// <returns>符合條件的集合</returns>
        List<ProductExtCustom> Query(int[] ids, ProductExtCustom.Condition condition);

        /// <summary>
        /// 刪除結果
        /// </summary>
        /// <param name="pe">刪除條件</param>
        /// <returns>受影響的行數</returns>
        int DeleteProductExtByCondition(ProductExtCustom pe);

        /// <summary>
        /// 操作商品細項
        /// </summary>
        /// <param name="pe">需要操作的商品條件</param>
        /// <returns>操作是否成功</returns>
        string UpdateProductExt(ProductExtCustom pe);
        int UpdatePendDel(uint proudctId, bool penDel);

        //根據有效期限修改數據  add by zhuoqin0830w 2015/06/09
        bool UpdateExtByCdedtincr(List<ParticularsSrc> particularsSrc);

        List<ProductExtCustom> QueryHistoryInfo(DateTime Update_start, DateTime Update_end, int Brand_id = 0, string Item_ids = null, string Product_ids = null);
    }
}
