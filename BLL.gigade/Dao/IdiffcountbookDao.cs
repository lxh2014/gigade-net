using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class IdiffcountbookDao
    {
        private IDBAccess _access;

        public IdiffcountbookDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public DataTable GetIdiffCountBookList(IdiffcountbookQuery idiffQuery, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sqlcondition = new StringBuilder();
            totalCount = 0;
            try
            {
                sql.Append(@"SELECT icb.book_id,icb.cb_jobid,icb.loc_id,icb.pro_qty,icb.st_qty,icb.create_time,icb.item_id,icb.made_date,icb.cde_dt,mu.user_username FROM idiff_count_book icb 
LEFT JOIN manage_user mu on icb.create_user=mu.user_id where 1=1 ");
                sqlcount.Append(@"SELECT icb.book_id FROM idiff_count_book icb 
LEFT JOIN manage_user mu on icb.create_user=mu.user_id where 1=1 ");
                if (!string.IsNullOrEmpty(idiffQuery.loc_id))
                {
                    sql.AppendFormat(" and icb.loc_id like '%{0}%' ",idiffQuery.loc_id);
                }
                if (idiffQuery.item_id!=0)
                {
                    sql.AppendFormat(" and icb.item_id='{0}' ", idiffQuery.item_id);
                }
                if (!string.IsNullOrEmpty(idiffQuery.cb_jobid))
                {
                    sql.AppendFormat(" and icb.cb_jobid like '%{0}%' ", idiffQuery.cb_jobid);
                }
                
                if (idiffQuery.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString()+sqlcondition.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sqlcondition.AppendFormat(" order by  icb.book_id limit {0},{1};", idiffQuery.Start, idiffQuery.Limit);
                }
                return _access.getDataTable(sql.ToString()+sqlcondition.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IdiffcountbookDao-->GetIdiffCountBookList-->" + ex.Message + sql.ToString()+sqlcondition.ToString(), ex);
            }
        }
    }
}
