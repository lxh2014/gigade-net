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
    public class WebContentType5Dao : IWebContentType5ImplDao
    {

        private IDBAccess _access;
        string strSql = string.Empty;
        string _connStr = string.Empty;
        public WebContentType5Dao(string connectionString)
        {
            _connStr = connectionString;
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public int Insert(Model.WebContentType5 model)
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
                    smodel.web_content_type = "web_content_type5";
                    _setDao.UpdateLimitStatus(smodel);////當前已啟用的個數超過5筆時，使最舊的不啟用，
                }

                sb.AppendFormat(@"INSERT INTO web_content_type5(`site_id`,`page_id`,`area_id`,`type_id`,`brand_id`,`content_title`,`content_image`,`content_default`,`content_status`,`link_url`,`link_mode`,`update_on`,`created_on`) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}') ", model.site_id, model.page_id, model.area_id, model.type_id, model.brand_id, model.content_title, model.content_image, model.content_default, model.content_status, model.link_url, model.link_mode, CommonFunction.DateTimeToString(model.update_on), CommonFunction.DateTimeToString(model.created_on));
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType5Dao.Insert-->" + ex.Message + sb.ToString(), ex); 
            }
           
        }

        public int Update(Model.WebContentType5 model)
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                WebContentType5 oldModel = GetModel(model);
                if (model.content_status == 1 && oldModel.content_status != 1)//啟用
                {
                    WebContentTypeSetupDao _setDao = new WebContentTypeSetupDao(_connStr);
                    WebContentTypeSetup smodel = new WebContentTypeSetup();
                    smodel.site_id = model.site_id;
                    smodel.page_id = model.page_id;
                    smodel.area_id = model.area_id;
                    smodel.web_content_type = "web_content_type5";
                    _setDao.UpdateLimitStatus(smodel);////當前已啟用的個數超過5筆時，使最舊的不啟用，
                }
             
                sb.AppendFormat(@"update web_content_type5  set site_id='{0}',page_id='{1}',area_id='{2}',type_id='{3}',brand_id='{4}',content_title='{5}',content_image='{6}',`content_default`='{7}',content_status='{8}',link_url='{9}',link_mode='{10}',update_on='{11}',created_on='{12}' where content_id={13}",
                    model.site_id, model.page_id, model.area_id, model.type_id, model.brand_id, model.content_title, model.content_image, model.content_default, model.content_status, model.link_url, model.link_mode, CommonFunction.DateTimeToString(model.update_on), CommonFunction.DateTimeToString(model.created_on), model.content_id);
                return _access.execCommand(sb.ToString());//修改表數據
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType5Dao.Update-->" + ex.Message + sb.ToString(), ex); ;
            }
          
        }

        public List<Model.Query.WebContentType5Query> QueryAll(Model.Query.WebContentType5Query query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder str = new StringBuilder();
            StringBuilder strcondition = new StringBuilder();
            try
            {
                str.Append(@"select w5.* , t_site.parameterName as site_name,t_page.parameterName as page_name,t_area.parameterName as area_name ,v_b.brand_name as brand_name  from web_content_type5 w5 ");
                str.AppendFormat(" left join t_parametersrc t_site on t_site.parameterType='site_id' and t_site.parameterCode=w5.site_id");
                str.AppendFormat(" left join t_parametersrc t_page on t_page.parameterType='page_id' and t_page.parameterCode=w5.page_id and t_page.topValue=t_site.rowid");
                str.AppendFormat(" left join t_parametersrc t_area on t_area.parameterType='area_id' and t_area.parameterCode=w5.area_id and t_area.topValue=t_page.rowid");
                str.AppendFormat(" left join vendor_brand v_b on v_b.brand_id=w5.brand_id");
                strcondition.AppendFormat(" where 1=1 order by content_id desc ");
                totalCount = 0;

                if (query.IsPage)
                {
                    DataTable dt = _access.getDataTable(@"SELECT count(*) as totalcount from web_content_type5 ");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalcount"]);
                    }
                    strcondition.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                string strs = str.ToString() + strcondition.ToString();
                return _access.getDataTableForObj<WebContentType5Query>(strs.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType5Dao.QueryAll-->" + ex.Message + str.ToString(), ex); 
            }
          
        }

        /// <summary>
        /// 獲取已啟用的個數
        /// </summary>
        /// <returns></returns>
        public int GetDefault(WebContentType5 model)
        {
            StringBuilder sql = new StringBuilder("");
            try
            {
                sql.AppendFormat(@" select count(*) as countSta from web_content_type5 where content_status=1 and page_id={0};", model.page_id);
                return Convert.ToInt32(_access.getDataTable(sql.ToString()).Rows[0]["countSta"].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType5Dao.GetDefault-->" + ex.Message + sql.ToString(), ex); 
            }
         
        }

        public Model.WebContentType5 GetModel(Model.WebContentType5 model)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"select * from web_content_type5 where content_id ={0}", model.content_id);
                return _access.getSinggleObj<WebContentType5>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType5Dao.GetModel-->" + ex.Message + sb.ToString(), ex); 
            }
         
        }

        public int delete(Model.WebContentType5 model)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"update web_content_type5  set content_status=0 where content_id='{0}'", model.content_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType5Dao.delete-->" + ex.Message + sb.ToString(), ex); 
            }
        
        }
    }
}
