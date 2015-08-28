using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class ArrivalNoticeDao : IArrivalNoticeImplDao
    {
        private IDBAccess _access;

        public ArrivalNoticeDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public List<ArrivalNoticeQuery> ArrivalNoticeList(ArrivalNoticeQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            totalCount = 0;
            try
            {
                sqlCount.Append("select count(an.user_id) as totalCount ");
                sql.Append(" select an.id,an.user_id,u.user_email, u.user_name,an.item_id,pi.item_stock,an.product_id,p.product_name, an.`status`,an.create_time,an.coming_time   ");
                sqlFrom.Append("  from arrival_notice as an  ");
                sqlFrom.Append(" LEFT JOIN users u on an.user_id=u.user_id ");
                sqlFrom.Append("  LEFT JOIN product p on an.product_id=p.product_id  ");
                sqlFrom.Append("  left JOIN product_item pi on an.item_id=pi.item_id  ");
                sqlWhere.Append(" where 1=1   ");
                if (query.condition == 1 && query.searchCon != "")
                {
                    sqlWhere.AppendFormat("  and u.user_name like '%{0}%' ", query.searchCon);
                }
                else if (query.condition == 2 && query.searchCon != "")
                {
                    sqlWhere.AppendFormat("  and p.product_name like '%{0}%' ", query.searchCon);
                }
                if (query.status != -1)
                {
                    sqlWhere.AppendFormat("  and an.`status` ={0} ", query.status);
                }
                sqlWhere.AppendFormat("  and pi.item_stock >{0} ", query.item_stock);
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable(sqlCount.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlWhere.AppendFormat("limit {0},{1}", query.Start, query.Limit);
                }
                return _access.getDataTableForObj<ArrivalNoticeQuery>(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ArrivalNoticeDao-->ArrivalNoticeList-->" + ex.Message + sql.ToString() + sqlWhere.ToString(), ex);
            }
        }


        public string IgnoreNotice(ArrivalNoticeQuery query)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("UPDATE  arrival_notice SET status='{0}' ,create_time='{1}' ", query.status, CommonFunction.GetPHPTime());
            if (query.status == 3 && query.coming_time != 0)
            {
                sql.AppendFormat(",coming_time='{0}'  ", query.coming_time);
            }
            sql.AppendFormat(" where id='{0}';",query.id);
            return sql.ToString();
        }
    }
}
