/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductTempImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/29 11:21:16 
 * 修改歷史：                                                                     
*         v1.1修改日期：2014/8/18 16:42：12 
*         v1.1修改人員：shuangshuang0420j     
*         v1.1修改内容：新增獲取供應商商品待審核列表
*                       List<BLL.gigade.Model.Custom.VenderProductListCustom> verifyWaitQuery(Model.Query.QueryVerifyCondition qcCon, out int totalCount)
*                       新增 獲取供應商新建的數據
*                       Model.ProductTemp GetProTempByVendor(Model.ProductTemp proTemp);
 *                      新增  修改數據+string Update(ProductTemp proTemp)
 *                      新增  刪除供應商商品臨時表數據 string DeleteVendorTemp(Model.ProductTemp proTemp)
 *                      新增  新增供應商商品基本資料+int vendorBaseInfoSave(ProductTemp p)
 *                      新增  供應商商品複製更新int vendorBaseInfoUpdate(ProductTemp pTemp)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using BLL.gigade.Model;
namespace BLL.gigade.Dao.Impl
{
    interface IProductTempImplDao
    {
        Model.ProductTemp GetProTemp(Model.ProductTemp proTemp);
        Model.ProductTemp GetVendorProTemp(Model.ProductTemp proTemp);
        int baseInfoSave(Model.ProductTemp pTemp);
        int baseInfoUpdate(Model.ProductTemp pTemp);
        int SpecInfoSave(Model.ProductTemp proTemp);
        int CategoryInfoUpdate(Model.ProductTemp proTemp);
        int FortuneInfoSave(Model.ProductTemp proTemp);
        int PriceBonusInfoSave(Model.ProductTemp proTemp);
        int ProductTempUpdate(Model.ProductTemp proTemp, string page);

        string DescriptionInfoSave(Model.ProductTemp proTemp);

        string MoveProduct(Model.ProductTemp proTemp);
        string Delete(Model.ProductTemp proTemp);

        string SaveFromPro(Model.ProductTemp proTemp);
        bool CopyProduct(ArrayList execSql, ArrayList specs, string selMaster, string moveMaster, string movePrice);
        List<BLL.gigade.Model.Custom.VenderProductListCustom> verifyWaitQuery(Model.Query.QueryVerifyCondition qcCon, out int totalCount);
        List<Model.ProductTemp> GetProTempByVendor(Model.ProductTemp proTemp);
        string Update(Model.ProductTemp proTemp);

        string DeleteVendorTemp(Model.ProductTemp proTemp);
        string vendorBaseInfoSave(Model.ProductTemp p);
        int vendorBaseInfoUpdate(Model.ProductTemp pTemp);
        int ProductTempUpdateByVendor(Model.ProductTemp proTemp, string page);
        string VendorDescriptionInfoSave(Model.ProductTemp proTemp);
        int SaveTempByVendor(ProductTemp proTemp);
        int vendorSpecInfoSave(Model.ProductTemp proTemp);
        int PriceBonusInfoSaveByVendor(Model.ProductTemp proTemp);
        int CategoryInfoUpdateByVendor(Model.ProductTemp proTemp);
        bool SaveTemp(ProductTemp proTemp);
        bool UpdateAchieve(Model.ProductTemp proTemp);
        int VendorSaveFromPro(ProductTemp proTemp);
        bool VendorCopyProduct(ArrayList execSql, ArrayList specs, string selMaster, string moveMaster, string movePrice);
        string VendorEditCM(ProductTemp proTemp);

        #region 與供應商商品相關
        string Vendor_MoveProduct(ProductTemp proTemp);
        string Vendor_Delete(ProductTemp proTemp);
        #endregion

        bool CancelVerify(ProductTemp productTemp);

        int GetDefaultArriveDays(ProductTemp temp);
    }
}
