#region 文件信息
/* 
* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：InfoMapDao.cs      
* 摘 要：                                                                               
* 臺新專區關係設定
* 当前版本：v1.0                                                                 
* 作 者： shuangshuang0420j                                           
* 完成日期：2014/12/05 
* 修改歷史：                                                                     
*        
*/

#endregion
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
    public class InfoMapDao : IInfoMapImplDao
    {
        private IDBAccess _access;

        public InfoMapDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);

        }

        #region 查詢臺新專區元素關係列表+  public List<InfoMapQuery> GetInfoMapList(InfoMapQuery query, out int totalCount)
        /// <summary>
        /// 查詢元素關係列表
        /// </summary>
        /// <param name="query">實體</param>
        /// <param name="totalCount">分頁</param>
        /// <returns>List<InfoMapQuery>集合</returns>
        public List<InfoMapQuery> GetInfoMapList(InfoMapQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            StringBuilder sbCondi = new StringBuilder();
            StringBuilder sbCount = new StringBuilder();
            List<InfoMapQuery> store = new List<InfoMapQuery>();
            try
            {
                sb.Append(@" select im.map_id,im.site_id,s.site_name,im.page_id,sp.page_name,im.area_id,pa.area_name,im.info_id,im.type,im.sort,create_date,update_date,mu.user_username as update_user_name");
                sbCondi.Append("  from info_map im ");
                sbCondi.Append(" left join site s on s.site_id=im.site_id ");
                sbCondi.Append(" left join site_page sp on sp.page_id=im.page_id ");
                sbCondi.Append(" left join page_area pa on im.area_id=pa.area_id ");
                sbCondi.Append(" left join manage_user mu on im.update_user_id=mu.user_id where 1=1 ");
                sbCount.Append(" select count(im.map_id) as totalCount  ");
                if (!string.IsNullOrEmpty(query.site_name))
                {
                    sbCondi.AppendFormat(" and s.site_name like  N'%{0}%'  ", query.site_name);
                }
                if (query.type != 0)
                {
                    sbCondi.AppendFormat(" and im.type='{0}'  ", query.type);
                }
                sbCondi.Append(" order by map_id desc");
                totalCount = 0;
                if (query.IsPage)
                {
                    sbCount.Append(sbCondi.ToString());
                    DataTable dtcount = new DataTable();
                    dtcount = _access.getDataTable(sbCount.ToString());
                    if (dtcount != null && dtcount.Rows.Count > 0)
                    {
                        totalCount =Convert.ToInt32(dtcount.Rows[0]["totalCount"]);
                    }
                }
                sbCondi.AppendFormat("  limit {0},{1}", query.Start, query.Limit);
                sb.Append(sbCondi.ToString());
                store = _access.getDataTableForObj<InfoMapQuery>(sb.ToString());
                foreach (var item in store)
                {
                    sql.Clear();
                    switch (item.type)//類型 1：活動頁面 2：最新消息 3：訊息公告 4：電子報',
                    {
                        case 1:
                            sql.AppendFormat("select tb.epaper_title as info_name from epaper_content tb where tb.epaper_id='{0}'", item.info_id);
                            break;
                        case 2:
                            sql.AppendFormat("select tb.news_title as info_name from news_content tb where tb.news_id='{0}'", item.info_id);
                            break;
                        case 3:
                            sql.AppendFormat("select tb.title as info_name from announce tb where tb.announce_id='{0}'", item.info_id);
                            break;
                        case 4:
                            sql.AppendFormat("select tb.content_title as info_name from edm_content tb where tb.content_id='{0}'", item.info_id);
                            break;
                    }
                    System.Data.DataTable _dt = _access.getDataTable(sql.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        item.info_name = _dt.Rows[0]["info_name"].ToString();
                    }
                }
                return store;
            }
            catch (Exception ex)
            {
                throw new Exception("InfoMapDao-->GetInfoMapList-->" + ex.Message + sb.ToString() + sbCount.ToString()+sql.ToString(), ex);
            }
        }
        #endregion

        #region 元素關係新增+ int SaveInfoMap(InfoMapQuery query)
        public int SaveInfoMap(InfoMapQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@" insert into info_map(site_id,page_id,area_id,type,info_id,sort,create_date,create_user_id,update_date,update_user_id)VALUES('{0}','{1}','{2}','{3}','{4}','{5}',", query.site_id, query.page_id, query.area_id, query.type, query.info_id, query.sort);
                sb.AppendFormat(" '{0}','{1}','{2}','{3}')", query.create_date.ToString("yyyy-MM-dd HH:mm:ss"), query.create_user_id, query.update_date.ToString("yyyy-MM-dd HH:mm:ss"), query.update_user_id);
                return _access.execCommand(sb.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("InfoMapDao-->SaveInfoMap-->" + ex.Message + sb.ToString(), ex);
            }

        }
        #endregion

        #region 元素關係修改+ int UpdateInfoMap(InfoMapQuery query)
        public int UpdateInfoMap(InfoMapQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@" update info_map set site_id='{0}',page_id='{1}',area_id='{2}',type='{3}',sort='{4}',info_id='{5}', ", query.site_id, query.page_id, query.area_id, query.type, query.sort, query.info_id);
                sb.AppendFormat(" update_date='{0}',update_user_id='{1}'  where map_id='{2}'", query.update_date.ToString("yyyy-MM-dd HH:mm:ss"), query.update_user_id, query.map_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InfoMapDao-->UpdateInfoMap-->" + ex.Message + sb.ToString(), ex);
            }

        }
        #endregion

        #region 判斷是否重複添加+bool SelectInfoMap(InfoMapQuery query)
        /// <summary>
        /// 判斷是否重複添加
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool SelectInfoMap(InfoMapQuery query)
        {
            string strSql = string.Empty;
            try
            {
                strSql = string.Format("select * from info_map where site_id='{0}' and page_id='{1}' and area_id='{2}' and type='{3}' and info_id='{4}'", query.site_id, query.page_id, query.area_id, query.type, query.info_id);
                InfoMapQuery model = _access.getSinggleObj<InfoMapQuery>(strSql);
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

                throw new Exception("InfoMapDao-->SelectInfoMap-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 根據id獲取整個model值
        public InfoMapQuery GetOldModel(InfoMapQuery query)
        {
            StringBuilder sql = new StringBuilder();


            try
            {
                sql.AppendFormat("select map_id, site_id,page_id,area_id,type,info_id,sort,create_date,create_user_id,update_date,update_user_id from info_map where map_id='{0}'", query.map_id);

                return _access.getSinggleObj<InfoMapQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InfoMapDao-->GetOldModel-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

    }
}
