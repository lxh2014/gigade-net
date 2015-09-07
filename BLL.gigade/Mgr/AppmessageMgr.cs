using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class AppmessageMgr:IAppmessageImplMgr
    {
        private IAppmessageImplDao _iappmessageImplDao;
        public AppmessageMgr(string connectionStr)
        {
            _iappmessageImplDao = new AppmessageDao(connectionStr);
        }
        /// <summary>
        /// 獲取列表
        /// </summary>
        /// <param name="appmsg"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<AppmessageQuery> GetAppmessageList(AppmessageQuery appmsg, out int totalCount)
        {
            try
            {
                return _iappmessageImplDao.GetAppmessageList(appmsg, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("AppmessageMgr-->GetAppmessageList-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 獲取參數
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public string GetParaList(string para)
        {
            try
            {
                List<Appmessage> appgoryPara = new List<Appmessage>();
                StringBuilder stb = new StringBuilder();
                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                //根據傳遞的需求決定返回的字段，用於查詢的4個不同的combox，因為不是參數表的原因沒有Id和Code
                switch (para)
                {
                    case "fit_os"://當需要查詢的欄位是category的時候
                        appgoryPara = _iappmessageImplDao.GetParaList("SELECT fit_os FROM appmessage GROUP BY fit_os");
                        foreach (Appmessage amt in appgoryPara)
                        {
                            stb.Append("{");
                            stb.Append(string.Format("\"parameterName\":\"{0}\"", amt.fit_os));
                            stb.Append("}");
                        }
                        break;
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("AppmessageMgr-->GetParaList-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 插入數據
        /// </summary>
        /// <param name="appmsg"></param>
        /// <returns></returns>
        public int AppMessageInsert(Appmessage appmsg)
        {
            try
            {
                return _iappmessageImplDao.AppMessageInsert(appmsg);
            }
            catch (Exception ex)
            {
                throw new Exception("AppmessageMgr-->AppcategoryDelete-->" + ex.Message, ex);
            }
        }
    }
}
