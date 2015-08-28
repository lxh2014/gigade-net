using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using System.Data;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using BLL.gigade.Common;
using MySql.Data.MySqlClient;
namespace BLL.gigade.Dao
{
    public class WebContentType7Dao : IWebContentType7ImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        string _connStr = string.Empty;
        private string connStr;
        public WebContentType7Dao(string connectionString)
        {
            _connStr = connectionString;
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        public int Insert(Model.WebContentType7 model)
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                if (model.content_status == 1)//啟用
                {
                    WebContentTypeSetupDao _setDao = new WebContentTypeSetupDao(_connStr);
                    WebContentTypeSetup smodel = new WebContentTypeSetup();
                    smodel.site_id = model.site_id;
                    smodel.page_id = model.page_id;
                    smodel.area_id = model.area_id;
                    smodel.web_content_type = "web_content_type7";
                    _setDao.UpdateLimitStatus(smodel);////當前已啟用的個數超過5筆時，使最舊的不啟用，
                }
              
                sb.AppendFormat(@"INSERT INTO web_content_type7(site_id,page_id,area_id,`type_id`,`home_title`,`home_text`,`content_title`,`content_html`,`content_image`,`content_default`,`content_status`,`link_url`,`link_mode`,`update_on`,`created_on`,`keywords`,`sort`,`start_time`,`end_time`) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}');select @@Identity; ", model.site_id, model.page_id, model.area_id, model.type_id, model.home_title, model.home_text, model.content_title, model.content_html, model.content_image, model.content_default, model.content_status, model.link_url, model.link_mode, CommonFunction.DateTimeToString(model.update_on), CommonFunction.DateTimeToString(model.created_on), model.keywords, model.sort, CommonFunction.DateTimeToString(model.start_time), CommonFunction.DateTimeToString(model.end_time));
                DataTable dt = _access.getDataTable(sb.ToString());
                model.content_id = Convert.ToInt32(dt.Rows[0][0].ToString());
                return model.content_id;
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType7Dao.Insert-->" + ex.Message + sb.ToString(), ex); 
            }
         
        }
        //public int Update2(Model.WebContentType7 model)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    model.link_url = model.link_url + "?content_id=" + model.content_id;
        //    sb.AppendFormat(@"update web_content_type7  set link_url='{0}' where content_id={1}", model.link_url, model.content_id);
        //    return _access.execCommand(sb.ToString());
        //}
        public int Update(Model.WebContentType7 model)
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                WebContentType7 oldModel = GetModel(model);
                if (model.content_status == 1 && oldModel.content_status != 1)//啟用
                {
                    WebContentTypeSetupDao _setDao = new WebContentTypeSetupDao(_connStr);
                    WebContentTypeSetup smodel = new WebContentTypeSetup();
                    smodel.site_id = model.site_id;
                    smodel.page_id = model.page_id;
                    smodel.area_id = model.area_id;
                    smodel.web_content_type = "web_content_type7";
                    _setDao.UpdateLimitStatus(smodel);////當前已啟用的個數超過5筆時，使最舊的不啟用，
                }
              
                sb.AppendFormat(@"update web_content_type7 set site_id='{0}',page_id='{1}',area_id='{2}',type_id='{3}',content_title='{4}',home_title='{5}',`content_html`='{6}',home_text='{7}',content_default='{8}',content_status='{9}',link_url='{10}',link_mode='{11}',update_on='{12}',created_on='{13}',content_image='{14}',keywords='{15}',sort='{17}',start_time='{18}',end_time='{19}' where content_id={16}",
                    model.site_id, model.page_id, model.area_id, model.type_id, model.content_title, model.home_title, model.content_html, model.home_text, model.content_default, model.content_status, model.link_url, model.link_mode, CommonFunction.DateTimeToString(model.update_on), CommonFunction.DateTimeToString(model.created_on), model.content_image, model.keywords, model.content_id, model.sort, CommonFunction.DateTimeToString(model.start_time), CommonFunction.DateTimeToString(model.end_time));
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType7Dao.Update-->" + ex.Message + sb.ToString(), ex); 
            }
          
        }
        public List<Model.Query.WebContentType7Query> QueryAll(Model.Query.WebContentType7Query query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder str = new StringBuilder();
            StringBuilder strcondition = new StringBuilder();
            try
            {
                str.Append(@"select w7.* , t_site.parameterName as site_name,t_page.parameterName as page_name,t_area.parameterName as area_name from web_content_type7 w7 ");
                str.AppendFormat(" left join t_parametersrc t_site on t_site.parameterType='site_id' and t_site.parameterCode=w7.site_id");
                str.AppendFormat(" left join t_parametersrc t_page on t_page.parameterType='page_id' and t_page.parameterCode=w7.page_id and t_page.topValue=t_site.rowid");
                str.AppendFormat(" left join t_parametersrc t_area on t_area.parameterType='area_id' and t_area.parameterCode=w7.area_id and t_area.topValue=t_page.rowid");

                if (!string.IsNullOrEmpty(query.serchchoose.ToString()))
                {
                    if (query.serchchoose == 5)
                    {
                        if (!string.IsNullOrEmpty(query.serchwhere.Trim()))
                        {
                            strcondition.AppendFormat(" where  page_id=5 and (content_title like '%{0}%' or content_html like '%{0}%')", query.serchwhere);
                        }
                        else
                        {
                            strcondition.AppendFormat(" where 1=1 and page_id=5 ");
                        }
                    }
                    else if (query.serchchoose == 6)
                    {
                        if (!string.IsNullOrEmpty(query.serchwhere.Trim()))
                        {
                            strcondition.AppendFormat(" where 1=1 and page_id=6 and (content_title like '%{0}%' or content_html like '%{0}%')", query.serchwhere);
                        }
                        else
                        {
                            strcondition.AppendFormat(" where 1=1 and page_id=6 ");
                        }
                    }
                    else if (query.serchchoose == 0)
                    {
                        if (!string.IsNullOrEmpty(query.serchwhere.Trim()))
                        {
                            strcondition.AppendFormat(" where 1=1 and (content_title like '%{0}%' or content_html like '%{0}%')", query.serchwhere);
                        }
                        else
                        {
                            strcondition.AppendFormat(" where 1=1 ");
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(query.serchwhere.Trim()))
                    {
                        strcondition.AppendFormat(" where 1=1 and (content_title like '%{0}%' or content_html like '%{0}%')", query.serchwhere);
                    }
                    else
                    {
                        strcondition.AppendFormat(" where 1=1");
                    }
                }
                strcondition.AppendFormat("  order by content_id desc ");
                totalCount = 0;

                if (query.IsPage)
                {

                    DataTable dt = _access.getDataTable(@"SELECT count(*) as totalcount from web_content_type7 " + strcondition);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalcount"]);
                    }
                    strcondition.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                string strs = str.ToString() + strcondition.ToString();
                return _access.getDataTableForObj<WebContentType7Query>(strs.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType7Dao.QueryAll-->" + ex.Message + str.ToString(), ex); 
            }
            
        }
        /// <summary>
        /// 獲取已啟用的個數
        /// </summary>
        /// <returns></returns>
        public int GetDefault(WebContentType7 model)
        {
            StringBuilder sql = new StringBuilder("");
            try
            {
                sql.AppendFormat(@" select count(*) as countSta from web_content_type7 where content_status=1 and  pag_id='{0}';", model.page_id);
                return Convert.ToInt32(_access.getDataTable(sql.ToString()).Rows[0]["countSta"].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType7Dao.GetDefault-->" + ex.Message + sql.ToString(), ex); 
            }
           
        }
        public Model.WebContentType7 GetModel(Model.WebContentType7 model)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"select * from web_content_type7 where content_id ={0}", model.content_id);
                return _access.getSinggleObj<WebContentType7>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType6Dao.GetModel-->" + ex.Message + sb.ToString(), ex); 
            }
           
        }
        public int delete(Model.WebContentType7 model)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"update web_content_type7 set content_status=0 where content_id='{0}'", model.content_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType6Dao.delete-->" + ex.Message + sb.ToString(), ex); 
            }
           
        }
    }
}
