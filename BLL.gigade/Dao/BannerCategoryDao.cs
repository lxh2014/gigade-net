using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using System.Data;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    public class BannerCategoryDao : IBannerCategoryImplDao
    {
        private IDBAccess _access;
        public BannerCategoryDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public List<BannerCategory> GetBannerCategoryList(BannerCategory bc, out int totalCount)
        {
            totalCount = 0;
            StringBuilder sqlfield = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            sqlwhere.AppendLine(@" from banner_category bc ");
            //獲取下拉列表的數據
            if (bc.category_father_id == 0)
            {
                sqlfield.AppendLine(@" SELECT bc.category_id,bc.category_name "); ;
                sqlwhere.AppendLine(@" WHERE bc.category_father_id = 0 ORDER BY bc.category_sort ASC");
            }
            //獲取列表頁的數據
            else
            {
                sqlfield.AppendLine(@"SELECT bc.category_id,bc.category_father_id,bc.category_sort,");
                sqlfield.AppendLine(@" bc.category_name,bc.content_type,bc.content_id,bc.description,bc.activity,bc.created_on,bc.updated_on ");
                sqlfield.AppendLine(@" ,bcc.category_name as fcategory_name ");
                sqlfield.AppendLine(@" ,bs.banner_site_name ");
                sqlwhere.AppendLine(@" LEFT JOIN  (SELECT category_id,category_name FROM banner_category WHERE category_father_id = 0 ) bcc on bc.category_father_id=bcc.category_id");
                sqlwhere.AppendLine(@" LEFT JOIN  (SELECT banner_site_id,banner_site_name from banner_site) bs on bs.banner_site_id=bc.content_id");
                sqlwhere.AppendLine(@" WHERE 1=1 ");
                if (bc.category_father_id == -1)
                {
                    sqlwhere.AppendLine(@" AND bc.category_father_id>0 ");
                }
                else
                {
                    sqlwhere.AppendFormat(@" AND bc.category_father_id='{0}' ", bc.category_father_id);
                }
                sqlwhere.AppendLine(@"  ORDER BY category_father_id ASC, category_sort ASC  ");
            }

            sql.Append(sqlfield.ToString());
            sql.Append(sqlwhere.ToString());
            if (bc.IsPage)
            {
                sql.AppendFormat(@" LIMIT {0},{1};", bc.Start, bc.Limit);
                DataTable dt = _access.getDataTable("SELECT bc.category_id " + sqlwhere);
                totalCount = dt.Rows.Count;
            }
            try
            {
                return _access.getDataTableForObj<BannerCategory>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BannerCategoryDao-->GetBannerCategoryList" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
