using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using BLL.gigade.Mgr;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
    public class PageAreaDao : IPageAreaImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        string _connStr = string.Empty;
        private GroupAuthMapMgr _groupAuthMapMgr;
        public PageAreaDao(string connectionString)
        {
            _connStr = connectionString;
            _groupAuthMapMgr = new GroupAuthMapMgr(connectionString);
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        #region 查找出列表頁數據+List<Model.Query.BannerAreaQuery> QueryAll(Model.Query.BannerAreaQuery query, out int totalCount)
        public List<Model.Query.PageAreaQuery> QueryAll(Model.Query.PageAreaQuery query, out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                StringBuilder sql = new StringBuilder();
                StringBuilder sqlcount = new StringBuilder();
                StringBuilder sqlfrom = new StringBuilder();
                StringBuilder sqlWhere = new StringBuilder();
                sql.Append(@"select area_id,area_name,area_desc,element_type,area_element_id,show_number,area_status,area_createdate,area_updatedate,ba.create_userid,ba.update_userid ");
                sqlcount.Append("select count(area_id) as totalcounts ");
                sqlfrom.Append(" FROM page_area as ba ");
                
                if (!string.IsNullOrEmpty(query.serchcontent.Trim()))
                {
                    sqlWhere.AppendFormat("  and (area_name like N'%{0}%' or area_desc like N'%{0}%')", query.serchcontent);
                }
               
                if (query.element_type != 0)
                {
                    sqlWhere.AppendFormat(" and element_type='{0}'", query.element_type);
                }

                if (sqlWhere.Length != 0)
                {
                    sqlfrom.Append(" WHERE ");
                    sqlfrom.Append(sqlWhere.ToString().TrimStart().Remove(0, 3));
                }
                totalCount = 0;
                if (query.IsPage)
                {
                    sb.Append(sqlcount.ToString() + sqlfrom.ToString() + ";");
                    System.Data.DataTable _dt = _access.getDataTable(sqlcount.ToString() + sqlfrom.ToString());

                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalcounts"]);
                    }

                    sqlfrom.AppendFormat(" ORDER BY ba.area_id DESC ");
                    sqlfrom.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                sb.Append(sql.ToString() + sqlfrom.ToString());
                return _access.getDataTableForObj<PageAreaQuery>(sql.ToString() + sqlfrom.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BannerAreaDao-->QueryAll-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 更新表page_area+int Update(Model.BannerArea model)
        public int Update(Model.PageArea model)
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"update page_area  set 
              area_name='{0}',
               area_desc='{1}',area_element_id='{2}',
               area_updatedate='{3}',
                update_userid='{4}',show_number='{5}',element_type='{6}' where area_id={7}",
               model.area_name,
               model.area_desc, model.area_element_id,
                model.area_updatedate.ToString("yyyy-MM-dd HH:mm:ss"),
               model.update_userid, model.show_number, model.element_type, model.area_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BannerAreaDao.Update-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion


        #region 根據某個id獲取該id下的數據+BannerArea GetModel(BannerArea model)
        public PageArea GetModel(PageArea model)
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"select area_id,area_name,area_desc,element_type,area_element_id,show_number,area_status,area_createdate,area_updatedate,create_userid,update_userid ");
                sb.AppendFormat(@"from page_area where area_id ={0}", model.area_id);
                return _access.getSinggleObj<PageArea>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BannerArea.GetModel-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        public List<PageArea> GetArea()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select area_id,area_name,element_type from page_area where area_status=1 ");
                return _access.getDataTableForObj<PageArea>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BannerAreaDao-->GetArea-->" + ex.Message + sql.ToString(), ex);
            }

        }

        #region 保存新增區域數據+int AreaSave(BannerArea ba)
        /// <summary>
        /// 保存新增區域數據
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        public int AreaSave(PageArea ba)
        {
            ba.Replace4MySQL();
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("insert into page_area (area_name,area_status,area_createdate,area_updatedate,update_userid,create_userid,area_desc,area_element_id,show_number,element_type)value('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", ba.area_name, ba.area_status, ba.area_createdate.ToString("yyyy-MM-dd HH:mm:ss"), ba.area_updatedate.ToString("yyyy-MM-dd HH:mm:ss"), ba.update_userid, ba.create_userid, ba.area_desc, ba.area_element_id, ba.show_number, ba.element_type);
                return _access.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("BannerArea.Dao-->AreaSave-->" + ex.Message + strSql.ToString(), ex);
            }

        }
        #endregion

        #region 查詢一條區域信息+BannerArea GetBannerByAreaId(int areaId)
        /// <summary>
        /// 查詢一條區域信息
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public PageAreaQuery GetBannerByAreaId(int areaId)
        {
            string strSql = string.Empty;
            try
            {
                strSql = string.Format("select area_id,area_name,show_number,area_status,area_desc,area_element_id from page_area a where  area_id='{0}'", areaId);
                return _access.getSinggleObj<PageAreaQuery>(strSql);
            }
            catch (Exception ex)
            {

                throw new Exception("BannerArea.Dao-->GetBannerByAreaId-->" + ex.Message + strSql.ToString(), ex);
            }

        }
        #endregion


        public int UpPageAreaStatus(PageArea model)
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("update page_area set area_status={0},area_updatedate='{1}',update_userid={2} where area_id={3} ", model.area_status, CommonFunction.DateTimeToString(model.area_updatedate), model.update_userid, model.area_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PageAreaDao.UpPageAreaStatus-->" + ex.Message + sb.ToString(), ex);
            }

        }
    }
}
