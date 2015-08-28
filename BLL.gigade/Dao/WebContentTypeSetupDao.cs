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
    public class WebContentTypeSetupDao : IWebContentTypeSetupImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        string _connStr = string.Empty;
        public WebContentTypeSetupDao(string connectionString)
        {
            _connStr = connectionString;
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public List<WebContentTypeSetup> Query(WebContentTypeSetup model)
        {
            model.Replace4MySQL();
            string s = string.Format(@"select * from web_content_type_setup where 1=1 and content_status=1");
         
            try
            {
                if (!string.IsNullOrEmpty(model.web_content_type))
                {
                    s += string.Format(" and web_content_type='{0}'", model.web_content_type);
                }
                if (model.site_id != 0)
                {
                    s += string.Format(" and site_id='{0}'", model.site_id);
                }
                if (model.page_id != 0)
                {
                    s += string.Format(" and page_id='{0}'", model.page_id);
                }
                if (model.area_id != 0)
                {
                    s += string.Format(" and area_id='{0}'", model.area_id);
                }
                return _access.getDataTableForObj<WebContentTypeSetup>(s.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentTypeSetupDao.Query-->" + ex.Message + s.ToString(), ex);
            }
          
        }
        /// <summary>
        /// //當前已啟用的個數超過5筆時，使最舊的不啟用，
        /// </summary>
        public void UpdateLimitStatus(Model.WebContentTypeSetup model)
        {

            //獲取啟用的限制值
            List<WebContentTypeSetup> limitModel = Query(model);
            if (limitModel != null && limitModel[0].content_status_num != 0 && !string.IsNullOrEmpty(limitModel[0].content_status_num.ToString()))//WebContentTypeSetup表中啟用限制數目不為空
            {
                //獲取目標表中已啟用的個數
                string s = string.Format(@" select count(*) as countSta from {0} where content_status=1 and page_id='{1}' ;", model.web_content_type,model.page_id);
                int countSta = Convert.ToInt32(_access.getDataTable(s).Rows[0][0].ToString());
                while (countSta >= limitModel[0].content_status_num)//當前已啟用的個數超過限制筆時，使最舊的不啟用，
                {
                    string oldS = string.Format(@" select content_id as oldSta from {0} where content_status=1 and page_id='{1}' order by created_on ;", model.web_content_type,model.page_id);
                    int oldId = Convert.ToInt32(_access.getDataTable(oldS).Rows[0][0].ToString());//獲取已啟用的最舊的id

                    string updateOldS = string.Format(@" update {0}  set content_status=0 ,content_default=1  where content_id={1} ", model.web_content_type, oldId);
                    _access.execCommand(updateOldS.ToString());//使最舊的啟用變為不啟用
                    countSta--;
                }
            }

        }

        

        public DataTable QueryPageStore(WebContentTypeSetup model)
        {
            model.Replace4MySQL();

            StringBuilder str = new StringBuilder(@"select DISTINCT (ws.page_id) , t_page.parameterName as page_name ");
         
            try
            {
                str.Append(" from web_content_type_setup ws  left join t_parametersrc t_site on t_site.parameterType='site_id' and t_site.parameterCode=ws.site_id ");
                str.Append(" left join t_parametersrc t_page on t_page.parameterType='page_id' and t_page.parameterCode=ws.page_id and t_page.topValue=t_site.rowid ");
                str.Append(" where 1=1 and  ws.content_status=1");
                if (!string.IsNullOrEmpty(model.web_content_type))
                {
                    str.AppendFormat(" and ws.web_content_type='{0}'", model.web_content_type);
                }
                if (model.site_id != 0)
                {
                    str.AppendFormat(" and ws.site_id='{0}'", model.site_id);
                }
                if (model.page_id != 0)
                {
                    str.AppendFormat(" and ws.page_id='{0}'", model.page_id);
                }
                if (model.area_id != 0)
                {
                    str.AppendFormat(" and ws.area_id='{0}'", model.area_id);
                }
                return _access.getDataTable(str.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentTypeSetupDao.QueryPageStore-->" + ex.Message + str.ToString(), ex);
            }
           
        }
        public DataTable QueryAreaStore(WebContentTypeSetup model)
        {
            model.Replace4MySQL();

            StringBuilder str = new StringBuilder(@"select DISTINCT (ws.area_id) , t_area.parameterName as 'area_name_s' ");
            try
            {
                str.Append(" from web_content_type_setup ws  left join t_parametersrc t_site on t_site.parameterType='site_id' and t_site.parameterCode=ws.site_id ");
                str.Append(" left join t_parametersrc t_page on t_page.parameterType='page_id' and t_page.parameterCode=ws.page_id and t_page.topValue=t_site.rowid ");
                str.Append(" left join t_parametersrc t_area on t_area.parameterType='area_id' and t_area.parameterCode=ws.area_id and t_area.topValue=t_page.rowid ");
                str.Append(" where 1=1 and ws.content_status=1");
                if (!string.IsNullOrEmpty(model.web_content_type))
                {
                    str.AppendFormat(" and ws.web_content_type='{0}'", model.web_content_type);
                }
                if (model.site_id != 0)
                {
                    str.AppendFormat(" and ws.site_id='{0}'", model.site_id);
                }
                if (model.page_id != 0)
                {
                    str.AppendFormat(" and ws.page_id='{0}'", model.page_id);
                }
                if (model.area_id != 0)
                {
                    str.AppendFormat(" and ws.area_id='{0}'", model.area_id);
                }

                return _access.getDataTable(str.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("WebContentTypeSetupDao.QueryAreaStore-->" + ex.Message + str.ToString(), ex);
            }
           
        }

    }
}

