using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    public class PromotionBannerRelationDao
    {
        private IDBAccess _accessMySql;
        private string connStr;
        public PromotionBannerRelationDao(string connectionstring)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }
        public List<PromotionBannerRelationQuery> GetRelationList(PromotionBannerRelationQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT pb_id,pbr.brand_id,vb.brand_name from promotion_banner_relation pbr LEFT JOIN vendor_brand vb ON pbr.brand_id=vb.brand_id WHERE 1=1");
                if (query.pb_id != 0)
                {
                    sql.AppendFormat(" AND pb_id={0}", query.pb_id);
                }
                if (query.brand_id != 0)
                {
                    sql.AppendFormat(" AND pbr.brand_id={0}", query.brand_id);
                }
                if (query.brand_name != string.Empty)
                {
                    sql.AppendFormat(" AND vb.brand_name LIKE '%{0}%'", query.brand_name);
                }
                return _accessMySql.getDataTableForObj<PromotionBannerRelationQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerRelationDao-->GetRelationList-->" + sql.ToString() + ex.Message, ex);
            }

        }

        public string DeleteBrand(PromotionBannerRelationQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"DELETE from promotion_banner_relation WHERE 1=1 ", query.pb_id);
                if (query.pb_id != 0)
                {
                    sql.AppendFormat(" AND  pb_id={0}", query.pb_id);
                }
                if (query.brand_id != 0)
                {
                    sql.AppendFormat(" AND  brand_id={0}", query.brand_id);
                }
                sql.AppendFormat(" ;");
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerRelationDao-->DeleteBrand-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string AddBrand(PromotionBannerRelationQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"INSERT INTO promotion_banner_relation VALUES({0},{1});",query.pb_id,query.brand_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerRelationDao-->AddBrand-->" + sql.ToString() + ex.Message, ex);
            }
        }
       
        public List<PromotionBannerRelationQuery> GetBrandIds(string pb_ids,string brand_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT brand_id,pb_id from promotion_banner_relation WHERE pb_id in ({0})", pb_ids);
                if (brand_id != string.Empty)
                {
                    sql.AppendFormat("AND brand_id={0}", brand_id);
                }
                sql.AppendFormat(" ORDER BY pb_id");
                return _accessMySql.getDataTableForObj<PromotionBannerRelationQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionBannerRelationDao-->GetBrandIds-->" + sql.ToString() + ex.Message, ex);
            }

        }
    }
}
