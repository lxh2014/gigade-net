using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class EventChinaTrustBagMapDao
    {
        private IDBAccess _access;
        private string connStr;
        string strSql = string.Empty;
        public EventChinaTrustBagMapDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
              this.connStr = connectionString;
        }
        /// <summary>
        /// 獲取EventChinaTrustBagMap數據
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<EventChinaTrustBagMapQuery> GetChinaTrustBagMapList(EventChinaTrustBagMapQuery query, out int totalCount)
        {
            StringBuilder str = new StringBuilder();
            StringBuilder strall = new StringBuilder();
            StringBuilder strcounts = new StringBuilder();
            try
            {
                strcounts.AppendFormat("select count(ec.map_id) as totalcounts  FROM event_chinatrust_bag_map ec ");
                strall.AppendFormat("SELECT ec.map_id,ec.bag_id,ec.product_id,ec.linkurl,ec.product_forbid_banner,ec.linkurl,ec.ad_product_id,ec.product_desc, ec.product_active_banner,ec.map_active,p.product_name,ec.map_sort, ect.event_name,ecb.bag_name FROM event_chinatrust_bag_map ec ");
                //str.AppendFormat(" LEFT JOIN vote_event ve ON va.event_id=ve.event_id ");
                str.AppendFormat(" LEFT JOIN product p ON ec.product_id=p.product_id ");
                str.AppendFormat(" LEFT JOIN event_chinatrust_bag ecb on ecb.bag_id=ec.bag_id ");
                str.AppendFormat(" LEFT JOIN event_chinatrust ect on ect.event_id=ecb.event_id ");
                //str.AppendFormat(" LEFT JOIN users m ON va.user_id=m.user_id ");
                str.AppendFormat(" where 1=1 ");
                totalCount = 0;
                if (query.bag_id > 0)
                {
                    str.AppendFormat(" and ec.bag_id='{0}' ", query.bag_id);
                }
                if (query.map_id > 0)//活動編號
                {
                    str.AppendFormat(" and ec.map_id='{0}' ", query.map_id);
                }
                if (query.search_con ==2)
                {
                    if (query.con != "")
                    {
                        str.AppendFormat(" and ecb.bag_name  like N'%{0}%' ", query.con);
                    }
                }
              else  if (query.search_con == 1)
                {
                    if (query.con != "")
                    {
                        str.AppendFormat(" and ect.event_name like N'%{0}%' ", query.con);
                    }
                }
                totalCount = 0;
                if (query.IsPage)
                {
                    strcounts.Append(str.ToString());
                    System.Data.DataTable _dt = _access.getDataTable(strcounts.ToString() );

                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalcounts"]);
                    }
                    str.AppendFormat("order by ec.bag_id DESC,ec.map_sort DESC limit {0},{1} ", query.Start, query.Limit);
                }
                strall.Append(str.ToString());
                return _access.getDataTableForObj<EventChinaTrustBagMapQuery>(strall.ToString() );
            }
            catch (Exception ex)
            {
                throw new Exception(" EventChinaTrustBagMapDao-->GetChinaTrustBagMapList-->" + ex.Message + "sql:" + strall.ToString() + strcounts.ToString(), ex);
            }
        }

        public int Save(EventChinaTrustBagMapQuery q)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("insert into event_chinatrust_bag_map (bag_id,product_id,linkurl,product_forbid_banner,product_active_banner,map_active,map_sort,ad_product_id,product_desc) Value");
                sb.AppendFormat("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", q.bag_id, q.product_id, q.linkurl, q.product_forbid_banner, q.product_active_banner, q.map_active, q.map_sort,q.ad_product_id,q.product_desc);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" EventChinaTrustBagMapDao-->Save-->" + ex.Message + "sql:" + sb.ToString(), ex);
            }
        }

        public int Update(EventChinaTrustBagMapQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                if (query.map_id > 0)
                {
                    sql.AppendFormat("update event_chinatrust_bag_map set bag_id='{0}',product_id='{1}',linkurl='{2}',product_forbid_banner='{3}',product_active_banner='{4}',map_active='{5}',map_sort='{6}', ad_product_id='{7}',product_desc='{8}' ", query.bag_id, query.product_id, query.linkurl, query.product_forbid_banner, query.product_active_banner, query.map_active, query.map_sort,query.ad_product_id, query.product_desc);
                    sql.AppendFormat(" where map_id='{0}';", query.map_id);
                    return _access.execCommand(sql.ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(" EventChinaTrustBagMapDao-->Update-->" + ex.Message + "sql:" + sql.ToString(), ex);
            }
        }

        public int UpdateStatus(EventChinaTrustBagMapQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                if (query.map_id > 0)
                {
                    sql.AppendFormat("update event_chinatrust_bag_map set map_active='{0}' ", query.map_active);
                    sql.AppendFormat(" where map_id='{0}';", query.map_id);
                    return _access.execCommand(sql.ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(" EventChinaTrustBagMapDao-->UpdateStatus-->" + ex.Message + "sql:" + sql.ToString(), ex);
            }
        }

        public DataTable GetMapSort(EventChinaTrustBagMapQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                if (query.bag_id > 0)
                {
                    sql.AppendFormat("SELECT map_sort from event_chinatrust_bag_map WHERE bag_id='{0}' Order by map_sort DESC;", query.bag_id);
                    return _access.getDataTable(sql.ToString());
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(" EventChinaTrustBagMapDao-->GetMapSort-->" + ex.Message + "sql:" + sql.ToString(), ex);
            }
        }

        public bool IsProductId(string id)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("select product_id from product where product_id='{0}';", id);
                if (_access.getDataTable(sb.ToString()).Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(" EventChinaTrustBagMapDao-->IsProductId-->" + ex.Message + "sql:" + sb.ToString(), ex);
            }
        }
    }
}
