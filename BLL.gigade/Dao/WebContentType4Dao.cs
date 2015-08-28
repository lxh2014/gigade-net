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
    public class WebContentType4Dao : IWebContentType4ImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        string _connStr = string.Empty;
        public WebContentType4Dao(string connectionString)
        {
            _connStr = connectionString;
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public int Insert(Model.WebContentType4 model)
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
                    smodel.web_content_type = "web_content_type4";
                    _setDao.UpdateLimitStatus(smodel);////當前已啟用的個數超過5筆時，使最舊的不啟用，
                }
         
                sb.AppendFormat(@"INSERT INTO web_content_type4(`site_id`,`page_id`,`area_id`,`type_id`,`brand_id`,`content_html`,`content_default`,`content_status`,`link_url`,`link_mode`,`update_on`,`created_on`) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}') ", model.site_id, model.page_id, model.area_id, model.type_id, model.brand_id, model.content_html, model.content_default, model.content_status, model.link_url, model.link_mode, CommonFunction.DateTimeToString(model.update_on), CommonFunction.DateTimeToString(model.update_on));
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType4Dao.Insert-->" + ex.Message + sb.ToString(), ex); 
            }
          
        }

        public int Update(Model.WebContentType4 model)
        {
            model.Replace4MySQL();
            StringBuilder sb = new StringBuilder();
            try
            {
                WebContentType4 oldModel = GetModel(model);
                if (model.content_status == 1 && oldModel.content_status != 1)//啟用
                {
                    WebContentTypeSetupDao _setDao = new WebContentTypeSetupDao(_connStr);
                    WebContentTypeSetup smodel = new WebContentTypeSetup();
                    smodel.site_id = model.site_id;
                    smodel.page_id = model.page_id;
                    smodel.area_id = model.area_id;
                    smodel.web_content_type = "web_content_type4";
                    _setDao.UpdateLimitStatus(smodel);////當前已啟用的個數超過5筆時，使最舊的不啟用，
                }
                
                sb.AppendFormat(@"update web_content_type4  set site_id='{0}',page_id='{1}',area_id='{2}',type_id='{3}',content_html='{4}',brand_id='{5}',`content_default`='{6}',content_status='{7}',link_url='{8}',link_mode='{9}',update_on='{10}' where content_id={11}",
    model.site_id, model.page_id, model.area_id, model.type_id, model.content_html, model.brand_id, model.content_default, model.content_status, model.link_url, model.link_mode, CommonFunction.DateTimeToString(model.update_on), model.content_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType4Dao.Update-->" + ex.Message + sb.ToString(), ex); 
            }
          
        }

        public List<Model.Query.WebContentType4Query> QueryAll(Model.Query.WebContentType4Query query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder str = new StringBuilder();
            StringBuilder strcondition = new StringBuilder();
            try
            {
                str.Append(@"select w4.* ,brand_name, t_site.parameterName as site_name,t_page.parameterName as page_name,t_area.parameterName as area_name from web_content_type4 w4 ");
                str.AppendFormat(" left join t_parametersrc t_site on t_site.parameterType='site_id' and t_site.parameterCode=w4.site_id");
                str.AppendFormat(" left join t_parametersrc t_page on t_page.parameterType='page_id' and t_page.parameterCode=w4.page_id and t_page.topValue=t_site.rowid");
                str.AppendFormat(" left join t_parametersrc t_area on t_area.parameterType='area_id' and t_area.parameterCode=w4.area_id and t_area.topValue=t_page.rowid");
                str.AppendFormat(" left join vendor_brand on w4.brand_id=vendor_brand.brand_id");
                if (!string.IsNullOrEmpty(query.serchwhere.Trim()))
                {
                    strcondition.AppendFormat(" where 1=1 and content_html like '%{0}%'", query.serchwhere);
                }
                else
                {
                    strcondition.AppendFormat(" where 1=1");
                }
                strcondition.AppendFormat(" order by content_id desc ");

                totalCount = 0;

                if (query.IsPage)
                {
                    DataTable dt = _access.getDataTable(@"SELECT count(*) as totalcount from web_content_type4 " + strcondition);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalcount"]);

                    }
                    strcondition.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                string strs = str.ToString() + strcondition.ToString();
                return _access.getDataTableForObj<WebContentType4Query>(strs.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType4Dao.QueryAll-->" + ex.Message + str.ToString(), ex); 
            }
        
        }

        /// <summary>
        /// 獲取已啟用的個數
        /// </summary>
        /// <returns></returns>
        public int GetDefault(WebContentType4 model)
        {
            StringBuilder sql = new StringBuilder("");
            try
            {
                sql.AppendFormat(@" select count(*) as countSta from web_content_type4 where content_status=1  and  pag_id='{0}';", model.page_id);
                return Convert.ToInt32(_access.getDataTable(sql.ToString()).Rows[0]["countSta"].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType4Dao.GetDefault-->" + ex.Message + sql.ToString(), ex); 
            }
           
        }

        public Model.WebContentType4 GetModel(Model.WebContentType4 model)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"select * from web_content_type4 where content_id ={0}", model.content_id);
                return _access.getSinggleObj<WebContentType4>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType4Dao.GetModel-->" + ex.Message + sb.ToString(), ex); 
            }
          
        }

        public int delete(Model.WebContentType4 model)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"update web_content_type4  set content_status=0 where content_id='{0}'", model.content_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentType4Dao.delete-->" + ex.Message + sb.ToString(), ex); 
            }
       
        }
    }
}
