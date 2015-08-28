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
    public class MarketProductMapDao : IMarketProductMapImplDao
    {
        private IDBAccess _access;
        public MarketProductMapDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public DataTable GetMarketProductMapList(MarketProductMapQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {// FROM_UNIXTIME(UNIX_TIMESTAMP(modified)) as modified_time, FROM_UNIXTIME(UNIX_TIMESTAMP(estimated_send_time)) as estimated_time,created_time
                //sql.Append(@" select  row_id,event_name,event_desc,q.event_id,q.group_id,link_url,promo_image,device,count_by,count,case when active_now=TRUE then 1 when active_now=false then 0 end as active_now,case when new_user=TRUE then 1 when new_user=false then 0 end as new_user,new_user_date,start,end,active,kuser,muser,created,modified ");
                sql.Append(@" select mpm.map_id,mpm.product_category_id,mpm.market_category_id, ");
                sql.Append("  mu.user_username as muser,mpm.modified,mc.market_category_code,mc.market_category_name,pc.category_name ");
                sqlCondi.Append(" from market_product_map mpm ");
                sqlCondi.Append("  left join product_category pc on mpm.product_category_id=pc.category_id left join market_category mc on mpm.market_category_id=mc.market_category_id ");
                sqlCondi.Append("  left join manage_user mu on mu.user_id=mpm.muser ");
                sqlCondi.Append(" where 1=1 ");
                if (!string.IsNullOrEmpty(query.product_number))
                {
                    sqlCondi.AppendFormat(" and mpm.product_category_id='{0}' or mc.market_category_code = '{1}' ", query.product_number, query.product_number);
                }
                if (!string.IsNullOrEmpty(query.product_name))
                {
                    sqlCondi.AppendFormat(" and mc.market_category_name like '%{0}%' or pc.category_name  like '%{1}%' ", query.product_name, query.product_name);
                }
                //

                sqlCondi.Append(" order by mpm.map_id desc ");
                totalCount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable("select count(mpm.map_id) as totalCount " + sqlCondi.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }
                    sqlCondi.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                }

                sql.Append(sqlCondi.ToString());

                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("MarketProductMapDao.GetMarketProductMapList-->" + ex.Message + sql.ToString(), ex);
            }

        }

        public string SavetMarketProductMap(MarketProductMapQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                if (query.map_id == 0)
                {
                    sql.Append(@"insert into market_product_map(product_category_id,market_category_id,kuser,muser,created,modified)values( ");
                    sql.AppendFormat(" '{0}','{1}','{2}','{3}' ", query.product_category_id, query.market_category_id, query.kuser, query.muser);
                    sql.AppendFormat(" ,'{0}','{1}')", CommonFunction.DateTimeToString(query.created), CommonFunction.DateTimeToString(query.modified));
                }
                else
                {
                    sql.AppendFormat(@" update market_product_map set product_category_id='{0}', market_category_id='{1}',  ", query.product_category_id, query.market_category_id);
                    sql.AppendFormat(" muser='{0}',modified='{1}' where map_id='{2}' ", query.muser, CommonFunction.DateTimeToString(query.modified), query.map_id);
                }
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("MarketProductMapDao.SavetMarketProductMap-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string SelectProductMapCount(MarketProductMapQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" select count(*) as num from market_product_map where product_category_id='{0}' and market_category_id='{1}' and map_id<>'{2}' ", query.product_category_id, query.market_category_id, query.map_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("MarketProductMapDao.SelectProductMapCount-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string DeleteMarketProductMap(string row_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"delete from  market_product_map  where map_id in ( {0} ) ;", row_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("MarketProductMapDao.DeleteNewPromoQuestion-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string DeleteMarketProductMap(MarketProductMapQuery model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@"delete from  market_product_map  where 1=1");
                if (model.map_id != 0)
                {
                    sql.AppendFormat(" and map_id ='{0}'", model.map_id);
                }
                if (model.market_category_id != 0)
                {
                    sql.AppendFormat(" and market_category_id ='{0}'", model.market_category_id);
                }
                sql.Append(";");
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("MarketProductMapDao.DeleteNewPromoQuestion-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
