/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：IlocDao.cs 
 * 摘   要： 
 *      料位
 * 当前版本：v1.1 
 * 作   者： dongya0410j
 * 完成日期：2014/10/04
 * 修改歷史：
 *      v1.1修改日期：2014/10/04
 *      v1.1修改人員：dongya0410j
 *      v1.1修改内容：
 *      v1.1修改內容：chaojie1124j 添加GetIocList方法 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using MySql.Data.MySqlClient;
using BLL.gigade.Common;
using System.Data;

namespace BLL.gigade.Dao
{
    public class IlocDao:IIlocImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        string connStr;
        public IlocDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connStr = connectionString;
            
        }
        #region 插入料位+int IlocInsert(Model.Iloc loc)
        public int IlocInsert(Model.Iloc loc)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                HashEncrypt hashpt=new HashEncrypt();
                sb.AppendFormat(@"insert into iloc(dc_id,whse_id,loc_id,llts_id,bkfill_loc,ldes_id,
                ldim_id,x_coord,y_coord,z_coord,bkfill_x_coord,bkfill_y_coord,
                bkfill_z_coord,lsta_id,sel_stk_pos,sel_seq_loc,sel_pos_hgt,rsv_stk_pos,
                rsv_pos_hgt,stk_lmt,stk_pos_wid,lev,lhnd_id,ldsp_id,
                create_user,create_dtim,comingle_allow,change_user,change_dtim,lcat_id,
                space_remain,max_loc_wgt,loc_status,stk_pos_dep,hash_loc_id
                ) values ('{0}','{1}','{2}','{3}','{4}','{5}',
                '{6}','{7}','{8}','{9}','{10}','{11}',
                '{12}','{13}','{14}','{15}','{16}','{17}',
                '{18}','{19}','{20}','{21}','{22}','{23}',
                '{24}','{25}','{26}','{27}','{28}','{29}',
                '{30}','{31}','{32}','{33}','{34}')",
                loc.dc_id, loc.whse_id, loc.loc_id, loc.llts_id, loc.bkfill_loc, loc.ldes_id,
                loc.ldim_id, loc.x_coord, loc.y_coord, loc.z_coord, loc.bkfill_x_coord, loc.bkfill_y_coord,
                loc.bkfill_z_coord, loc.lsta_id, loc.sel_stk_pos, loc.sel_seq_loc, loc.sel_pos_hgt, loc.rsv_stk_pos,
                loc.rsv_pos_hgt, loc.stk_lmt, loc.stk_pos_wid, loc.lev, loc.lhnd_id, loc.ldsp_id,
                loc.create_user, Common.CommonFunction.DateTimeToString(loc.create_dtim), loc.comingle_allow, loc.change_user, Common.CommonFunction.DateTimeToString(loc.change_dtim), loc.lcat_id,
                loc.space_remain, loc.max_loc_wgt, loc.loc_status, loc.stk_pos_dep, hashpt.Md5Encrypt(loc.loc_id, "16")
                );
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IlocDao-->IlocInsert-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 料位列表
        /// <summary>
        /// 獲取料位列表
        /// </summary>
        /// <param name="loc">實體</param>
        /// <param name="totalCount"></param>
        /// <returns>料位列表</returns>
        public List<IlocQuery> GetIocList(IlocQuery loc, out int totalCount)
        {
            StringBuilder sb=new StringBuilder();
            StringBuilder sbt=new StringBuilder();
            StringBuilder sqlcount =new  StringBuilder();
           
            if (loc.lcat_id != "0")
            {
                sbt.AppendFormat(@" and iloc.lcat_id='{0}'",loc.lcat_id);
            }
            if (!string.IsNullOrEmpty(loc.lsta_id))
            {
                sbt.AppendFormat(@" and iloc.lsta_id='{0}'",loc.lsta_id );
            }
            if(!string.IsNullOrEmpty(loc.loc_id))
            {
                sbt.AppendFormat(@" and (iloc.loc_id like '%{0}%' or iloc.hash_loc_id LIKE '%{0}%') ", loc.loc_id);
            }
            if (!string.IsNullOrEmpty(loc.starttime.ToString()) && loc.starttime>DateTime.MinValue)
            {
                sbt.AppendFormat(@" and iloc.create_dtim >'{0}' ", loc.starttime);
            }
            if (!string.IsNullOrEmpty(loc.endtime.ToString()) && loc.endtime > DateTime.MinValue)
            {
                sbt.AppendFormat(@" and iloc.create_dtim < '{0}' ", loc.endtime);
            }
            sb.Append(@" select mu.user_username as change_users ,row_id,dc_id,whse_id,loc_id,llts_id,ldes_id,lsta_id,sel_stk_pos,sel_pos_hgt,rsv_stk_pos,rsv_pos_hgt,stk_pos_dep,stk_lmt,");
            sb.Append(" stk_pos_wid,lev,lhnd_id,ldsp_id,create_user,create_dtim,comingle_allow,change_dtim,lcat_id,hash_loc_id ");
            sb.Append(" from iloc left join manage_user mu on iloc.change_user=mu.user_id where 1=1 and loc_status=1 ");
            sb.Append(sbt.ToString());
            sb.AppendFormat(" order by loc_id asc ");
            //sqlcount.AppendFormat(" select count(*) as search_total from iloc where 1=1 and loc_status=1 ");
            //sqlcount.AppendFormat(sbt.ToString());
            totalCount = 0;

            try 
	        {	        
		        if (loc.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sb.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sb.AppendFormat(" limit {0},{1}", loc.Start, loc.Limit);
	          }
                return _access.getDataTableForObj<IlocQuery>(sb.ToString());
            }
	        catch (Exception ex)
	        {
                throw new Exception("IlocDao.GetIocList-->" + ex.Message + sb.ToString(), ex);
	        }
        }
        #endregion

        #region 編輯料位+int IlocInsert(Model.Iloc loc)
        public int IlocEdit(Model.Iloc loc)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"Update iloc Set dc_id='{1}',whse_id='{2}',loc_id='{3}',llts_id='{4}',bkfill_loc='{5}',ldes_id='{6}',ldim_id='{7}',
                    x_coord='{8}',y_coord='{9}',z_coord='{10}',bkfill_x_coord='{11}',bkfill_y_coord='{12}',bkfill_z_coord='{13}',
                    sel_stk_pos='{15}',sel_seq_loc='{16}',sel_pos_hgt='{17}',rsv_stk_pos='{18}',rsv_pos_hgt='{19}',stk_lmt='{20}',stk_pos_wid='{21}',
                    lev='{22}',lhnd_id='{23}',ldsp_id='{24}',comingle_allow='{25}',change_user='{26}',change_dtim='{27}',lcat_id='{28}',
                    space_remain='{29}',stk_pos_dep='{30}',max_loc_wgt='{14}' where row_id='{0}' ;", loc.row_id, loc.dc_id, loc.whse_id, loc.loc_id, loc.llts_id, loc.bkfill_loc,
                    loc.ldes_id,loc.ldim_id, loc.x_coord, loc.y_coord,loc.z_coord, loc.bkfill_x_coord, loc.bkfill_y_coord,loc.bkfill_z_coord,loc.max_loc_wgt,
                    loc.sel_stk_pos,loc.sel_seq_loc, loc.sel_pos_hgt,loc.rsv_stk_pos,loc.rsv_pos_hgt, loc.stk_lmt, loc.stk_pos_wid,loc.lev, loc.lhnd_id,
                    loc.ldsp_id, loc.comingle_allow, loc.change_user, Common.CommonFunction.DateTimeToString(loc.change_dtim), loc.lcat_id, loc.space_remain, loc.stk_pos_dep);                    
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IlocDao-->IlocEdit-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 判斷料位是否重複 +int GetLoc_id(Model.Iloc loc)
        public int GetLoc_id(Model.Iloc loc)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(@" select row_id from iloc where loc_id='{0}' and loc_status=1 ", loc.loc_id);
            int rownumber=0;
            try
            {
                    System.Data.DataTable _dt = _access.getDataTable(sb.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                         rownumber = _dt.Rows.Count;//判斷是否已經存在此料位
                    }
                return rownumber;
            }
            catch (Exception ex)
            {
                throw new Exception("IlocDao.GetLoc_id-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 根據row_id獲取到料位+ string  GetLoc_idByRow_id(int row_id)
        public string  GetLoc_idByRow_id(int row_id)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(@" select loc_id from iloc where row_id='{0}' and loc_status=1 ", row_id);
            string rownumber = "0";
            try
            {
               
                    System.Data.DataTable _dt = _access.getDataTable(sb.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        rownumber = _dt.Rows[0][0].ToString();//判斷是否已經存在此料位
                    }
                return rownumber;
            }
            catch (Exception ex)
            {
                throw new Exception("IlocDao.GetLoc_idByRow_id-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 刪除,也就是把loc_status狀態變成0,軟刪除+int DeleteLocidByIloc(Model.Iloc loc)
        public int DeleteLocidByIloc(Model.Iloc loc)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@" Delete from iloc where row_id  in ({0})", loc.loc_id);
            //sb.AppendFormat(@" update iloc set loc_status=0,change_user='{1}',change_dtim='{2}' where row_id  in ({0})", loc.loc_id, loc.change_user, Common.CommonFunction.DateTimeToString(loc.change_dtim), loc.lsta_id);
            try
            {
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IlocCheckDao-->DeleteLocidByIloc-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion


        public int UpdateIlocLock(Model.Iloc loc)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@" update iloc set lsta_id='{0}',change_user='{1}',change_dtim='{2}' where row_id='{3}' ", loc.lsta_id, loc.change_user, Common.CommonFunction.DateTimeToString(loc.change_dtim), loc.row_id);
            try
            {
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IlocCheckDao-->UpdateIlocLock-->" + ex.Message + sb.ToString(), ex);
            }
        }


        public int SetIlocUsed(Model.Iloc loc)//當商品上架時,改變料位的狀態,設置為已指派
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"set sql_safe_updates = 0; update iloc set lsta_id='{0}',change_user='{1}',change_dtim='{2}' where loc_id='{3}';set sql_safe_updates = 1; ", "A", loc.change_user, Common.CommonFunction.DateTimeToString(loc.change_dtim), loc.loc_id);
            try
            {
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IlocCheckDao-->SetIlocUsed-->" + ex.Message + sb.ToString(), ex);
            }
        }
        
        public List<IlocQuery> Export(IlocQuery loc)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbt = new StringBuilder();
           
            if (loc.lcat_id!="0")
            {
                sbt.AppendFormat(@" and iloc.lcat_id='{0}'", loc.lcat_id);
            }
            if (loc.startiloc!="" && loc.endiloc!="")
            {
                sbt.AppendFormat(@" and (iloc.loc_id>='{0}' and iloc.loc_id <='{1}') ", loc.startiloc,loc.endiloc);
            }
            sb.Append(@" select mu.user_username as change_users,loc_id,llts_id,ldes_id,lsta_id,sel_stk_pos,sel_pos_hgt,rsv_stk_pos,rsv_pos_hgt,stk_pos_dep,stk_lmt,");
            sb.Append(" stk_pos_wid,lev,lhnd_id,ldsp_id,create_user,create_dtim,comingle_allow,change_dtim,lcat_id");
            sb.Append(" from iloc left join manage_user mu on iloc.change_user=mu.user_id where 1=1 and loc_status=1 and lsta_id='F' ");
            sb.Append(sbt.ToString());
            sb.Append(" order by loc_id asc ");
            try
            {
                return _access.getDataTableForObj<IlocQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IlocDao.Export-->" + ex.Message + sb.ToString(), ex);
            }
        }
        
        public List<IlocQuery> GetIlocExportList(IlocQuery loc)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbt = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();

            if (loc.lcat_id != "0")
            {
                sbt.AppendFormat(@" and iloc.lcat_id='{0}'", loc.lcat_id);
            }
            if (!string.IsNullOrEmpty(loc.lsta_id))
            {
                sbt.AppendFormat(@" and iloc.lsta_id='{0}'", loc.lsta_id);
            }
            if (!string.IsNullOrEmpty(loc.loc_id))
            {
                sbt.AppendFormat(@" and (iloc.loc_id like '%{0}%' or iloc.hash_loc_id LIKE '%{0}%') ", loc.loc_id);
            }
            DateTime dt = DateTime.Parse("1970-01-01 08:00:00");
            if (!string.IsNullOrEmpty(loc.starttime.ToString()) && loc.starttime>dt)
            {
                sbt.AppendFormat(@" and iloc.create_dtim >'{0}' ", loc.starttime);
            }
            if (!string.IsNullOrEmpty(loc.endtime.ToString()) && loc.endtime > dt)
            {
                sbt.AppendFormat(@" and iloc.create_dtim < '{0}' ", loc.endtime);
            }
            sb.Append(@" select mu.user_username as change_users ,row_id,dc_id,whse_id,loc_id,llts_id,ldes_id,lsta_id,sel_stk_pos,sel_pos_hgt,rsv_stk_pos,rsv_pos_hgt,stk_pos_dep,stk_lmt,");
            sb.Append(" stk_pos_wid,lev,lhnd_id,ldsp_id,create_user,create_dtim,comingle_allow,change_dtim,lcat_id,hash_loc_id");
            sb.Append(" from iloc left join manage_user mu on iloc.change_user=mu.user_id where 1=1 and loc_status=1 ");
            sb.Append(sbt.ToString());
            sb.AppendFormat(" order by loc_id asc ");
            try
            {
                return _access.getDataTableForObj<IlocQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IlocDao.GetIocList-->" + ex.Message + sb.ToString(), ex);
            }
        }

        public int SaveBySql(string str)
        {
            try
            {
                return _access.execCommand(str.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IlocDao.SaveBySql-->" + ex.Message + str.ToString(), ex);
            }
        }


        public int HashAll()
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder str = new StringBuilder();
            try
            {
                HashEncrypt hashpt=new HashEncrypt();
                
                sql.AppendFormat("select row_id,loc_id,hash_loc_id from iloc where hash_loc_id is NULL;");
                DataTable _dt = _access.getDataTable(sql.ToString());
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    str.AppendFormat("UPDATE iloc SET hash_loc_id='{0}' WHERE row_id ='{1}';",hashpt.Md5Encrypt(_dt.Rows[i]["loc_id"].ToString(),"16"),_dt.Rows[i]["row_id"]);
                }
                if (!string.IsNullOrEmpty(str.ToString()))
                {
                    return _access.execCommand(str.ToString());
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("IlocDao.HashAll-->" + ex.Message + str.ToString(), ex);
            }
        }


        public string GetLocidByHash(string loc_id)
        {
            StringBuilder sql = new StringBuilder();
          
            try
            {
              
                sql.AppendFormat("select loc_id from iloc where hash_loc_id ='{0}';",loc_id);
                DataTable _dt = _access.getDataTable(sql.ToString());
                return _dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("IlocDao.GetLocidByHash-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
