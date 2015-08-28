using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao
{
    public class BannerNewsContentDao
    {
        private IDBAccess _access;

        public BannerNewsContentDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        /// <summary>
        /// 列表頁
        /// </summary>
        /// <param name="bnc"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetBannerNewsContentList(BannerNewsContent bnc, out int totalCount)
        {
            totalCount = 0;
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            sql.AppendLine(@"SELECT news_id,news_site_id,news_title,news_content,news_link_url,news_link_mode,news_sort,news_status,");
            sql.AppendLine(@"FROM_UNIXTIME(news_start) AS news_start ,FROM_UNIXTIME(news_end) AS news_end,");
            sql.AppendLine(@"FROM_UNIXTIME(news_createdate) AS news_createdate,FROM_UNIXTIME(news_updatedate) AS news_updatedate,news_ipfrom");
            sqlwhere.AppendLine(@" FROM banner_news_content WHERE 1=1 ");
            if (bnc.news_site_id != 0)
            {
                sqlwhere.AppendFormat(@" AND news_site_id='{0}' ", bnc.news_site_id);
            }
            if (bnc.news_status == 3)
            {
                sqlwhere.AppendFormat(@" AND news_status=3 ");
            }
            else
            {
                sqlwhere.AppendFormat(@" AND news_status <>3 ");
            }
            sql.Append(sqlwhere.ToString());
            if (bnc.news_status == 3)
            {
                sql.AppendFormat(@" ORDER BY news_id DESC ");
            }
            else
            {
                sql.AppendFormat(@" ORDER BY news_sort DESC, news_id DESC ");
            }
            try
            {
                if (bnc.IsPage)
                {
                    sql.AppendFormat(@" LIMIT {0},{1} ", bnc.Start, bnc.Limit);
                    DataTable dt = _access.getDataTable(" SELECT news_id " + sqlwhere.ToString());
                    totalCount = dt.Rows.Count;
                }
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BannerNewsContentDao-->GetBannerNewsContentList" + ex.Message + sql.ToString(), ex);
            }

        }

        public int UpdateBannerNewsContent(BannerNewsContentQuery q)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" update banner_news_content  ");
                sql.AppendFormat(@" set news_title='{0}',news_link_url='{1}',news_link_mode='{2}',news_sort='{3}', ", q.news_title, q.news_link_url, q.news_link_mode, q.news_sort);
                sql.AppendFormat(@" news_start='{0}',news_end='{1}',news_updatedate='{2}', ", q.news_start, q.news_end,q.news_updatedate);
                sql.AppendFormat(@" news_status='{0}' ",q.news_status);
                sql.AppendFormat(" where news_id='{0}' ;",q.news_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BannerNewsContentDao-->UpdateBannerNewsContent" + ex.Message + sql.ToString(), ex);
            }
        }
        public int SaveBannerNewsContent(BannerNewsContentQuery q)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = new DataTable();
            try
            {
                sb.Append("SELECT * FROM serial where serial_id='8';");
                dt = _access.getDataTable(sb.ToString());
                if (dt.Rows.Count > 0)
                {
                    q.news_id = uint.Parse(dt.Rows[0]["serial_value"].ToString()) + 1;
                    sb.Clear();
                    sb.Append("insert into banner_news_content(news_id,news_site_id,news_title,news_content,news_link_url,news_link_mode,news_sort,news_status,news_start,news_end,news_createdate,news_updatedate,news_ipfrom) Value(");
                    sb.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}');", q.news_id, q.news_site_id, q.news_title, q.news_content, q.news_link_url, q.news_link_mode, q.news_sort, q.news_status, q.news_start, q.news_end, q.news_createdate, q.news_updatedate, q.news_ipfrom);
                    sb.AppendFormat("update serial set serial_value='{0}' where serial_id='8'; ", q.news_id);
                    return _access.execCommand(sb.ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("BannerNewsContentDao-->SaveBannerNewsContent" + ex.Message + sb.ToString(), ex);
            }
        }
    }
}
