using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;
using System.IO;
using System.Data;
namespace BLL.gigade.Mgr.Impl
{
    public interface IProductItemMapImplMgr
    {

        /// <summary>
        /// 通過外站編號和商品編號查詢商品對照信息
        /// </summary>
        /// <param name="cId">外站編號</param>
        /// <param name="pNo">商品編號</param>
        /// <param name="totalCount">總記錄</param>
        /// <returns></returns>
        List<Model.Query.ProductCompare> QueryProductByNo(int cId, int pNo,int pmId, out int totalCount);
        /// <summary>
        /// 查詢外站下拉列表
        /// </summary>
        /// <returns></returns>
        List<Model.Channel> QueryOutSite();
        /// 查詢站台價格下拉列表 add by xiangwang0413w 2014/07/01
        /// </summary>
        /// <param name="productId">商品id</param>
        /// <returns></returns>
        List<PriceMasterCustom> QuerySitePriceOption(uint productId, uint channelId);
        /// <summary>
        /// 對照之保存
        /// </summary>
        /// <param name="save"></param>
        /// <returns></returns>
        //int Save(ProductCompare save);
        /// <summary>
        /// 單一商品對照之保存
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        bool SingleCompareSave(List<ProductItemMap> pList);
        /// <summary>
        /// 刪除對照信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        int Delete(ProductItemMap p);
        /// <summary>
        /// 對照信息查詢
        /// </summary>
        /// <param name="p">查詢條件實體</param>
        /// <param name="totalCount">總記錄</param>
        /// <returns></returns>
        List<ProductItemMapCustom> QueryProductItemMap(ProductItemMapQuery p, out int totalCount);

        /// <summary>
        /// 查詢所有信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        List<ProductItemMap> QueryAll(ProductItemMap p);

        /// <summary>
        /// 查詢商品類型及組合商品下的子商品
        /// </summary>
        /// <param name="product_id">商品ID</param>
        /// <returns></returns>
        List<ProductMapCustom> CombinationQuery(ProductItemMapCustom p);

        /// <summary>
        /// 組合商品子商品細項信息
        /// </summary>
        /// <param name="child_id"></param>
        /// <param name="parent_id"></param>
        /// <returns></returns>
        List<ProductComboMap> ProductComboItemQuery(int child_id, int parent_id);

        /// <summary>
        /// 組合商品對照之保存
        /// </summary>
        /// <param name="saveList"></param>
        /// <returns></returns>
        bool ComboCompareSave(List<ProductItemMapCustom> saveList);
        /// <summary>
        /// 組合商品細項對照查詢
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<ProductComboMap> ComboItemQuery(ProductCombo query);
        /// <summary>
        /// 組合商品對照之刪除
        /// </summary>
        /// <param name="delete"></param>
        /// <returns></returns>
        bool ComboDelete(ProductItemMap delete);
        /// <summary>
        /// 查找是否在同一個外站下有相同的賣場ID
        /// </summary>
        /// <param name="p"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        List<ProductItemMap> QueryAll(uint channelId, string condition);

        /// <summary>
        /// 查找建立過對照的信息
        /// </summary>
        /// <param name="Pip"></param>
        /// <returns></returns>
        List<ProductItemMap> QueryOldItemMap(ProductItemMap Pip);
        
        /// <summary>
        /// 為判斷庫存
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<ProductItemMap> QueryChannel_detail_id(string id);
        //匯出商品對照信息-20140805
        DataTable ProductItemMapExc(ProductItemMap Pip);
        int Selrepeat(string cdid);
        int UpdatePIM(ProductItemMap p);
        int Save(ProductItemMap p);

        /// <summary>
        /// 匯出商品對照檔
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        MemoryStream ExportProductItemMap(QueryVerifyCondition query);
    }
}
