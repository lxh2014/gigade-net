/* 
* 文件名稱 :SitePageDao.cs 
* 文件功能描述 :頁面表數據操作 
* 版權宣告 : 
* 開發人員 : shiwei0620j 
* 版本資訊 : 1.0 
* 日期 : 2014/10/14 
* 修改人員 : 
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using BLL.gigade.Mgr;

namespace BLL.gigade.Dao
{
    public class SitePageDao : ISitePageImplDao
    {
        private IDBAccess _accessMySql;
        private GroupAuthMapMgr _groupAuthMapMgr;

        public SitePageDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            _groupAuthMapMgr = new GroupAuthMapMgr(connectionString);
        }

        #region 列表頁 +GetSitePageList(SitePageQuery store, out int totalCount)
        /// <summary>
        /// GetBannerPageList
        /// </summary>
        /// <param name="store">store</param>
        /// <param name="totalCount">totalCount</param>
        /// <returns></returns>
        public List<SitePageQuery> GetSitePageList(SitePageQuery store, out int totalCount)
        {
            store.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            totalCount = 0;
            sqlCount.AppendFormat("select count(sp.page_id) as totalCount ");
            sql.AppendFormat(" select sp.page_id, sp.page_name,sp.page_url,sp.page_status,sp.page_html,sp.page_desc,sp.page_createdate,sp.page_updatedate,sp.create_userid,sp.update_userid");
            sqlFrom.AppendFormat(" from site_page sp ");

            if (!string.IsNullOrEmpty(store.page_name))
            {
                sqlWhere.AppendFormat(" and sp.page_name like N'%{0}%'", store.page_name);
            }
            if (sqlWhere.Length != 0)
            {
                sqlFrom.Append(" WHERE ");
                sqlFrom.Append(sqlWhere.ToString().TrimStart().Remove(0, 3));
            }
            if (store.IsPage)
            {
                DataTable _dt = _accessMySql.getDataTable(sqlCount.ToString() + sqlFrom.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                }
            }
            sqlFrom.AppendFormat(" order by sp.page_id desc limit {0},{1};", store.Start, store.Limit);
            try
            {
                return _accessMySql.getDataTableForObj<SitePageQuery>(sql.ToString() + sqlFrom.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SitePageDao-->GetSitePageList-->" + ex.Message + sql.ToString() + sqlFrom.ToString(), ex);
            }
        }
        #endregion

        #region 查詢站點下的所有網頁+List<BannerPage> GetPage(BannerPage bp)
        /// <summary>
        /// 查詢站點下的所有網頁
        /// </summary>
        /// <param name="bp"></param>
        /// <returns></returns>
        public List<SitePage> GetPage(SitePage bp)
        {
            bp.Replace4MySQL();
            string strSql = string.Format("select page_id,page_name from site_page where  page_status=1 ");

            try
            {
                return _accessMySql.getDataTableForObj<SitePage>(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception("SitePageDao-->GetPage-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 更改狀態+int UpdateStatus(SitePageQuery query)
        public int UpdateStatus(SitePageQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("update site_page set page_status={0},page_updatedate='{1}',update_userid={2} where page_id={3}", query.page_status, CommonFunction.DateTimeToString(query.page_updatedate), query.update_userid, query.page_id);
            try
            {
                int result = _accessMySql.execCommand(sql.ToString());
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("SitePageDao-->UpdateStatus-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 新增頁面+ int Save(SitePageQuery model)
        public int Save(SitePageQuery model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("INSERT INTO site_page(page_name,page_url,page_status,page_html,page_desc,page_createdate,page_updatedate,create_userid,update_userid)");
            sql.AppendFormat(" values('{0}','{1}','{2}',", model.page_name, model.page_url, model.page_status);
            sql.AppendFormat(" '{0}','{1}','{2}','{3}',", model.page_html, model.page_desc, CommonFunction.DateTimeToString(model.page_createdate), CommonFunction.DateTimeToString(model.page_updatedate));
            sql.AppendFormat("'{0}','{1}')", model.create_userid, model.update_userid);
            try
            {
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SitePageDao-->Save-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 頁面編輯+Update(SitePageQuery model)
        public int Update(SitePageQuery model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" UPDATE site_page SET page_name='{0}',page_url='{1}',page_html='{2}',page_desc='{3}',page_updatedate='{4}',update_userid='{5}' WHERE page_id='{6}' ", model.page_name, model.page_url, model.page_html, model.page_desc, CommonFunction.DateTimeToString(model.page_updatedate), model.update_userid, model.page_id);
            try
            {
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SitePageDao-->Update-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
    }
}
