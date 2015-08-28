using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Dao
{
    public class PageMetaDataDao
    {
        private IDBAccess _access;
        public PageMetaDataDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
         
        public List<PageMetaData> GetPageMetaDataList(PageMetaData query, ref int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            sql.Append("SELECT pm_id,pm_url_para,pm_page_name,pm_title,pm_keywords, pm_description,pm_created,pm_modified");
            sqlCondi.Append(" FROM page_metadata ");
            sqlCondi.Append(" WHERE 1=1 ");
            if (!string.IsNullOrEmpty(query.pm_page_name))
            {
                sqlCondi.AppendFormat(" and (pm_page_name like N'%{0}%' or pm_title like N'%{0}%' or pm_keywords like N'%{0}%')", query.pm_page_name);
            }
            try
            {

                if (query.IsPage)
                {
                    DataTable dt = _access.getDataTable("select count(pm_id) as totalCount " + sqlCondi.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                    }
                    sqlCondi.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                }
                sql.Append(sqlCondi.ToString());
                return _access.getDataTableForObj<PageMetaData>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PageMetaDataDao-->GetPageMetaDataList-->" + ex.Message + sql.ToString(), ex);
            }

        }

        public int UpdatePageMeta(PageMetaData store)
        {
            store.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update page_metadata set pm_url_para='{0}',pm_page_name='{1}',pm_title='{2}',pm_keywords='{3}'", store.pm_url_para
                    , store.pm_page_name, store.pm_title, store.pm_keywords);
                sql.AppendFormat(", pm_description='{0}',pm_modified='{1}',pm_modify_user='{2}'", store.pm_description, Common.CommonFunction.DateTimeToString(store.pm_modified), store.pm_modify_user);
                sql.AppendFormat(" where pm_id='{0}';", store.pm_id);
                return _access.execCommand(sql.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("PageMetaDataDao-->UpdatePageMeta-->" + ex.Message + sql.ToString(), ex);
            }

        }

        public int InsertPageMeta(PageMetaData store)
        {
            store.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("insert into page_metadata (pm_url_para,pm_page_name,pm_title,pm_keywords, pm_description,pm_created,pm_modified,pm_modify_user,pm_create_user)");
                sql.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                    store.pm_url_para, store.pm_page_name, store.pm_title, store.pm_keywords, store.pm_description,
                   Common.CommonFunction.DateTimeToString(store.pm_created), Common.CommonFunction.DateTimeToString(store.pm_modified),
                   store.pm_modify_user, store.pm_create_user);
                return _access.execCommand(sql.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("PageMetaDataDao-->InsertPageMeta-->" + ex.Message + sql.ToString(), ex);
            }

        }

        public int DeletePageMeta(string pm_ids)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" delete from page_metadata where pm_id in ({0});", pm_ids);
                return _access.execCommand(sql.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("PageMetaDataDao-->DeletePageMeta-->" + ex.Message + sql.ToString(), ex);
            }

        }


    }
}
