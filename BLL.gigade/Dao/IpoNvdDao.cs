using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
    public class IpoNvdDao
    {
         private IDBAccess _access;
        string strSql = string.Empty;
        public IpoNvdDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        /// <summary>
        /// 採購單收貨加價記錄，列表頁
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public List<IpoNvdQuery> GetIpoNvdList(IpoNvdQuery query, out int totalcount)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            try
            {
                sqlCount.Append(@" select  count(invd.`row_id`) as totalCount ");
                sql.Append(@" select  invd.`row_id`,invd.`work_id`, invd.`ipo_id`, invd.`item_id`, invd.`ipo_qty`, invd.`out_qty`, invd.`com_qty`, invd.`cde_dt`, invd.`made_date`, invd.`work_status`, invd.`create_user`, invd.`create_datetime`, invd.`modify_user`, invd.`modify_datetime`,mu1.user_username as create_username,mu1.user_username as modify_username ");
                sql.Append(@",pe.pwy_dte_ctl,pe.cde_dt_incr,iplas.loc_id,CONCAT('(',invd.item_id,')',v.brand_name,'-',p.product_name) as description,concat(IFNULL(ps1.spec_name,''),IFNULL(ps2.spec_name,'')) as prod_sz ");
                sqlWhere.Append(" from ipo_nvd invd ");
                //sqlCondi.Append(" left joinvd vendor v on v.vendor_id=i.vend_id ");
                sqlWhere.Append(" left join manage_user mu1 on mu1.user_id=invd.create_user ");
                sqlWhere.Append(@" left join manage_user mu2 on mu2.user_id=invd.modify_user 
LEFT JOIN product_ext pe ON invd.item_id = pe.item_id 
LEFT JOIN iplas ON iplas.item_id=invd.item_id 
LEFT join iloc ic on iplas.loc_id=ic.loc_id  
LEFT JOIN product_item pi ON invd.item_id = pi.item_id 
LEFT JOIN product_spec ps1 ON pi.spec_id_1 = ps1.spec_id
LEFT JOIN product_spec ps2 ON pi.spec_id_2 = ps2.spec_id
LEFT JOIN product p ON pi.product_id=p.product_id 
LEFT JOIN vendor_brand v ON p.brand_id=v.brand_id ");
                sqlWhere.Append(" where 1=1  ");


                if (!string.IsNullOrEmpty(query.work_id))
                {
                    if (query.work_id == "empty")
                    {
                        sqlWhere.AppendFormat(" and invd.work_id = '{0}' ", string.Empty);
                    }
                    else
                    {
                        sqlWhere.AppendFormat(" and invd.work_id = '{0}' ", query.work_id);
                    }
                }
                if (!string.IsNullOrEmpty(query.work_status))
                {
                    if (query.work_status == "all_type")
                    {
                        sqlWhere.AppendFormat(" and work_status in ('AVL','SKP','COM') ");
                    }
                    else
                    {
                        sqlWhere.AppendFormat(" and work_status in ('{0}') ", query.work_status);
                    }
                }
                if (query.item_id!=0)
                {
                    sqlWhere.AppendFormat(" and invd.item_id = '{0}' ", query.item_id);
                }
                if (!query.locid_allownull)
                {
                    sqlWhere.AppendFormat(" and iplas.loc_id IS NOT NULL ");
                }
                if (!string.IsNullOrEmpty(query.ipo_id))
                {
                    sqlWhere.AppendFormat(" and ipo_id = '{0}' ", query.ipo_id);
                }
                if (query.start_time != DateTime.MinValue && query.end_time != DateTime.MinValue)
                {
                    sqlWhere.AppendFormat(" and invd.create_datetime between '{0}' and '{1}'",
                      CommonFunction.DateTimeToString(query.start_time), CommonFunction.DateTimeToString(query.end_time));

                }
                //if (query.start_time!=DateTime.MinValue)
                //{
                //    sqlCondi.AppendFormat(" and i.create_dtim >= '{0}'", Common.CommonFunction.DateTimeToString(query.start_time));
                //}
                //if (query.end_time != DateTime.MinValue)
                //{
                //    sqlCondi.AppendFormat(" and i.create_dtim <= '{0}'", Common.CommonFunction.DateTimeToString(query.end_time));
                //}
                
                totalcount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable(sqlCount.ToString() + sqlWhere.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalcount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }
                    sqlWhere.AppendFormat(" ORDER BY invd.row_id limit {0},{1} ", query.Start, query.Limit);
                }

                sql.Append(sqlWhere.ToString());

                return _access.getDataTableForObj<IpoNvdQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IpoDao.GetIpoNvdList-->" + ex.Message + sql.ToString(), ex);
            }

        }
        public IpoNvdQuery GetIpoNvd(IpoNvdQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" select  invd.`row_id`,invd.`work_id`, invd.`ipo_id`, invd.`item_id`, invd.`ipo_qty`, invd.`out_qty`, invd.`com_qty`, invd.`cde_dt`, invd.`made_date`, invd.`work_status`, invd.`create_user`, invd.`create_datetime`, invd.`modify_user`, invd.`modify_datetime`  from ipo_nvd invd where row_id = '{0}';", query.row_id);

                return _access.getSinggleObj<IpoNvdQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IpoNvdDao.GetIpoNvd-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string UpdateIpoNvdSql(IpoNvdQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" update ipo_nvd set out_qty = '{0}',com_qty='{1}',made_date='{2}',cde_dt='{3}',work_status='{4}',", query.out_qty, query.com_qty, query.made_date.ToString("yyyy-MM-dd"), query.cde_dt.ToString("yyyy-MM-dd"),query.work_status);
                sql.AppendFormat(@" modify_user='{0}',modify_datetime='{1}' where 1=1 and row_id = '{2}';", query.modify_user, CommonFunction.DateTimeToString(DateTime.Now),query.row_id);

                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("IpoNvdDao.UpdateIpoNvdSql-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string InsertIpoNvdLogSql(IpoNvdLogQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" INSERT INTO `ipo_nvd_log` (`work_id`, `ipo_id`, `item_id`, `add_qty`, `made_date`, `cde_date`, `create_user`, `create_datetime`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');", query.work_id, query.ipo_id, query.item_id, query.add_qty, query.made_date.ToString("yyyy-MM-dd"), query.cde_date.ToString("yyyy-MM-dd"), query.create_user, CommonFunction.DateTimeToString(DateTime.Now));

                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("IpoNvdDao.InsertIpoNvdLogSql-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int CreateTallyList(IpoNvdQuery query, string id)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" update ipo_nvd set work_id = '{0}',modify_user='{1}',modify_datetime='{2}' where 1=1 and row_id in ({3});",query.work_id,query.modify_user,CommonFunction.DateTimeToString(DateTime.Now), id);
                
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IpoNvdDao.CreateTallyList-->" + ex.Message + sql.ToString(), ex);
            }
        }

    }
}
