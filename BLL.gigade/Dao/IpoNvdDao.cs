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
                sqlCount.Append(@" select  count(`row_id`) as totalCount ");
                sql.Append(@" select  invd.`row_id`,invd.`work_id`, invd.`ipo_id`, invd.`item_id`, invd.`ipo_qty`, invd.`out_qty`, invd.`com_qty`, invd.`cde_dt`, invd.`made_date`, invd.`work_status`, invd.`create_user`, invd.`create_datetime`, invd.`modify_user`, invd.`modify_datetime`,mu1.user_username as create_username,mu1.user_username as modify_username ");
                sqlWhere.Append(" from ipo_nvd invd ");
                //sqlCondi.Append(" left joinvd vendor v on v.vendor_id=i.vend_id ");
                sqlWhere.Append(" left join manage_user mu1 on mu1.user_id=invd.create_user ");
                sqlWhere.Append(" left join manage_user mu2 on mu2.user_id=invd.modify_user ");
                sqlWhere.Append(" where 1=1  ");
                sqlWhere.Append(" and work_status <> 'COM' ");
                //sqlWhere.AppendFormat(" and work_id = '{0}' ",string.Empty);
                //if (!string.IsNullOrEmpty(query.po_id))
                //{
                //    sqlCondi.AppendFormat(" and i.po_id ='{0}' ", query.po_id);
                //}
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
                    sqlWhere.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                }

                sql.Append(sqlWhere.ToString());

                return _access.getDataTableForObj<IpoNvdQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IpoDao.GetIpoNvdList-->" + ex.Message + sql.ToString(), ex);
            }

        }

        public int CreateTallyList(IpoNvdQuery query, string id)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" update ipo_nvd set work_id = '{0}',modify_user='{1}',modify_datetime='{2}' where 1=1 and row_id in ('{3}');",query.work_id,query.modify_user,CommonFunction.DateTimeToString(DateTime.Now), id);
                
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IpoDao.CreateTallyList-->" + ex.Message + sql.ToString(), ex);
            }

        }

    }
}
