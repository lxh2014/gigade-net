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
namespace BLL.gigade.Dao
{
    public class WebContentType3Dao : IWebContentType3ImplDao
    {

        private IDBAccess _access;
        string strSql = string.Empty;
        string _connStr = string.Empty;
        public WebContentType3Dao(string connectionString)
        {
            _connStr = connectionString;
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public int Insert(Model.WebContentType3 model)
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
                    smodel.web_content_type = "web_content_type3";
                    _setDao.UpdateLimitStatus(smodel);////當前已啟用的個數超過5筆時，使最舊的不啟用，
                }
            
                sb.AppendFormat(@"INSERT INTO web_content_type3(`site_id`,`page_id`,`area_id`,`type_id`,`content_title`,`content_image`,`brand_id`,`content_default`,`content_status`,`link_url`,`link_page`,`link_mode`,`update_on`,`created_on`) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}') ", model.site_id, model.page_id, model.area_id, model.type_id, model.content_title, model.content_image, model.brand_id, model.content_default, model.content_status, model.link_url, model.link_page, model.link_mode, CommonFunction.DateTimeToString(model.update_on), CommonFunction.DateTimeToString(model.created_on));
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType3Dao.Insert-->" + ex.Message + sb.ToString(), ex);
            }
           
        }

        public int Update(Model.WebContentType3 model)
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                WebContentType3 oldModel = GetModel(model);
                if (model.content_status == 1 && oldModel.content_status != 1)//啟用
                {
                    WebContentTypeSetupDao _setDao = new WebContentTypeSetupDao(_connStr);
                    WebContentTypeSetup smodel = new WebContentTypeSetup();
                    smodel.site_id = model.site_id;
                    smodel.page_id = model.page_id;
                    smodel.area_id = model.area_id;
                    smodel.web_content_type = "web_content_type3";
                    _setDao.UpdateLimitStatus(smodel);////當前已啟用的個數超過5筆時，使最舊的不啟用，
                }

                sb.AppendFormat(@"update web_content_type3  set site_id='{0}',page_id='{1}',area_id='{2}',type_id='{3}',content_title='{4}',content_image='{5}',`content_default`='{6}',content_status='{7}',link_url='{8}',link_page='{9}',link_mode='{10}',update_on='{11}',brand_id='{12}' where content_id={13}",
                 model.site_id, model.page_id, model.area_id, model.type_id, model.content_title, model.content_image, model.content_default, model.content_status, model.link_url, model.link_page, model.link_mode, CommonFunction.DateTimeToString(model.update_on), model.brand_id, model.content_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType3Dao.Update-->" + ex.Message + sb.ToString(), ex);
            }

          
        }

        public List<Model.Query.WebContentType3Query> QueryAll(Model.Query.WebContentType3Query query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder str = new StringBuilder();
            StringBuilder strcondition = new StringBuilder();
            try
            {
                str.Append(@"select w3.* , t_site.parameterName as site_name,t_page.parameterName as page_name,t_area.parameterName as area_name,v_b.brand_name as brand_name from web_content_type3 w3 ");
                str.AppendFormat(" left join t_parametersrc t_site on t_site.parameterType='site_id' and t_site.parameterCode=w3.site_id");
                str.AppendFormat(" left join t_parametersrc t_page on t_page.parameterType='page_id' and t_page.parameterCode=w3.page_id and t_page.topValue=t_site.rowid");
                str.AppendFormat(" left join t_parametersrc t_area on t_area.parameterType='area_id' and t_area.parameterCode=w3.area_id and t_area.topValue=t_page.rowid");
                str.AppendFormat(" left join vendor_brand v_b on v_b.brand_id=w3.brand_id");
                strcondition.AppendFormat(" where 1=1 order by content_id desc ");
                totalCount = 0;

                if (query.IsPage)
                {
                    DataTable dt = _access.getDataTable(@"SELECT count(*) as totalcount from web_content_type3 where 1=1 ");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalcount"]);
                    }
                    strcondition.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                string strs = str.ToString() + strcondition.ToString();
                return _access.getDataTableForObj<WebContentType3Query>(strs.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType3Dao.QueryAll-->" + ex.Message + str.ToString(), ex); ;
            }
           
        }

        /// <summary>
        /// 獲取已啟用的個數
        /// </summary>
        /// <returns></returns>
        public int GetDefault(WebContentType3 model)
        {
            StringBuilder sql = new StringBuilder("");
            try
            {
                sql.AppendFormat(@" select count(*) as countSta from web_content_type3 where content_status=1  and  pag_id='{0}';", model.page_id);
                return Convert.ToInt32(_access.getDataTable(sql.ToString()).Rows[0]["countSta"].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType3Dao.GetDefault-->" + ex.Message + sql.ToString(), ex); ;
            }
          
        }

        public Model.WebContentType3 GetModel(Model.WebContentType3 model)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"select * from web_content_type3 where content_id ={0}", model.content_id);
                return _access.getSinggleObj<WebContentType3>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType3Dao.GetDefault-->" + ex.Message + sb.ToString(), ex); 
            }
           
        }

        public int delete(Model.WebContentType3 model)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"update web_content_type3  set content_status=0 where content_id='{0}'", model.content_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType3Dao.delete-->" + ex.Message + sb.ToString(), ex); 
            }
           
        }
    }
}
