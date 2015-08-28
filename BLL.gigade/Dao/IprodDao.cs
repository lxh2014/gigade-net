/**
 * chaojie_zz添加于2014/11/4 02:25 PM
 * 料位管理
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;

namespace BLL.gigade.Dao
{
   public class IprodDao:IIprodImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;

        public IprodDao(string connectionStr)
       {
           _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
           this.connStr = connectionStr;
       }
        #region Iprod的添加+int AddIprod(Iprod iprod)

       
        public int AddIprod(Iprod iprod)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@" insert into iprod(dc_id,prod_id,ucn,change_user,change_dtim,create_user,create_dtim,vend_prod_no,description,sdesc,crush_factor,psta_id,");
                sb.Append(" pend_del,commodity_type,buyer_ref,pwy_dte_ctl, cde_dt_incr,cde_dt_prod,cde_dt_var,hzd_ind,hzd_class,unit_ship_cse,lot_no_cntl,case_cost)VALUE(");
                sb.AppendFormat(" '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',", iprod.dc_id, iprod.prod_id, iprod.ucn, iprod.change_user, iprod.change_dtim, iprod.create_user, iprod.create_dtim, iprod.vend_prod_no);
                sb.AppendFormat(" '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',", iprod.description, iprod.sdesc, iprod.crush_factor, iprod.psta_id, iprod.pend_del, iprod.commodity_type, iprod.buyer_ref, iprod.pwy_dte_ctl);
                sb.AppendFormat(" '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", iprod.cde_dt_incr, iprod.cde_dt_prod, iprod.cde_dt_var, iprod.hzd_ind, iprod.hzd_class, iprod.unit_ship_cse, iprod.lot_no_cntl, iprod.case_cost);
                return _dbAccess.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IprodDao.AddIprod-->" +ex.Message+sb.ToString(),ex);
            }

        }
        #endregion
    }
}
