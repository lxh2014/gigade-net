using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductDeliverySetImplMgr
    {
        /// <summary>
        /// 新增插入信息
        /// </summary>
        /// <param name="sc">一個ProductDeliverySet對象</param>
        /// <returns>受影響的行數</returns>
        bool Save(List<ProductDeliverySet> pds,int delProdId);

        /// <summary>
        /// 導入excel,批量設置本島店配
        /// </summary>
        /// <param name="filePath">excel路徑</param>
        /// <param name="deliverySet">設置的物流配送模式</param>
        /// <returns>設置結果集</returns>
        List<ProdDeliverySetImport> Save(string filePath, out string resultPath, ProductDeliverySet deliverySet);

        /// <summary>
        /// 導出物流配送模式
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns>excel流</returns>
        MemoryStream ExportProdDeliverySet(ProductDeliverySet query);

        /// <summary>
        /// 查詢符合條件的ProductDeliverySet集合
        /// </summary>
        /// <param name="pds">查詢的條件</param>
        /// <returns>符合條件的集合</returns>
        List<ProductDeliverySet> QueryByProductId(int productId);


        bool Delete(ProductDeliverySet prodDeliSet, params uint[] productIds);
        /// <summary>
        /// 根據上傳的excel批量刪除配送模式
        /// </summary>
        /// <param name="filePath">excel路徑</param>
        /// <param name="deliverySet">要刪除的配送模式</param>
        /// <returns></returns>
        bool Delete(string filePath, ProductDeliverySet deliverySet);

    }
}
