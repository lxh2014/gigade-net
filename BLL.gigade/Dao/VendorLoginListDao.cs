#region 文件信息
/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：VendorLoginListDao.cs 
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
using DBAccess;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Common;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao
{
    public class VendorLoginListDao : IVendorLoginListImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        public VendorLoginListDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        #region 查詢供應商登入記錄列表+List<VendorLoginQuery> Query(VendorLoginQuery store, out int totalCount)

        public List<VendorLoginQuery> Query(VendorLoginQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            sql.Append("SELECT  FROM_UNIXTIME(vl.login_createdate) as slogin_createdate,vl.login_id,vl.login_ipfrom,v.vendor_code,v.vendor_name_simple as username,v.vendor_id ");
            sqlcount.Append("select count(vl.vendor_id) as totalcounts ");
            sqlfrom.Append("FROM vendor_login vl left join vendor v on vl.vendor_id=v.vendor_id ");
            sqlfrom.Append(" WHERE 1=1");
            if (store.serchstart != DateTime.MinValue)
            {
                sqlfrom.AppendFormat(" and vl.login_createdate >= '{0}'", CommonFunction.GetPHPTime(store.serchstart.ToString()));
            }
            if (store.serchend != DateTime.MinValue)
            {
                sqlfrom.AppendFormat(" and vl.login_createdate <= '{0}'", CommonFunction.GetPHPTime(store.serchend.ToString()));
            }

            if (!string.IsNullOrEmpty(store.vendor_code))
            {
                sqlfrom.AppendFormat(" and vendor_code LIKE '%{0}%'", store.vendor_code);
            }
            if (store.vendor_id == 0)
            {
                sqlfrom.AppendFormat("");
            }
            else
            {
                sqlfrom.AppendFormat(" and vl.vendor_id = '{0}'", store.vendor_id);
            }
            if (store.login_id != 0)
            {
                sqlfrom.AppendFormat(" and vl.login_id = '{0}'", store.login_id);
            }
            sqlfrom.AppendFormat(" ORDER BY login_createdate DESC, vl.vendor_id ASC");
            totalCount = 0;
            if (store.IsPage)
            {
                System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString() + sqlfrom.ToString());

                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["totalcounts"]);
                }

                sqlfrom.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
            }
            try
            {
                return _access.getDataTableForObj<VendorLoginQuery>(sql.ToString() + sqlfrom.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("VendorLoginListDao.Query-->" + ex.Message + sql.ToString() + sqlfrom.ToString(), ex);
            }

        }

        #endregion
    }
}
