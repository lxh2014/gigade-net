using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using System.Data;
using BLL.gigade.Dao.Impl;

namespace BLL.gigade.Dao
{
    public class BannerSiteDao : IBannerSiteImplDao
    {
        private IDBAccess _access;
        public BannerSiteDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public List<BannerSite> GetList(BannerSite bs, out int totalCount)
        {
            StringBuilder sqlfield = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            StringBuilder sqlorderby = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            sqlfield.AppendLine(@"SELECT banner_site_id,banner_site_sort,banner_site_status,banner_site_name,");
            sqlfield.AppendLine(@"banner_site_description,banner_site_createdate,banner_site_updatedate,banner_site_ipfrom  ");
            sqlfield.AppendLine(@"from banner_site where 1=1 ");
            sql.Append(sqlfield);
            if (bs.banner_site_status != 0)
            {
                sqlwhere.AppendFormat(@" and banner_site_status='{0}' ", bs.banner_site_status);
            }
            sql.Append(sqlwhere);
            sqlorderby.AppendFormat(@" ORDER BY banner_site_status ASC, banner_site_sort DESC ");
            sql.Append(sqlorderby);
            sql.AppendFormat(@" limit {0},{1};", bs.Start, bs.Limit);
            //int totalCount;
            totalCount = 0;
            try
            {
                if (bs.IsPage)
                {
                    DataTable dt = _access.getDataTable("select count(*) from banner_site where 1=1 " + sqlwhere);
                    totalCount = int.Parse(dt.Rows[0][0].ToString());
                }
                return _access.getDataTableForObj<BannerSite>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BannerSiteDao-->GetList" + ex.Message + sql.ToString(), ex);
            }

        }
        /// <summary>
        /// 獲取站點名稱
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        public List<BannerSite> GetBannerSiteName(BannerSite bs)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@" SELECT banner_site_id,banner_site_name FROM banner_site ");
            sql.AppendLine(@" WHERE 1=1 ");
            if (bs.banner_site_id != 0)
            {
                sql.AppendFormat(@" AND banner_site_id='{0}'",bs.banner_site_id);
            }
            try
            {
                return _access.getDataTableForObj<BannerSite>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BannerSiteDao-->GetBannerSiteName" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
