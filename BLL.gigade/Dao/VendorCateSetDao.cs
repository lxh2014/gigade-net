using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao
{
    public class VendorCateSetDao : IVendorCateSetImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public VendorCateSetDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            connStr = connectionStr;
        }
        public string GetMaxCodeSerial(Model.VendorCateSet vcs)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append(" select max(cate_code_serial) from  vendor_cate_set");

                strSql.AppendFormat(" where cate_code_big ='{0}' and cate_code_middle='{1}' and tax_type='{2}' ", vcs.cate_code_big, vcs.cate_code_middle, vcs.tax_type);
                return _dbAccess.getDataTable(strSql.ToString()).Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(" VendorCateSetDao-->GetMaxCodeSerial-->" + ex.Message + strSql.ToString(), ex);
            }
        }

        public string SaveSql(VendorQuery vcsquery)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("INSERT into vendor_cate_set (vendor_id,cate_code_big,cate_code_middle,cate_code_serial,tax_type)");
                strSql.AppendFormat(" values ('{0}','{1}','{2}','{3}','{4}')", vcsquery.vendor_id, vcsquery.prod_cate, vcsquery.buy_cate, vcsquery.serial, vcsquery.tax_type);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(" VendorCateSetDao-->SaveSql-->" + ex.Message + strSql.ToString(), ex);
            }
        }
    }
}
