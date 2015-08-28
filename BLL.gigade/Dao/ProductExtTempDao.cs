using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model.Temp;

namespace BLL.gigade.Dao
{
    public class ProductExtTempDao:IProdcutExtTempImplDao
    {
        private IDBAccess _dbAccess;
        public ProductExtTempDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        /// <summary>
        /// 保存ProducTextTemp臨時表
        /// </summary>
        /// <param name="pe">保存條件</param>
        /// <returns>受影響的行數</returns>
        //public int Save(ProductExtTemp pe)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("INSERT INTO product_ext_temp ");
        //    sb.AppendFormat("VALUES({0},'{1}',{2},'{3}',{4},{5},'{6}',{7},{8},'{9}',", pe.item_id, pe.pend_del, pe.cde_dt_shp, pe.pwy_dte_ctl, pe.cde_dt_incr, pe.cde_dt_var, pe.hzd_ind, pe.cse_wid, pe.cse_wgt, pe.cse_unit);
        //    sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}) ", pe.cse_len, pe.cse_hgt, pe.unit_ship_cse, pe.inner_pack_wid, pe.inner_pack_wgt, pe.inner_pack_unit, pe.inner_pack_len, pe.inner_pack_wgt);
        //    return _dbAccess.execCommand(sb.ToString());
        //}

        /// <summary>
        /// 查詢ProductTextTemp表
        /// </summary>
        /// <param name="pe">查詢條件</param>
        /// <returns>滿足條件的集合</returns>
//        public List<ProductExtTemp> Query(ProductExtTemp pe)
//        {
//            StringBuilder sb = new StringBuilder();
//            sb.Append(@"SELECT 
//	pet.item_id,pet.pend_del,pet.cde_dt_shp,pet.pwy_dte_ctl,pet.cde_dt_incr,pet.cde_dt_var,pet.hzd_ind,pet.cse_wid,pet.cse_wgt,pet.cse_unit,
//	pet.cse_len,pet.cse_hgt,pet.unit_ship_cse,pet.inner_pack_wid,pet.inner_pack_wgt,pet.inner_pack_unit,pet.inner_pack_len,pet.inner_pack_hgt 
// FROM product_ext_temp pet WHERE 1=1 ");
//            if (pe.item_id != 0)
//            {
//                sb.AppendFormat("AND item_id = {0}", pe.item_id);
//            }
//            return _dbAccess.getDataTableForObj<ProductExtTemp>(sb.ToString());
//        }
    }
}
