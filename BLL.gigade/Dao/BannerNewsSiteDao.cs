using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class BannerNewsSiteDao
    {
        private IDBAccess _access;
        public BannerNewsSiteDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public List<BannerNewsSiteQuery> GetList(BannerNewsSiteQuery bs, out int totalCount)
        {
            StringBuilder sqlfield = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            StringBuilder sqlorderby = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            sqlfield.AppendLine(@"SELECT	news_site_id,news_site_sort,news_site_status,news_site_mode,news_site_name,news_site_description,news_site_createdate,news_site_updatedate,news_site_ipfrom ");
            sqlfield.AppendLine(@" FROM	banner_news_site where news_site_status = 1 ");
            sql.Append(sqlfield);
            //sql.Append(sqlwhere);
            sqlorderby.AppendFormat(@" ORDER BY news_site_sort DESC ");
            sql.Append(sqlorderby);
            sql.AppendFormat(@" limit {0},{1};", bs.Start, bs.Limit);
            //int totalCount;
            totalCount = 0;
            try
            {
                if (bs.IsPage)
                {
                    DataTable dt = _access.getDataTable("select count(*) from banner_news_site where 1=1 " + sqlwhere);
                    totalCount = int.Parse(dt.Rows[0][0].ToString());
                }
                return _access.getDataTableForObj<BannerNewsSiteQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BannerNewsSiteDao-->GetList" + ex.Message + sql.ToString(), ex);
            }

        }
        /// <summary>
        /// 獲得站台名
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        public List<BannerNewsSite> GetBannerNewsSiteName(BannerNewsSite bs)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"SELECT news_site_id,news_site_name ");
            sql.AppendLine(@" FROM	banner_news_site where 1=1 ");
            if (bs.news_site_id != 0)
            {
                sql.AppendFormat(@" AND news_site_id='{0}' ", bs.news_site_id);
            }
            try
            {
                return _access.getDataTableForObj<BannerNewsSite>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BannerNewsSiteDao-->GetBannerNewsSiteName" + ex.Message + sql.ToString(), ex);
            }

        }
    }
}
