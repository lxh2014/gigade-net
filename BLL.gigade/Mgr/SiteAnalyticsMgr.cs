﻿using BLL.gigade.Dao;
using BLL.gigade.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public  class SiteAnalyticsMgr
    {
        private SiteAnalyticsDao _siteAnalytics;
        public SiteAnalyticsMgr(string connectionString)
        {
            _siteAnalytics = new SiteAnalyticsDao(connectionString);
        }

        public List<SiteAnalytics> GetSiteAnalyticsList(SiteAnalytics query, out int totalCount)
        {
            try
            {
                return _siteAnalytics.GetSiteAnalyticsList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("SiteAnalyticsMgr-->GetSiteAnalyticsList-->" + ex.Message, ex);
            }
        }

        public string ImportExcelToDt(DataTable _dt)
        {
            string  json = "{success:'true'}";
            ArrayList arrList = new ArrayList();
            SiteAnalytics query = new SiteAnalytics();
            try
            {
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    if (_dt.Rows[i][0].ToString() != "" && _dt.Rows[i][1].ToString() != "" && _dt.Rows[i][2].ToString() != "")
                    {
                        query = new SiteAnalytics();
                        query.s_sa_date = Convert.ToDateTime(_dt.Rows[i][0]).ToString("yyyy-MM-dd");
                        query.sa_id = _siteAnalytics.IsExist(query);
                        query.s_sa_date = Convert.ToDateTime(_dt.Rows[i][0]).ToString("yyyy-MM-dd");
                        query.sa_work_stage = Convert.ToInt32(_dt.Rows[i][1].ToString().Replace(',', ' ').Replace(" ", ""));
                        query.sa_user = Convert.ToInt32(_dt.Rows[i][2].ToString().Replace(',', ' ').Replace(" ", ""));
                        query.sa_create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                        if (query.sa_id > 0)
                        {
                          arrList.Add(_siteAnalytics.UpdateSNA(query));
                        }
                        else
                        {
                            arrList.Add(_siteAnalytics.InsertSNA(query));
                        }
                    }
                }
                if (arrList.Count > 0)
                {
                    if (_siteAnalytics.ExecSql(arrList))
                    {
                        json = "{success:'true'}";
                    }
                }
                return json;
               // return _siteAnalytics.ImportExcelToDt(query, _dt);
            }
            catch (Exception ex)
            {
                throw new Exception("SiteAnalyticsMgr-->ImportExcelToDt-->" + ex.Message, ex);
            }
        }

        public DataTable SiteAnalyticsDt(SiteAnalytics query)
        {
            try
            {
                return _siteAnalytics.SiteAnalyticsDt(query);
            }
            catch (Exception ex)
            {
                throw new Exception("SiteAnalyticsMgr-->SiteAnalyticsDt-->" + ex.Message, ex);
            }
        }
    }
}
