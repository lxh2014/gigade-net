/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductTempImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/29 11:21:45 
 *        修改歷史：                                                                     
 *         v1.1修改日期：2014/8/18 16:42：12 
 *         v1.1修改人員：shuangshuang0420j     
 *         v1.1修改内容：新增獲取供應商商品待審核列表
 *                       List<BLL.gigade.Model.Custom.VenderProductListCustom> verifyWaitQuery(Model.Query.QueryVerifyCondition qcCon, out int totalCount)
 *                       新增 說明：獲取供應商新建的商品 
 *                        Model.ProductTemp GetProTempByVendor(Model.ProductTemp proTemp);
 *                        新增 string Update(Model.ProductTemp proTemp);
 *                        新增 供應商商品臨時表數據刪除string DeleteVendorTemp(Model.ProductTemp proTemp);
 *                        新增 供應商複製商品更新基本信息        int vendorBaseInfoUpdate(Model.ProductTemp pTemp);

 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductTempImplMgr
    {
        Model.ProductTemp GetProTemp(Model.ProductTemp proTemp);
        Model.ProductTemp GetVendorProTemp(Model.ProductTemp proTemp);
        int baseInfoSave(Model.ProductTemp pTemp);
        int baseInfoUpdate(Model.ProductTemp pTemp);
        int PriceBonusInfoSave(Model.ProductTemp proTemp);
        bool DescriptionInfoSave(Model.ProductTemp proTemp, List<Model.ProductTagSetTemp> tagTemps, List<Model.ProductNoticeSetTemp> noticeTemps);
        bool SpecInfoSave(Model.ProductTemp proTemp);
        bool CategoryInfoUpdate(Model.ProductTemp proTemp);
        bool FortuneInfoSave(Model.ProductTemp proTemp);
        bool DeleteTemp(int writerId, int combo_type, string product_Id);
        int ProductTempUpdate(Model.ProductTemp proTemp, string page);
        string MoveProduct(Model.ProductTemp proTemp);
        string Delete(Model.ProductTemp proTemp);
        bool DeleteVendorProductTemp(int writerId, int combo_type, string product_Id);
        string SaveFromPro(Model.ProductTemp proTemp);
        bool CopyProduct(int writerId, int combo_type, string product_Id);
        List<BLL.gigade.Model.Custom.VenderProductListCustom> verifyWaitQuery(Model.Query.QueryVerifyCondition qcCon, out int totalCount);
        List<Model.ProductTemp> GetProTempByVendor(Model.ProductTemp proTemp);
        string Update(Model.ProductTemp proTemp);
        string vendorBaseInfoSave(Model.ProductTemp p);
        int vendorBaseInfoUpdate(Model.ProductTemp pTemp);
        int ProductTempUpdateByVendor(Model.ProductTemp proTemp, string page);
        bool VendorDescriptionInfoUpdate(ProductTemp proTemp, List<ProductTagSetTemp> tagTemps, List<ProductNoticeSetTemp> noticeTemps);
        bool UpdateAchieve(Model.ProductTemp proTemp);
        int SaveTempByVendor(ProductTemp proTemp);
        int PriceBonusInfoSaveByVendor(Model.ProductTemp proTemp);
        int vendorSpecInfoSave(Model.ProductTemp proTemp);
        bool CategoryInfoUpdateByVendor(Model.ProductTemp proTemp);
        bool SaveTemp(ProductTemp proTemp);
        bool VendorCopyProduct(int writerId, int combo_type, string old_product_Id, ref string id, string product_Ipfrom);
        string VendorEditCM(ProductTemp proTemp);
        /// <summary>
        /// 供應商取消商品送審
        /// </summary>
        /// <param name="productTemp"></param>
        bool CancelVerify(ProductTemp productTemp);
        int GetDefaultArriveDays(ProductTemp prod);
    }
}
