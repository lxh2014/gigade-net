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
        /// <summary>
        /// 查找列表數據
        /// </summary>
        /// <param name="appgory"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 獲取參數
        /// </summary>
        /// <param name="para">表中對應的其欄位</param>
        /// <param name="appgory">查詢的條件</param>
        /// <returns></returns>
        public string GetParaList(string para, Appcategory appgory)
        {
            try
            {
                List<Appcategory> appgoryPara = new List<Appcategory>();
                StringBuilder stb = new StringBuilder();//用於存儲json來返回前臺的數據
                StringBuilder sqlwhere = new StringBuilder();//用於sql語句的查詢條件，根據查詢條件是否有值，如果有則添加到查詢條件中
                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                //當前的邏輯
                //初次加載的時候進行四次查詢，分別獲取對應的四個欄位的參數
                //在用戶進行選擇一個下拉框欄位的時候，前臺會傳過來此欄位及父級所有的下拉框的值
                //附：分類一二三帶有一定的關係，但是并不是上下級關係。
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
        /// <summary>
        /// 刪除數據
        /// </summary>
        /// <param name="appgory"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 保存數據
        /// </summary>
        /// <param name="appgory"></param>
        /// <returns></returns>
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
