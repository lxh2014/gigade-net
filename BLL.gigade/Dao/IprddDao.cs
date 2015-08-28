using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
//changjian 于 2014/11/04 建立
namespace BLL.gigade.Dao
{
    
   public class IprddDao:IIprddImplDao
    {
       private IDBAccess _access;
       public IprddDao(string connectionString)
       {
          _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
       }


       #region IIprddImplDao 成员

       public int InsertIprdd(Iprdd iprd)
       {

           StringBuilder sb = new StringBuilder();
           try
           {
               sb.AppendFormat(@"INSERT INTO iprdd (dc_id,prod_id,prdd_id,cse_wid,cse_wgt,cse_unit,");
               sb.AppendFormat(@"cse_len,cse_hgt,cse_cub,dim_chng_dtim,stor_hi,stor_ti,");
               sb.AppendFormat(@"unit_ship_cse,vend_id,vnds_id,upc,prod_sz,vend_hi,");
               sb.AppendFormat(@"vend_ti,inner_pack_wid,inner_pack_wgt,inner_pack_uint,inner_pack_len,inner_pack_hgt,");
               sb.AppendFormat(@"inner_pack_cub,eaches_wid,eaches_wgt,eaches_len,eaches_hgt,eaches_cub,");
               sb.AppendFormat(@"change_dtim,change_user,create_dtim,create_user,nest_hgt,lst_cnt_dt,nest_wid,nest_len)");
               sb.AppendFormat(@"VALUES('{0}','{1}','{2}','{3}','{4}','{5}',");
               sb.AppendFormat(@"'{6}','{7}','{8}','{9}','{10}','{11}',");
               sb.AppendFormat(@"'{12}','{13}','{14}','{15}','{16}','{17}',");
               sb.AppendFormat(@"'{18}','{19}','{20}','{21}','{22}','{23}',");
               sb.AppendFormat(@"'{24}','{25}','{26}','{27}','{28}','{29}',");
               sb.AppendFormat(@"'{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}')",
                   iprd.dc_id, iprd.prod_id, iprd.prdd_id, iprd.cse_wid, iprd.cse_wgt, iprd.cse_unit,
                   iprd.cse_len, iprd.cse_hgt, iprd.cse_cub, iprd.dim_chng_dtim, iprd.stor_hi, iprd.stor_ti,
                   iprd.unit_ship_cse, iprd.vend_id, iprd.vnds_id, iprd.upc, iprd.prod_sz, iprd.vend_hi,
                   iprd.vend_ti, iprd.inner_pack_wid, iprd.inner_pack_wgt, iprd.inner_pack_uint, iprd.inner_pack_len, iprd.inner_pack_hgt,
                   iprd.inner_pack_cub, iprd.eaches_wid, iprd.eaches_wgt, iprd.eaches_len, iprd.eaches_hgt, iprd.eaches_cub,
                   iprd.change_dtim, iprd.change_user, iprd.create_dtim, iprd.create_user, iprd.nest_hgt, iprd.lst_cnt_dt, iprd.nest_wid, iprd.nest_len
                   );
               return _access.execCommand(sb.ToString());
           }
           catch (Exception ex)
           {

               throw new Exception("IprddDao-->InsertIprdd-->"+ex.Message+sb.ToString(),ex);
           }
           

       }

       #endregion
    }
}
