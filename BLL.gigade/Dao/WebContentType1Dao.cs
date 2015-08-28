using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using System.Data;
using BLL.gigade.Common;
using BLL.gigade.Model;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao
{
    public class WebContentType1Dao : IWebContentType1ImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        string _connStr = string.Empty;
        public WebContentType1Dao(string connectionString)
        {
            _connStr = connectionString;
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public int Insert(Model.WebContentType1 model)
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
                        smodel.web_content_type = "web_content_type1";
                        _setDao.UpdateLimitStatus(smodel);////當前已啟用的個數超過5筆時，使最舊的不啟用，
                    }
                  
                    sb.AppendFormat(@"insert into web_content_type1(site_id,page_id,area_id,type_id,content_title,content_image,content_default,content_status,link_url,link_page,link_mode,update_on,created_on) 
                values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')", model.site_id, model.page_id, model.area_id, model.type_id, model.content_title, model.content_image, model.content_default, model.content_status, model.link_url, model.link_page, model.link_mode, CommonFunction.DateTimeToString(model.update_on), CommonFunction.DateTimeToString(model.created_on));
                    return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType1Dao.Insert-->" + ex.Message + sb.ToString(), ex);
            }
           
        }

        public int Update(Model.WebContentType1 model)
        {
            model.Replace4MySQL(); 
            StringBuilder sb = new StringBuilder();
            try
            {
                         WebContentType1 oldModel = GetModel(model);
                        if (model.content_status == 1 && oldModel.content_status != 1)//啟用
                        {
                            WebContentTypeSetupDao _setDao = new WebContentTypeSetupDao(_connStr);
                            WebContentTypeSetup smodel = new WebContentTypeSetup();
                            smodel.site_id = model.site_id;
                            smodel.page_id = model.page_id;
                            smodel.area_id = model.area_id;
                            smodel.web_content_type = "web_content_type1";
                            _setDao.UpdateLimitStatus(smodel);////當前已啟用的個數超過5筆時，使最舊的不啟用，
                        }
                       
                        sb.AppendFormat(@"update web_content_type1  set site_id='{0}',page_id='{1}',area_id='{2}',type_id='{3}',content_title='{4}',content_image='{5}',`content_default`='{6}',content_status='{7}',link_url='{8}',link_page='{9}',link_mode='{10}',update_on='{11}' where content_id={12}",
                            model.site_id, model.page_id, model.area_id, model.type_id, model.content_title, model.content_image, model.content_default, model.content_status, model.link_url, model.link_page, model.link_mode, CommonFunction.DateTimeToString(model.update_on), model.content_id);
                        return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("WebContentType1Dao.Update-->" + ex.Message + sb.ToString(), ex);
            }
          
        }

        public List<Model.Query.WebContentType1Query> QueryAll(Model.Query.WebContentType1Query query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder str = new StringBuilder();
            StringBuilder strcondition = new StringBuilder();
            try
            {
                    str.Append(@"select w1.* , t_site.parameterName as site_name,t_page.parameterName as page_name,t_area.parameterName as area_name from web_content_type1 w1 ");
                    str.AppendFormat(" left join t_parametersrc t_site on t_site.parameterType='site_id' and t_site.parameterCode=w1.site_id");
                    str.AppendFormat(" left join t_parametersrc t_page on t_page.parameterType='page_id' and t_page.parameterCode=w1.page_id and t_page.topValue=t_site.rowid");
                    str.AppendFormat(" left join t_parametersrc t_area on t_area.parameterType='area_id' and t_area.parameterCode=w1.area_id and t_area.topValue=t_page.rowid");
                    strcondition.AppendFormat(" where 1=1 order by content_id desc ");
                    totalCount = 0;

                    if (query.IsPage)
                    {
                        DataTable dt = _access.getDataTable(@"SELECT count(*) as totalcount from web_content_type1 ");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            totalCount = Convert.ToInt32(dt.Rows[0]["totalcount"]);

                        }
                        strcondition.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                    }
                    string strs = str.ToString() + strcondition.ToString();
                    return _access.getDataTableForObj<WebContentType1Query>(strs.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType1Dao.QueryAll-->" + ex.Message + str.ToString(), ex);
            }
            
        }

        public WebContentType1 GetModel(WebContentType1 model)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"select * from web_content_type1 where content_id ={0}", model.content_id);
                return _access.getSinggleObj<WebContentType1>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType1Dao.GetModel-->" + ex.Message + sb.ToString(), ex);
            }
            
        }
        /// <summary>
        /// 獲取已啟用的個數
        /// </summary>
        /// <returns></returns>
        public int GetDefault(WebContentType1 model)
        {
            StringBuilder sql = new StringBuilder("");
            try
            {
                sql.AppendFormat(@" select count(*) as countSta from web_content_type1 where content_status=1 and  pag_id='{0}';", model.page_id);
                return Convert.ToInt32(_access.getDataTable(sql.ToString()).Rows[0]["countSta"].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType1Dao.GetDefault-->" + ex.Message + sql.ToString(), ex);
            }
           
        }

        public int delete(WebContentType1 model)
        {

            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"update web_content_type1  set content_status=0 where content_id='{0}'",
           model.content_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType1Dao.delete-->" + ex.Message + sb.ToString(), ex);
            }
        }

    }

}
