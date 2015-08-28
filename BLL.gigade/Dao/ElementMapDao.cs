using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class ElementMapDao : IElementMapImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public ElementMapDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }
        #region 查詢元素關係列表+ List<ElementMapQuery> GetElementMapList(ElementMapQuery query, out int totalCount)
        /// <summary>
        /// 查詢元素關係列表
        /// </summary>
        /// <param name="query">實體</param>
        /// <param name="totalCount">分頁</param>
        /// <returns>List<ElementMapQuery>集合</returns>
        public List<ElementMapQuery> GetElementMapList(ElementMapQuery query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            sb.Append(@" select map_id,em.site_id,s.site_name,em.page_id,sp.page_name,em.area_id,pa.area_name,ap.packet_id,ap.packet_name,em.sort,create_date,update_date,mu.user_username as create_user_name");
            sb.Append("  from element_map em left join site s on s.site_id=em.site_id left join site_page sp on sp.page_id=em.page_id left join page_area pa on em.area_id=pa.area_id ");
            sb.Append(" left join area_packet ap on em.packet_id=ap.packet_id left join manage_user mu on em.update_user_id=mu.user_id  ");

            if (!string.IsNullOrEmpty(query.site_name))
            {
                sqlWhere.AppendFormat(" and s.site_name like  N'%{0}%'  ", query.site_name);
            }
            if (query.element_type != 0)
            {
                sqlWhere.AppendFormat(" and ap.element_type='{0}'  ", query.element_type);
            }
            if (sqlWhere.Length != 0)
            {
                sb.Append(" WHERE ");
                sb.Append(sqlWhere.ToString().TrimStart().Remove(0, 3));
            }

            try
            {
                totalCount = 0;
                if (query.IsPage)
                {

                    System.Data.DataTable _dt = _access.getDataTable(sb.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }

                    sb.Append(" order by map_id desc");
                    sb.AppendFormat("  limit {0},{1}", query.Start, query.Limit);
                }
                return _access.getDataTableForObj<ElementMapQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ElementMapDao-->GetElementMapList-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 元素關係新增+ int AddElementMap(ElementMapQuery query)
        public int AddElementMap(ElementMapQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@" insert into element_map(site_id,page_id,area_id,packet_id,sort,create_date,create_user_id,update_date,update_user_id)VALUES('{0}','{1}','{2}','{3}','{4}',", query.site_id, query.page_id, query.area_id, query.packet_id, query.sort);
                sb.AppendFormat(" '{0}','{1}','{2}','{3}')", query.create_date.ToString("yyyy-MM-dd HH:mm:ss"), query.create_user_id, query.update_date.ToString("yyyy-MM-dd HH:mm:ss"), query.update_user_id);
                return _access.execCommand(sb.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("ElementMapDao-->AddElementMap-->" + ex.Message + sb.ToString(), ex);
            }

        }
        #endregion

        #region 元素關係修改+ int upElementMap(ElementMapQuery query)
        public int upElementMap(ElementMapQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@" update element_map set site_id='{0}',page_id='{1}',area_id='{2}',packet_id='{3}',sort='{4}', ", query.site_id, query.page_id, query.area_id, query.packet_id, query.sort);
                sb.AppendFormat(" update_date='{0}',update_user_id='{1}'  where map_id='{2}'", query.update_date.ToString("yyyy-MM-dd HH:mm:ss"), query.update_user_id, query.map_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ElementMapDao-->upElementMap-->" + ex.Message + sb.ToString(), ex);
            }

        }
        #endregion

        #region 判斷是否重複添加+bool SelectElementMap(ElementMapQuery query)
        /// <summary>
        /// 判斷是否重複添加
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool SelectElementMap(ElementMapQuery query)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("select map_id,site_id,page_id,area_id,packet_id,sort,create_date,create_user_id,update_date,update_user_id from element_map ");
                strSql.AppendFormat(" where site_id='{0}' and page_id='{1}' and area_id='{2}' and packet_id='{3}'", query.site_id, query.page_id, query.area_id, query.packet_id);
                ElementMapQuery model = _access.getSinggleObj<ElementMapQuery>(strSql.ToString());
                if (model == null)
                {
                    return true;//代表沒有重複添加
                }
                else if (model.map_id == query.map_id && query.map_id != 0)
                {
                    return true;
                }
                else
                {
                    return false;//代表有重複添加
                }

            }
            catch (Exception ex)
            {

                throw new Exception("ElementMapDao-->SelectElementMap-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        public bool GetAreaCount(int areaId)
        {
            StringBuilder sqlElementM = new StringBuilder();
            StringBuilder sqlPageArea = new StringBuilder();
            DataTable _Edt = new DataTable();
            DataTable _Pdt = new DataTable();
            try
            {
                sqlElementM.AppendFormat("select count(map_id) as totalCount from element_map where area_id={0};", areaId);
                sqlPageArea.AppendFormat("select show_number  from page_area where area_id={0};", areaId);
                _Edt = _access.getDataTable(sqlElementM.ToString());
                _Pdt = _access.getDataTable(sqlPageArea.ToString());
                if (Convert.ToInt32(_Edt.Rows[0]["totalCount"]) >= Convert.ToInt32(_Pdt.Rows[0]["show_number"]))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                string sql = sqlElementM.ToString() + sqlPageArea.ToString();
                throw new Exception("ElementMapDao-->GetAreaCount-->" + sql.ToString() + ex.Message, ex);
            }


        }
    }
}
