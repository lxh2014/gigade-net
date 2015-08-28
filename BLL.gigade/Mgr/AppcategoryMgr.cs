using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class AppcategoryMgr : IAppcategoryImplMgr
    {
        private IAppcategoryImplDao _iappcategoryImplDao;
        public AppcategoryMgr(string connectionStr)
        {
            _iappcategoryImplDao = new AppcategoryDao(connectionStr);
        }

        public List<Appcategory> GetAppcategoryList(Appcategory appgory, out int totalCount)
        {
            try
            {
                return _iappcategoryImplDao.GetAppcategoryList(appgory, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("AppcategoryMgr-->GetAppcategoryList-->" + ex.Message, ex);
            }
        }

        public string GetParaList(string para, Appcategory appgory)
        {
            try
            {
                List<Appcategory> appgoryPara = new List<Appcategory>();
                StringBuilder stb = new StringBuilder();
                StringBuilder sqlwhere = new StringBuilder();
                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                //根據傳遞的需求決定返回的字段，用於查詢的4個不同的combox，因為不是參數表的原因沒有Id和Code
                //sql.Append(SELECT category AS parameterName FROM appcategory)
                //sql.Append(WHERE category = query.name)
                //sql.Append(GROUP BY category)
                switch (para)
                {
                    case "category"://當需要查詢的欄位是category的時候
                        appgoryPara = _iappcategoryImplDao.GetParaList("SELECT category FROM appcategory GROUP BY category");
                        foreach (Appcategory act in appgoryPara)
                        {
                            stb.Append("{");
                            stb.Append(string.Format("\"parameterName\":\"{0}\"", act.category));
                            stb.Append("}");
                        }
                        break;
                    case "category1":
                        if (string.IsNullOrEmpty(appgory.category))
                        {
                            appgoryPara = _iappcategoryImplDao.GetParaList("SELECT category1 FROM appcategory GROUP BY category1");
                        }
                        else
                        {
                            appgoryPara = _iappcategoryImplDao.GetParaList(string.Format("SELECT category1 FROM appcategory WHERE category = '{0}' GROUP BY category1", appgory.category));
                        }
                        foreach (Appcategory act in appgoryPara)
                        {
                            stb.Append("{");
                            stb.Append(string.Format("\"parameterName\":\"{0}\"", act.category1));
                            stb.Append("}");
                        }
                        break;
                    case "category2":
                        if (!string.IsNullOrEmpty(appgory.category))
                        {
                            sqlwhere.AppendFormat(" AND category = '" + appgory.category + "'");
                        }
                        if (!string.IsNullOrEmpty(appgory.category1))
                        {
                            sqlwhere.AppendFormat(" AND category1 = '" + appgory.category1 + "'");
                        }
                        appgoryPara = _iappcategoryImplDao.GetParaList("SELECT category2 FROM appcategory WHERE 1=1" + sqlwhere.ToString() + " GROUP BY category2");

                        foreach (Appcategory act in appgoryPara)
                        {
                            stb.Append("{");
                            stb.Append(string.Format("\"parameterName\":\"{0}\"", act.category2));
                            stb.Append("}");
                        }
                        break;
                    case "category3":
                        if (!string.IsNullOrEmpty(appgory.category))
                        {
                            sqlwhere.AppendFormat(" AND category = '" + appgory.category + "'");
                        }
                        if (!string.IsNullOrEmpty(appgory.category1))
                        {
                            sqlwhere.AppendFormat(" AND category1 = '" + appgory.category1 + "'");
                        }
                        if (!string.IsNullOrEmpty(appgory.category2))
                        {
                            sqlwhere.AppendFormat(" AND category2 = '" + appgory.category2 + "'");
                        }
                        appgoryPara = _iappcategoryImplDao.GetParaList("SELECT category3 FROM appcategory WHERE 1=1" + sqlwhere.ToString() + " GROUP BY category3");

                        foreach (Appcategory act in appgoryPara)
                        {
                            stb.Append("{");
                            stb.Append(string.Format("\"parameterName\":\"{0}\"", act.category3));
                            stb.Append("}");
                        }
                        break;
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("AppcategoryMgr-->GetParaList-->" + ex.Message, ex);
            }
        }

        public int AppcategoryDelete(Appcategory appgory)
        {
            try
            {
                return _iappcategoryImplDao.AppcategoryDelete(appgory);
            }
            catch (Exception ex)
            {
                throw new Exception("AppcategoryMgr-->AppcategoryDelete-->" + ex.Message, ex);
            }
        }


        public int AppcategorySave(Appcategory appgory)
        {
            try
            {
                return _iappcategoryImplDao.AppcategorySave(appgory);
            }
            catch (Exception ex)
            {
                throw new Exception("AppcategoryMgr-->AppcategorySave-->" + ex.Message, ex);
            }
        }
    }
}
