using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class NewPromoRecordDao : INewPromoRecordImplDao
    {
        private IDBAccess _access;

        public NewPromoRecordDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public List<Model.Query.NewPromoRecordQuery> NewPromoRecordList(Model.Query.NewPromoRecordQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            totalCount = 0;
            try
            {//,ip,create_time,user_tel,user_address,user_mail
                sql.AppendFormat(@"SELECT row_id,event_id,message,event_type,user_id,user_name,user_reg_date ");
                sqlwhere.Append(" FROM new_promo_record ");
                sqlwhere.Append(" WHERE 1=1 ");
                sqlcount.Append("select count(row_id) as totalCount ");
                if (!string.IsNullOrEmpty(query.event_id))
                {
                    sqlwhere.AppendFormat(" and event_id='{0}'", query.event_id);
                }
                sqlcount.Append(sqlwhere.ToString());
                DataTable dt = _access.getDataTable(sqlcount.ToString());
                if (dt != null && dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                }
                sql.Append(sqlwhere.ToString());
                return _access.getDataTableForObj<NewPromoRecordQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoRecordDao-->NewPromoRecordList-->" + ex.Message + sql.ToString() + sqlcount.ToString(), ex);
            }
        }
    }
}
