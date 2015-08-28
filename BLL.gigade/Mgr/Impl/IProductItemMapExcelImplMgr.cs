using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductItemMapExcelImplMgr
    {
        /// <summary>
        /// 讀取Excel文件
        /// </summary>
        /// <param name="filePath">文件路徑</param>
        /// <param name="channel_id">外站編號</param>
        /// <param name="fExtension">文件擴展名</param>
        /// <param name="isHeader">文件表頭信息是否可識別</param>
        /// <param name="errorPm">錯誤信息</param>
        /// <returns></returns>
        List<ProductItemMapCustom> ReadFile(string filePath, int channel_id, string fExtension, ref bool isHeader, List<ProductItemMapCustom> errorPm);

        /// <summary>
        /// 保存Excel數據到數據庫
        /// </summary>
        /// <param name="pm">EXCEL實體</param>
        /// <returns></returns>
        int SaveToDB(List<ProductItemMapCustom> pm);

        /// <summary>
        /// 創建Excel範本
        /// </summary>
        /// <returns></returns>
        MemoryStream CreateExcelTable();

        /// <summary>
        /// 查詢product_item_map表中是否有數據
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        List<ProductItemMap> QueryProductItemMap(ProductItemMap p);
    }
}
