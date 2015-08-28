#region 文件信息
/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：VendorLoginListMgr.cs 
 * 摘   要： 
 *      供應商管理-->供應商登入記錄
 * 当前版本：v1.1 
 * 作   者： changjian0408j
 * 完成日期：2014/10/7
 */

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Mgr.Impl;
namespace BLL.gigade.Mgr
{
    public class VendorLoginListMgr:IVendorLoginListImplMgr
    {
        private IVendorLoginListImplDao _vendorlogin;
        public VendorLoginListMgr(string connectionString)
        {
            _vendorlogin = new VendorLoginListDao(connectionString);
        }


        #region 查詢供應商登入記錄列表+List<Model.Query.VendorLoginQuery> Query(Model.Query.VendorLoginQuery store, out int totalCount)

        public List<Model.Query.VendorLoginQuery> Query(Model.Query.VendorLoginQuery store, out int totalCount)
        {
            try
            {
                return _vendorlogin.Query(store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("VendorLoginListMgr.Query-->"+ex.Message,ex);
            }
        }

        #endregion
    }
}
