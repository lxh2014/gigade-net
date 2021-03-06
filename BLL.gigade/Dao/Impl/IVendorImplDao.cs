﻿/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IVendorImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/1/14 13:48:06 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    interface IVendorImplDao
    {
        Vendor GetSingle(Vendor query);
        string GetLoginId(int vendorid);
        List<BLL.gigade.Model.Query.VendorQuery> Query(BLL.gigade.Model.Query.VendorQuery query, ref int totalCount);
        DataTable GetVendorDetail(string sqlwhere);

        List<Vendor> VendorQueryAll(Vendor query);
        List<Vendor> VendorQueryList(Vendor query);
        //int Update(VendorQuery model, string update_log);
        int Add(VendorQuery model);
        int IsExitEmail(string email);
        // Vendor Login(Vendor query);
        int Add_Login_Attempts(int vendor_id);
        void Modify_Vendor_Status(int vendor_id, int status);
        void Modify_Vendor_Confirm_Code(int vendor_id, string vendor_confirm_code);
        int EditPass(string vendorId, string newPass);
        List<ManageUser> GetVendorPM();

        string ReturnHistoryCon(VendorQuery model);
        string UpdateVendor(Vendor model);
        int GetOffGradeCount(string vendorId);
        int UnGrade(string vendorId, string active);
        List<Vendor> GetArrayDaysInfo(uint brand_id);
      
    }
}
