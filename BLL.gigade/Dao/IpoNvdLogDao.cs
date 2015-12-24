using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class IpoNvdLogDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        public IpoNvdLogDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        /// <summary>
        /// 採購單,列表頁
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public List<IpoNvdLogQuery> GetIpoNvdLogList(IpoNvdLogQuery query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder conndSql = new StringBuilder();
            try
            {
                sql.Append(@"select inl.row_id,inl.`work_id`, inl.`ipo_id`, inl.item_id, i.upc_id,ip.loc_id, `add_qty`, `made_date`, `cde_date`,pe.pwy_dte_ctl,mu.user_username as create_user_string, inl.`create_datetime` 
                             FROM ipo_nvd_log inl LEFT JOIN iupc i on inl.item_id=i.item_id 
                             INNER  JOIN manage_user mu on mu.user_id=inl.create_user  
                             left join iplas ip on ip.item_id=inl.item_id 
                             left join product_ext pe on pe.item_id=inl.item_id
                             where 1=1 ");

                if (!string.IsNullOrEmpty(query.work_id))
                {
                    conndSql.AppendFormat(" and inl.work_id ='{0}' ", query.work_id);
                }
                if (!string.IsNullOrEmpty(query.ipo_id))
                {
                    conndSql.AppendFormat(" and inl.ipo_id ='{0}' ", query.ipo_id);
                }

                if (!string.IsNullOrEmpty(query.upc_id))
                {
                    conndSql.AppendFormat(" and i.upc_id ='{0}' ", query.upc_id);
                }
                if (query.item_id !=0)
                {
                    conndSql.AppendFormat(" and inl.item_id ='{0}' ", query.item_id);
                }
                if (!string.IsNullOrEmpty(query.loc_id))
                {
                    conndSql.AppendFormat(" and ip.loc_id ='{0}' ", query.loc_id);
                }


                if (query.start_time!=DateTime.MinValue)
                {
                    conndSql.AppendFormat(" and inl.create_datetime >= '{0}'", Common.CommonFunction.DateTimeToString(query.start_time));
                }
                if (query.end_time != DateTime.MinValue)
                {
                    conndSql.AppendFormat(" and inl.create_datetime <= '{0}'", Common.CommonFunction.DateTimeToString(query.end_time));
                }
                conndSql.Append("group by inl.row_id order by inl.create_datetime desc ");

                sql = sql.Append(conndSql.ToString());
                totalCount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = new DataTable();
                    _dt = _access.getDataTable(@"select count(tab.row_id) as totalCount from (select inl.row_id  FROM ipo_nvd_log inl LEFT JOIN iupc i on inl.item_id = i.item_id 
                             INNER  JOIN manage_user mu on mu.user_id=inl.create_user  
                             left join iplas ip on ip.item_id=inl.item_id                                        
                             where 1=1 " + conndSql.ToString() + ") tab");
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sql.AppendFormat(" limit {0},{1}", query.Start, query.Limit);

                }
                return _access.getDataTableForObj<IpoNvdLogQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IpoNvdLogDao-->GetIpoNvdLogList-->" + ex.Message + sql.ToString(), ex);
            }

        }

        public bool GetInfoByItemId(uint itemId)
        {
            
            StringBuilder sql = new StringBuilder();
            try
            {
                bool result = false;
                sql.AppendFormat("select item_id from ipo_nvd_log where item_id='{0}'", itemId);
                DataTable _dt = _access.getDataTable(sql.ToString());
                if (_dt.Rows.Count > 0)
                {
                    result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("IpoNvdLogDao-->GetInfoByItemId-->" + ex.Message + sql.ToString(), ex);
            }
        }
       
    }
}
