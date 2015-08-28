using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class PageErrorLogMgr : IPageErrorLogMgr
    {
        string connectionstring = string.Empty;
        PageErrorLogDao dao;
      
        /// <summary>
        /// 更新數據的方式 -1:出錯  0:增 1:刪 2:改 3:查
        /// </summary>
        int oppType = -1;
        public PageErrorLogMgr(string _connectionstring)
        {
            this.connectionstring = _connectionstring;
            dao = new PageErrorLogDao(connectionstring);
        }


        /// <summary>
        /// 獲取錯誤類型參數
        /// </summary>
        /// <param name="queryPara"></param>
        /// <returns></returns>
        public string QueryPara(string strParaType)
        {
            StringBuilder stb = new StringBuilder();
            stb.Append("{");
            stb.Append(string.Format("success:true,items:["));
            stb.Append("{");
            stb.Append(string.Format("\"parameterName\":\"{0}\",\"parameterCode\":\"{1}\"", "全部","0"));
            stb.Append("}");
            try
            {
                List<Parametersrc> paraResult = dao.QueryPara(new Parametersrc { ParameterType = strParaType });
                
                if (strParaType == "page_error_type")
                {
                    foreach (Parametersrc para in paraResult)
                    {
                        stb.Append("{");
                        stb.Append(string.Format("\"parameterName\":\"{0}\",\"parameterCode\":\"{1}\"", para.parameterName, para.ParameterCode));
                        stb.Append("}");
                    }
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch
            {
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
        }
        #region 查分頁數據
        public List<PageErrorLogQuery> GetData(PageErrorLogQuery query, out int totalCount)
        {
            return dao.GetData(query, out totalCount);
        }
        #endregion
    }
}
