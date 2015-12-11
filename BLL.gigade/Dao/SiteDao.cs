using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Mgr;
namespace BLL.gigade.Dao
{
    public class SiteDao : Impl.ISiteImplDao
    {
        IDBAccess _access;
        private GroupAuthMapMgr _groupAuthMapMgr;
        public SiteDao(string connectionString)
        {
            _groupAuthMapMgr = new GroupAuthMapMgr(connectionString);
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public List<Site> Query(Site query)
        {
            StringBuilder stb = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                stb.Append("select site_id,site_name,domain,cart_delivery from site where 1=1 ");
                if (!string.IsNullOrEmpty(query.Site_Name))
                {
                    stb.AppendFormat(" and site_name like N'%{0}%'", query.Site_Name);
                }
                if (query.Site_Id != 0)
                {
                    stb.AppendFormat(" and site_id={0}", query.Site_Id);
                }
                return _access.getDataTableForObj<Site>(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SiteDao-->Query-->" + ex.Message + stb.ToString(), ex);
            }
        }

        #region site站臺方法
        /// <summary>
        /// chaojie_zz添加於2014/10/14
        /// </summary>
        /// <param name="site"></param>
        /// <param name="totalCount"></param>


        #region site站臺列表
        /// <summary>
        /// site站臺列表
        /// </summary>
        /// <param name="site"></param>
        /// <param name="totalCount"></param>
        /// <returns>List</returns>
        public List<SiteQuery> QuerryAll(SiteQuery site, out int totalCount)
        {
            StringBuilder stb = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            site.Replace4MySQL();
            try
            {
                sqlCondi.AppendFormat(@"select site.site_id,site.site_name,site.domain,site.cart_delivery,st.site_name as csitename,site.online_user,");
                sqlCondi.AppendFormat(@"site.max_user,site.page_location,site.site_status,site.site_createdate,site.site_updatedate,site.create_userid,site.update_userid ");
                stb.AppendFormat(@"from site  left join site st on site.cart_delivery=st.site_id ");

                GroupAuthMapQuery groupAuthMapModel = new GroupAuthMapQuery();
                groupAuthMapModel.table_name = "site";
                groupAuthMapModel.user_id = site.create_userid;
                stb.Append(_groupAuthMapMgr.Query(groupAuthMapModel));

                //if (site.site_id != 0)
                //{
                //    strcondition.AppendFormat(@" and site.site_id='{0}' ", site.site_id);
                //}
                if (!string.IsNullOrEmpty(site.site_name))
                {
                    sqlWhere.AppendFormat(" and site.site_name like N'%{0}%' ", site.site_name);
                }
                totalCount = 0;
                if (sqlWhere.Length != 0)
                {
                    stb.Append(" WHERE ");
                    stb.Append(sqlWhere.ToString().TrimStart().Remove(0, 3));
                }
                if (site.IsPage)
                {
                    System.Data.DataTable _dt = _access.getDataTable(" select  count(site.site_id) as totalcount " + stb.ToString());

                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalcount"]);
                    }
                    stb.AppendFormat(" order by site.site_id desc limit {0},{1}", site.Start, site.Limit);
                }
                return _access.getDataTableForObj<SiteQuery>(sqlCondi.ToString() + stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SiteDao-->QuerryAll-->" + ex.Message + sqlCondi.ToString() + stb.ToString(), ex);
            }

        }
        #endregion

        #region 添加站臺信息
        /// <summary>
        /// 添加站臺信息
        /// </summary>
        /// <param name="site"></param>
        /// <returns>int</returns>
        public int InsertSite(SiteModel site)
        {
            site.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"insert into site(site_name,domain,cart_delivery,online_user");
                sb.AppendFormat(@",max_user,page_location,site_status,site_createdate,site_updatedate,");
                sb.AppendFormat(@"create_userid,update_userid)VALUES(");
                sb.AppendFormat(@"'{0}','{1}','{2}','{3}',", site.site_name, site.domain, site.cart_delivery, site.online_user);
                sb.AppendFormat(@"'{0}','{1}','{2}','{3}'", site.max_user, site.page_location, site.site_status, site.site_createdate.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.AppendFormat(@",'{0}','{1}','{2}')", site.site_updatedate.ToString("yyyy-MM-dd HH:mm:ss"), site.create_userid, site.update_userid);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SiteDao-->InsertSite-->" + ex.Message + sb.ToString(), ex);
            }


        }
        #endregion

        #region 更改站臺信息
        public int UpSite(SiteModel site)
        {
            site.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"update site SET site_name='{0}',domain='{1}',cart_delivery='{2}'", site.site_name, site.domain, site.cart_delivery);
                sb.AppendFormat(@",max_user='{0}',page_location='{1}'", site.max_user, site.page_location);
                sb.AppendFormat(",site_updatedate='{0}',update_userid='{1}'", site.site_updatedate.ToString("yyyy-MM-dd HH:mm:ss"), site.update_userid);
                sb.AppendFormat("  where site_id='{0}'", site.site_id);

                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SiteDao-->UpSite-->" + ex.Message + sb.ToString(), ex);
            }


        }
        #endregion

        #region 更改站臺狀態
        public int UpSiteStatus(SiteModel site)
        {
            site.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"update site set site_status='{0}',site_updatedate='{1}'", site.site_status, site.site_updatedate.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.AppendFormat(@",update_userid='{0}' where site_id='{1}'", site.update_userid, site.site_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SiteDao-->UpSiteStatus-->" + ex.Message + sb.ToString(), ex);
            }

        }
        #endregion

        #endregion

        public List<Site> GetSite(Site query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select site_id,site_name from site where site_status=1 ");
            GroupAuthMapQuery groupAuthMapModel = new GroupAuthMapQuery();
            groupAuthMapModel.table_name = "site";
            groupAuthMapModel.user_id = query.Create_Userid;
            sql.Append(_groupAuthMapMgr.Query(groupAuthMapModel));
            try
            {
                return _access.getDataTableForObj<Site>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SiteDao-->GetSite-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public Site GetSiteInfo(int ids)
        {
            StringBuilder stb = new StringBuilder();
            try
            {
                stb.AppendFormat("select site_id,site_name from site where site_id ={0}", ids);
                return _access.getSinggleObj <Site>(stb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SiteDao-->GetSiteInfo-->" + ex.Message + stb.ToString(), ex);
            }
        }
    }
}
