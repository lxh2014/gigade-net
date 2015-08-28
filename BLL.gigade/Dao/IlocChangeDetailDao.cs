using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
    public class IlocChangeDetailDao
    {
        private IDBAccess _access;
        public IlocChangeDetailDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        /// <summary>
        /// 列表頁
        /// </summary>
        /// <param name="ilocDetailQuery"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public List<IlocChangeDetailQuery> GetIlocChangeDetailList(IlocChangeDetailQuery q, out int totalcount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            totalcount = 0;
            try
            {  
                sql.AppendLine(@"SELECT icd.icd_id,icd.icd_item_id,pt.product_name,'' as 'product_sz',icd.icd_status,pt.prepaid ,icd.icd_old_loc_id,icd.icd_create_time,icd.icd_new_loc_id,pe.cde_dt_incr,pe.pwy_dte_ctl,'' as 'isjq','' as 'isgq',cde_dt_var,cde_dt_shp,icd.icd_create_time,mu.user_username
FROM iloc_change_detail icd 
LEFT JOIN product_ext pe on icd.icd_item_id = pe.item_id 
LEFT JOIN product_item pii on icd.icd_item_id=pii.item_id 
LEFT JOIN product pt on pii.product_id=pt.product_id 
LEFT JOIN manage_user mu on icd.icd_create_user=mu.user_id where 1=1 ");
                if (!string.IsNullOrEmpty(q.productids))
                {
                    sqlwhere.AppendFormat(" and icd.icd_item_id in ({0})", q.productids);
                }
                if (!string.IsNullOrEmpty(q.icd_old_loc_id.Trim()))
                {
                    sqlwhere.AppendFormat(" and icd.icd_old_loc_id ='{0}'", q.icd_old_loc_id);
                }
                if (!string.IsNullOrEmpty(q.icd_new_loc_id.Trim()))
                {
                    sqlwhere.AppendFormat(" and icd.icd_new_loc_id ='{0}'", q.icd_new_loc_id);
                }
                if (!string.IsNullOrEmpty(q.start_time.ToString()) && q.start_time > DateTime.MinValue)
                {
                    sqlwhere.AppendFormat(" and icd.icd_create_time >='{0}'",  CommonFunction.DateTimeToString(q.start_time));
                }
                if (!string.IsNullOrEmpty(q.end_time.ToString()) && q.end_time > DateTime.MinValue)
                {
                    sqlwhere.AppendFormat(" and icd.icd_create_time <='{0}'", CommonFunction.DateTimeToString(q.end_time));
                }
                if (!string.IsNullOrEmpty(q.startloc.Trim()))
                {
                    sqlwhere.AppendFormat(" and icd.icd_new_loc_id >='{0}'", q.startloc);
                }
                if (!string.IsNullOrEmpty(q.endloc.Trim()))
                {
                    sqlwhere.AppendFormat(" and icd.icd_new_loc_id <='{0}'", q.endloc);
                }
                if (!string.IsNullOrEmpty(q.icd_status))
                {
                    sqlwhere.AppendFormat(" and icd.icd_status='{0}' ", q.icd_status);
                }
                if (q.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sql.ToString() + sqlwhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalcount = _dt.Rows.Count;
                    }
                    sqlwhere.AppendFormat(" limit {0},{1}", q.Start, q.Limit);
                }
                return _access.getDataTableForObj<IlocChangeDetailQuery>(sql.ToString() + sqlwhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IlocChangeDetail-->GetIlocChangeDetailList-->" + ex.Message + sql.ToString() + sqlwhere.ToString(), ex);
            }
        }

        public DataTable GetIlocChangeDetailExcelList(IlocChangeDetailQuery ilocDetailQuery)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            try
            {
                sql.AppendLine(@"SELECT icd.icd_item_id,pt.product_name,'' as 'product_sz',pt.prepaid ,icd.icd_old_loc_id,pe.cde_dt_incr,icd.icd_create_time,icd.icd_new_loc_id,pe.pwy_dte_ctl,'' as 'isjq','' as 'isgq',cde_dt_var,cde_dt_shp,icd.icd_create_time,mu.user_username,icd.icd_status
FROM iloc_change_detail icd 
LEFT JOIN product_ext pe on icd.icd_item_id = pe.item_id 
LEFT JOIN product_item pii on icd.icd_item_id=pii.item_id 
LEFT JOIN product pt on pii.product_id=pt.product_id 
LEFT JOIN manage_user mu on icd.icd_create_user=mu.user_id  where 1=1 ");
                if (ilocDetailQuery.icd_id != 0)
                {
                    sqlwhere.AppendFormat(" and icd.icd_id ='{0}'", ilocDetailQuery.icd_id);
                }
                if (!string.IsNullOrEmpty(ilocDetailQuery.productids))
                {
                    sqlwhere.AppendFormat(" and icd.icd_item_id in ({0})", ilocDetailQuery.productids);
                }
                if (!string.IsNullOrEmpty(ilocDetailQuery.icd_old_loc_id.Trim()))
                {
                    sqlwhere.AppendFormat(" and icd.icd_old_loc_id ='{0}'", ilocDetailQuery.icd_old_loc_id);
                }
                if (!string.IsNullOrEmpty(ilocDetailQuery.icd_new_loc_id.Trim()))
                {
                    sqlwhere.AppendFormat(" and icd.icd_new_loc_id ='{0}'", ilocDetailQuery.icd_new_loc_id);
                }
                DateTime dt = DateTime.Parse("1970-01-02 08:00:00");
                if (!string.IsNullOrEmpty(ilocDetailQuery.start_time.ToString()) && dt < ilocDetailQuery.start_time)
                {
                    sqlwhere.AppendFormat(" and icd.icd_create_time >='{0}' ", CommonFunction.DateTimeToString(ilocDetailQuery.start_time));
                }
                if (!string.IsNullOrEmpty(ilocDetailQuery.end_time.ToString()) && dt < ilocDetailQuery.end_time)
                {
                    sqlwhere.AppendFormat(" and icd.icd_create_time <='{0}' ", CommonFunction.DateTimeToString(ilocDetailQuery.end_time));
                }
                if (!string.IsNullOrEmpty(ilocDetailQuery.startloc.Trim()))
                {
                    sqlwhere.AppendFormat(" and icd.icd_new_loc_id >='{0}'", ilocDetailQuery.startloc);
                }
                if (!string.IsNullOrEmpty(ilocDetailQuery.endloc.Trim()))
                {
                    sqlwhere.AppendFormat(" and icd.icd_new_loc_id <='{0}'", ilocDetailQuery.endloc);
                }
                if (!string.IsNullOrEmpty(ilocDetailQuery.icd_status))
                {
                    sqlwhere.AppendFormat(" and icd.icd_status='{0}' ", ilocDetailQuery.icd_status);
                }
                return _access.getDataTable(sql.ToString() + sqlwhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IlocChangeDetail-->GetIlocChangeDetailList-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public int UpdateIcdStatus(IlocChangeDetailQuery ilocDetailQuery)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                if (!string.IsNullOrEmpty(ilocDetailQuery.icd_id_In))
                {
                    sql.AppendFormat(" set sql_safe_updates=0; update iloc_change_detail set icd_status='COM',icd_modify_time='{0}',icd_modify_user='{1}' where icd_id in ({2});set sql_safe_updates=1;", Common.CommonFunction.DateTimeToString(ilocDetailQuery.icd_modify_time),ilocDetailQuery.icd_modify_user,ilocDetailQuery.icd_id_In);

                    return _access.execCommand(sql.ToString());

                }
                else 
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("IlocChangeDetail-->UpdateIcdStatus-->" + ex.Message + sql.ToString(), ex);
            } 
        
        }
    }
}
