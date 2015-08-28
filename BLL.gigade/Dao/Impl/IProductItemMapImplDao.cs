using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System.Data;
namespace BLL.gigade.Dao.Impl
{
    public interface IProductItemMapImplDao
    {
        /// <summary>
        /// 通過外站編號和商品編號查詢商品對照信息
        /// </summary>
        /// <param name="cId">外站編號</param>
        /// <param name="pNo">商品編號</param>
        /// <param name="totalCount">總記錄</param>
        /// <returns></returns>
        List<Model.Query.ProductCompare> QueryProductByNo(int cId, int pNo,int pm_id, out int totalCount);
        /// <summary>
        /// 保存對照信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        //int Save(ProductCompare p);
        int Save(ProductItemMap p);
        int UpdatePIM(ProductItemMap p);
        string saveString(ProductItemMap p);
        void Save_Comb(ProductItemMapCustom p);
        bool comboSave(System.Collections.ArrayList delSqls, string itemMapSql, System.Collections.ArrayList mapSetSqls);
        /// <summary>
        /// 刪除對照信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        int Delete(ProductItemMap p);
        string deleteString(ProductItemMap p);
        /// <summary>
        /// 更新對照信息
        /// </summary>
        /// <param name="p">對照表MODEL</param>
        /// <returns></returns>
        int Update(ProductCompare p);
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
        /// 驗證對照信息表中是否已存在
        /// </summary>
        /// <param name="p">對照信息實體</param>
        /// <returns></returns>
        int Exist(ProductItemMapCustom p);

        /// <summary>
        /// 查詢組合商品對照是否已經存在
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        int Comb_Exist(ProductItemMapCustom p);

        /// <summary>
        /// 查看組合的商品信息是否符合
        /// </summary>
        /// <param name="product_id"></param>
        /// <param name="item_id"></param>
        /// <returns></returns>
        int Comb_Compare(uint product_id, uint item_id);

        /// <summary>
        /// 查詢商品信息是否是組合商品
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        List<ProductMapCustom> CombinationQuery(ProductItemMapCustom p);

        /// <summary>
        /// 組合商品子商品細項信息查詢
        /// </summary>
        /// <param name="child_id"></param>
        /// <param name="parent_id"></param>
        /// <returns></returns>
        List<ProductComboMap> ProductComboItemQuery(int child_id, int parent_id);

        List<ProductItemMap> QueryProductItemMap(ProductItemMap p);

        /// <summary>
        /// 查詢固定組合商品下的子商品信息
        /// </summary>
        /// <param name="product_id"></param>
        /// <returns></returns>
        List<ProductComboMap> QueryItemId(uint product_id);

        List<ProductComboMap> ComboItemQuery(ProductCombo query);

        /// <summary>
        /// 查找是否存在對照了
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
        DataTable ProductItemMapExc(ProductItemMap m);
        int Selrepeat(string cdid);
    }
}
