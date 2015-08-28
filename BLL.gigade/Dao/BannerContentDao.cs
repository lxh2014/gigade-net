using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using System.Data;
using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;

namespace BLL.gigade.Dao
{
    public class BannerContentDao:IBannerContentImplDao
    {
        private IDBAccess _access;
        public BannerContentDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public List<BannerContent> GetList(BannerContent bc, out int totalCount)
        {
            StringBuilder sqlfield = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            StringBuilder sqlorderby = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            sqlfield.AppendLine(@"SELECT banner_content_id,banner_site_id,banner_title,banner_link_url,");
            sqlfield.AppendLine(@"banner_link_mode,banner_sort,banner_status,banner_image,FROM_UNIXTIME(banner_start) as banner_start,");
            sqlfield.AppendLine(@"FROM_UNIXTIME(banner_end) as banner_end,FROM_UNIXTIME(banner_createdate) as banner_createdate,");
            sqlfield.AppendLine(@"FROM_UNIXTIME(banner_updatedate) as banner_updatedate,banner_ipfrom ");
            sqlfield.AppendLine(@" from banner_content WHERE 1=1 ");
            sql.Append(sqlfield);
            if (bc.banner_content_id != 0)
            {
                sql.AppendFormat(@" and banner_content_id='{0}' ", bc.banner_content_id);
            }
            if (bc.banner_site_id != 0)
            {
                sqlwhere.AppendFormat(@" and banner_site_id='{0}' ", bc.banner_site_id);
            }
            if (bc.banner_status == 3)
            {
                sqlwhere.AppendFormat(@" and banner_status='{0}' ", bc.banner_status);
                sqlorderby.AppendFormat(@" ORDER BY banner_content_id DESC ");
            }
            else
            {
                sqlwhere.AppendFormat(@" and banner_status <> 3  ");
                sqlorderby.AppendFormat(@" ORDER BY banner_sort DESC, banner_content_id DESC ");
            }
            sql.Append(sqlwhere);
            sql.Append(sqlorderby + string.Format("limit {0},{1}", bc.Start, bc.Limit));
            //int totalCount;
            totalCount = 0;
            try
            {
                if (bc.IsPage)
                {
                    DataTable dt = _access.getDataTable("select count(*) from banner_content where 1=1 " + sqlwhere);
                    totalCount = int.Parse(dt.Rows[0][0].ToString());
                }
                return _access.getDataTableForObj<BannerContent>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BannerContentDao-->GetList" + ex.Message + sql.ToString(), ex);
            }

        }

        public int Add(BannerContent bc)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"INSERT INTO banner_content (banner_content_id,banner_site_id,banner_title,banner_link_url,");
            sql.AppendLine(@"banner_link_mode,banner_sort,banner_status,banner_image,banner_start,");
            sql.AppendLine(@"banner_end,banner_createdate,banner_updatedate,banner_ipfrom) values(");
            sql.AppendFormat(@" '{0}','{1}','{2}','{3}',", bc.banner_content_id, bc.banner_site_id, bc.banner_title, bc.banner_link_url);
            sql.AppendFormat(@" '{0}','{1}','{2}','{3}','{4}',", bc.banner_link_mode, bc.banner_sort, bc.banner_status, bc.banner_image, CommonFunction.GetPHPTime(bc.banner_start.ToString("yyyy-MM-dd HH:mm:ss")));
            sql.AppendFormat(@" '{0}','{1}','{2}','{3}')", CommonFunction.GetPHPTime(bc.banner_end.ToString("yyyy-MM-dd HH:mm:ss")), CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), bc.banner_ipfrom);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BannerContentDao-->Add" + ex.Message + sql.ToString(), ex);

            }

        }
        public int Update(BannerContent bc)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@" UPDATE banner_content set  ");
            sql.AppendFormat(@"banner_title='{0}',banner_link_mode='{1}',",bc.banner_title,bc.banner_link_mode);
            sql.AppendFormat(@" banner_sort='{0}',banner_status='{1}',",bc.banner_sort,bc.banner_status);
            sql.AppendFormat(@" banner_image='{0}',banner_start='{1}',banner_end='{2}',", bc.banner_image, CommonFunction.GetPHPTime(bc.banner_start.ToString("yyyy-MM-dd HH:mm:ss")), CommonFunction.GetPHPTime(bc.banner_end.ToString("yyyy-MM-dd HH:mm:ss")));
            sql.AppendFormat(@"banner_updatedate='{0}',banner_ipfrom='{1}', ", CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), bc.banner_ipfrom);
            sql.AppendFormat(@"banner_link_url='{0}'",bc.banner_link_url);
            sql.AppendFormat(@"  WHERE 1=1 and banner_content_id='{0}'; ",bc.banner_content_id);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BannerContentDao-->Update" + ex.Message + sql.ToString(), ex);

            }
        }
    }
}
